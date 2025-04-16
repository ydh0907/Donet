using System;
using System.Net.Sockets;
using System.Threading.Tasks;

using Donet.Utils;

namespace Donet.Sessions
{
    public delegate void ReceiveHandle(ushort id, IPacket packet);

    public class SessionReceiver
    {
        private Socket socket = null;
        private Session session = null;
        private ReceiveHandle handler = null;

        private MemorySegment memory = null;
        private int left = 0;
        private int right = 0;

        private Serializer serializer = new Serializer();

        public void Initialize(Socket socket, Session session, ReceiveHandle handler)
        {
            this.socket = socket;
            this.session = session;
            this.handler = handler;

            memory = MemoryPool.DequeueReceiveMemory();
            left = 0;
            right = 0;

            Receive();
        }

        public void Dispose()
        {
            socket = null;
            session = null;
            handler = null;

            left = 0;
            right = 0;
            MemoryPool.EnqueueReceiveMemory(memory);
        }

        private void Receive()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += HandleReceive;
            args.SetBuffer(memory.segment.Array, memory.segment.Offset + right, memory.segment.Count - right);
            bool pending = socket.ReceiveAsync(args);
            if (!pending)
                HandleReceive(socket, args);
        }

        private void HandleReceive(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success || args.BytesTransferred == 0)
            {
                Logger.Log(LogLevel.Warning, "[Session] packet receiving failed please check connection");

                if (session != null)
                    using (var active = session.Active.Lock())
                        if (active.Value)
                            session.Close();
                return;
            }

            right += args.BytesTransferred;
            if (right >= memory.segment.Count)
                ClearMemory();
            DeserializePacket();
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
            try
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
                    if (size <= raw)
                    {
                        ushort id = 0;
                        serializer.Serialize(ref id);
                        left += 4;
                        size -= 4;

                        IPacket packet = PacketFactory.GetPacket(id);
                        serializer.Open(
                            NetworkSerializeMode.Deserialize,
                            new ArraySegment<byte>(seg.Array, seg.Offset + left, size)
                        );
                        serializer.SerializeObject(ref packet);

                        packet.OnReceived(session);
                        left += size;

                        Task.Run(() => handler?.Invoke(id, packet));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message);

                using (var active = session.Active.Lock())
                    if (active.Value)
                        session.Close();
            }
        }
    }
}
