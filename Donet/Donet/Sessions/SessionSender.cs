using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

using Donet.Utils;

namespace Donet.Sessions
{
    public delegate void SendHandle(int byteTransferred);

    public class SessionSender
    {
        private Socket socket = null;
        private Session session = null;

        private SendHandle handler => session.sended;

        private ConcurrentQueue<IPacket> pendingQueue = new ConcurrentQueue<IPacket>();
        private List<IPacket> sendingQueue = new List<IPacket>();
        private MemorySegment memory;

        private Serializer serializer = new Serializer();

        private Atomic<bool> sending = new Atomic<bool>();

        public void Initialize(Socket socket, Session session)
        {
            using (var local = sending.Locker)
                local.Set(false);

            this.socket = socket;
            this.session = session;

            pendingQueue.Clear();
            sendingQueue.Clear();
            memory = MemoryPool.DequeueSendMemory();
        }

        public void Dispose()
        {
            while (true)
                using (var local = sending.Locker)
                    if (!local.Value)
                        break;

            socket = null;
            session = null;

            pendingQueue.Clear();
            sendingQueue.Clear();
            MemoryPool.EnqueueSendMemory(memory);
        }

        public void Send(IPacket packet)
        {
            pendingQueue.Enqueue(packet);
            using (var local = sending.Locker)
            {
                if (!local.Value)
                {
                    local.Set(true);
                    Task.Run(Flash);
                }
            }
        }

        private void Flash()
        {
            while (pendingQueue.Count > 0)
                if (pendingQueue.TryDequeue(out IPacket packet))
                    sendingQueue.Add(packet);

            try
            {
                ushort offset = 0;
                while (sendingQueue.Count > 0)
                {
                    IPacket packet = sendingQueue[0];
                    sendingQueue.RemoveAt(0);
                    try
                    {
                        ushort packetId = PacketFactory.GetPacketId(packet);
                        serializer.Open(NetworkSerializeMode.Serialize, memory.segment, (ushort)(offset + 2));
                        serializer.Serialize(ref packetId);
                        serializer.SerializeObject(ref packet);
                        ushort next = serializer.GetOffset();
                        serializer.SetOffset(offset);
                        ushort size = (ushort)(next - offset);
                        serializer.Serialize(ref size);
                        offset = next;
                    }
                    catch
                    {
                        sendingQueue.Insert(0, packet);
                        break;
                    }
                }

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += HandleSendCompleted;
                args.SetBuffer(memory.segment.Slice(0, offset));
                bool pending = socket.SendAsync(args);
                if (!pending)
                    HandleSendCompleted(socket, args);
            }
            catch (Exception ex)
            {
                HandleError(LogLevel.Warning, ex.ToString());
            }
        }

        private void HandleSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success || args.BytesTransferred == 0)
            {
                HandleError(LogLevel.Warning, $"[Session {args.SocketError}] packet sending failed please check session");
                return;
            }

            try
            {
                if (pendingQueue.Count > 0)
                    Task.Run(Flash);
                else
                    using (var local = sending.Locker)
                        local.Set(false);
            }
            catch (Exception ex)
            {
                HandleError(LogLevel.Warning, ex.Message);
            }
        }

        private void HandleError(LogLevel level, string message)
        {
            if (session == null)
                return;

            using (var local = sending.Locker)
                local.Set(false);

            Logger.Log(level, message);

            session?.Close();
        }
    }
}
