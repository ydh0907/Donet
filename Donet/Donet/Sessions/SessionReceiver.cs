using System;
using System.Net.Sockets;
using System.Threading;

using Donet.Utils;

namespace Donet.Sessions
{
    public delegate void ReceiveHandle(ushort id, IPacket packet);

    public class SessionReceiver : IDisposable
    {
        private Socket socket = null;
        private Session session = null;

        private ReceiveHandle handler => session.received;

        private SocketAsyncEventArgs receiveArgs = null;
        private MemorySegment memory = null;
        private int left = 0;
        private int right = 0;

        private Serializer serializer = new Serializer();

        private Atomic<bool> receiving = new Atomic<bool>(false);

        public void Initialize(Socket socket, Session session)
        {
            using (var local = receiving.Locker)
                local.Set(false);

            this.socket = socket;
            this.session = session;

            receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.Completed += HandleReceiveCompleted;
            memory = MemoryPool.DequeueReceiveMemory();
            left = 0;
            right = 0;

            Receive();
        }

        public void Dispose()
        {
            bool active = true;
            SpinWait wait = new SpinWait();
            while (active)
                using (var local = receiving.Locker)
                {
                    active = local.Value;
                    wait.SpinOnce();
                }

            socket = null;
            session = null;

            receiveArgs.Dispose();
            receiveArgs = null;
            left = 0;
            right = 0;
            MemoryPool.EnqueueReceiveMemory(memory);
            memory = null;
        }

        private void Receive()
        {
            try
            {
                using (var local = receiving.Locker)
                    local.Set(true);

                receiveArgs.SetBuffer(new ArraySegment<byte>(memory.segment.Array, memory.segment.Offset + right, memory.segment.Count - right));

                bool pending = socket.ReceiveAsync(receiveArgs);
                if (!pending)
                    HandleReceiveCompleted(socket, receiveArgs);
            }
            catch (Exception ex)
            {
                HandleError(LogLevel.Warning, ex.Message);
            }
        }

        private void HandleReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success || args.BytesTransferred == 0)
            {
                if (args.SocketError == SocketError.ConnectionReset)
                    HandleError(LogLevel.Notify, $"[Session {args.SocketError}] socket connection reset by remote.");
                else
                    HandleError(LogLevel.Warning, $"[Session {args.SocketError}] packet receiving failed please check session.");
                return;
            }

            try
            {
                right += args.BytesTransferred;
                if (right == memory.segment.Count)
                    ClearMemory();

                DeserializePacket();
            }
            catch (Exception ex)
            {
                HandleError(LogLevel.Warning, ex.Message);
                return;
            }

            using (var local = receiving.Locker)
                local.Set(false);

            Receive();
        }

        private void ClearMemory()
        {
            Array.Copy(
                memory.segment.Array,
                memory.segment.Offset + left,
                memory.segment.Array,
                memory.segment.Offset,
                right - left);
            right -= left;
            left = 0;
        }

        private void DeserializePacket()
        {
            int raw = right - left;
            while (raw >= 4)
            {
                var seg = memory.segment;
                serializer.Open(
                    NetworkSerializeMode.Deserialize,
                    new ArraySegment<byte>(seg.Array, seg.Offset + left, raw)
                );

                ushort size = 0;
                serializer.Serialize(ref size);

                if (size == 0)
                    throw new Exception("packet size is zero.");

                if (size <= raw)
                {
                    ushort id = 0;
                    serializer.Serialize(ref id);

                    IPacket packet = PacketFactory.GetPacket(id);
                    serializer.SerializeObject(ref packet);

                    packet.OnReceived(session);
                    left += size;
                    raw -= size;

                    handler?.Invoke(id, packet);
                }
                else
                    break;
            }
        }

        private void HandleError(LogLevel level, string message)
        {
            if (session == null)
                return;

            using (var local = receiving.Locker)
                local.Set(false);

            Logger.Log(level, message);

            session?.Close();
        }
    }
}
