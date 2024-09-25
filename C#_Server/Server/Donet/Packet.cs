using System;
using Donet.Tcp;

namespace Donet
{
    public abstract class Packet
    {
        private ushort packetID;
        private Serializer serializer = new Serializer();

        public Packet()
        {
            packetID = PacketFactory.GetID(GetType());
        }

        public int Serialize(ArraySegment<byte> buffer)
        {
            serializer.Open(NetworkSerializeMode.Serialize, buffer);
            OnSerialize(serializer);
            serializer.WriteID(packetID, buffer);
            serializer.WriteSize(buffer);
            return serializer.Success ? serializer.Close() : -1;
        }

        public bool Deserialize(ArraySegment<byte> buffer)
        {
            serializer.Open(NetworkSerializeMode.Deserialize, buffer);
            OnSerialize(serializer);
            return serializer.Success && serializer.Close() == buffer.Count;
        }

        public abstract void OnSerialize(Serializer serializer); // use serializer for easy serialize
        public abstract void OnReceived(TcpSession session);
    }
}
