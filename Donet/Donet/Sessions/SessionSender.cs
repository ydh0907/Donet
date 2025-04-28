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

        private volatile int sending = 0;

        public void Initialize(Socket socket, Session session)
        {
            this.socket = socket;
            this.session = session;

            sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += HandleSendCompleted;
            packetQueue.Clear();
            memory = MemoryPool.DequeueSendMemory();
        }

        public void Dispose()
        {
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

            if (Interlocked.CompareExchange(ref sending, 1, 0) == 0)
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
                    Interlocked.Exchange(ref sending, 0);
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

            Logger.Log(level, message);

            session?.Close();
        }
    }
}
