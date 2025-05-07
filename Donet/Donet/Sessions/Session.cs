using System;
using System.Net.Sockets;
using System.Threading;

using Donet.Utils;

namespace Donet.Sessions
{
    public delegate void SessionClose(Session session);

    [Serializable]
    public class Session
    {
        public int id = 0;
        private Socket socket = null;

        public Socket Socket => socket;

        public bool Active => active == 1;

        private SessionSender sender = new SessionSender();
        private SessionReceiver receiver = new SessionReceiver();

        public SendHandle sended = null;
        public ReceiveHandle received = null;
        public SessionClose closed = null;

        private volatile int active = 0;

        public void Initialize(int id, Socket socket)
        {
            if (Interlocked.CompareExchange(ref active, 1, 0) == 1)
                return;

            this.id = id;
            this.socket = socket;

            sender.Initialize(socket, this);
            receiver.Initialize(socket, this);

            received += HandleReceive;
        }

        private void HandleReceive(ushort id, ArraySegment<byte> body)
        {
            IPacket packet = PacketFactory.PopPacket(id);
            Serializer serializer = new Serializer();
            serializer.Open(NetworkSerializeMode.Deserialize, body, 0);
            serializer.SerializeObject(ref packet);
            packet.OnReceived(this);
            PacketFactory.PushPacket(packet);
        }

        public void Send(RawPacket packet)
        {
            sender.Send(packet);
        }

        public void Send<T>(T packet) where T : IPacket
        {
            MemorySegment memory = GetMemorySegment();
            Serializer serializer = new Serializer();
            serializer.Open(NetworkSerializeMode.Serialize, memory.segment, 2);
            ushort id = PacketFactory.GetId(packet);
            serializer.Serialize(ref id);
            serializer.SerializeObject(ref packet);
            ushort size = serializer.GetOffset();
            serializer.SetOffset(0);
            serializer.Serialize(ref size);
            RawPacket raw = new RawPacket();
            raw.memory = memory;
            raw.data = new ArraySegment<byte>(memory.segment.Array, memory.segment.Offset, size);
            Send(raw);
        }

        private MemorySegment GetMemorySegment()
        {
            if (sender.memoryContainer.Count > 0)
                if (sender.memoryContainer.TryDequeue(out MemorySegment memory))
                    return memory;
            return MemoryPool.DequeueSendMemory();
        }

        public void Close()
        {
            if (Interlocked.CompareExchange(ref active, 0, 1) == 0)
                return;

            socket.Shutdown(SocketShutdown.Both);

            receiver.Dispose();
            sender.Dispose();

            sended = null;
            received = null;

            closed?.Invoke(this);
            closed = null;

            id = 0;
            socket.Close();
            socket = null;
        }
    }
}
