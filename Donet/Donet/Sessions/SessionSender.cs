using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;

using Donet.Utils;

namespace Donet.Sessions
{
    public delegate void SendHandle(int byteTransferred);

    public class SessionSender : IDisposable
    {
        private Socket socket = null;
        private Session session = null;

        private SendHandle handler => session.sended;

        private SocketAsyncEventArgs sendArgs = null;
        private ConcurrentQueue<IPacket> packetQueue = new ConcurrentQueue<IPacket>();
        private MemorySegment memory = null;

        private Serializer serializer = new Serializer();

        private Atomic<bool> sending = new Atomic<bool>();

        public void Initialize(Socket socket, Session session)
        {
            using (var local = sending.Locker)
                local.Set(false);

            this.socket = socket;
            this.session = session;

            sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += HandleSendCompleted;
            packetQueue.Clear();
            memory = MemoryPool.DequeueSendMemory();
        }

        public void Dispose()
        {
            bool active = true;
            SpinWait wait = new SpinWait();
            while (active)
                using (var local = sending.Locker)
                {
                    active = local.Value;
                    wait.SpinOnce();
                }

            socket = null;
            session = null;

            sendArgs.Dispose();
            sendArgs = null;
            packetQueue.Clear();
            MemoryPool.EnqueueSendMemory(memory);
        }

        public void Send(IPacket packet)
        {
            packetQueue.Enqueue(packet);

            using (var local = sending.Locker)
            {
                if (!local.Value)
                {
                    local.Set(true);
                }
                else return;
            }

            Flash();
        }

        private void Flash()
        {
            try
            {
                ushort offset = 0;
                while (packetQueue.TryDequeue(out IPacket packet))
                {
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
                        packetQueue.Enqueue(packet);
                        break;
                    }
                }

                if (offset > 0)
                {
                    sendArgs.SetBuffer(memory.segment.Slice(0, offset));
                    bool pending = socket.SendAsync(sendArgs);
                    if (!pending)
                        HandleSendCompleted(socket, sendArgs);
                }
                else
                {
                    using (var local = sending.Locker)
                        local.Set(false);
                }
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
                if (!packetQueue.IsEmpty)
                    Flash();
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
