namespace ServerCore
{
    public abstract class Packet
    {
        private ushort packetID;
        private Serializer serializer = new();

        public Packet()
        {
            packetID = PacketFactory.GetID(GetType());
        }

        public int Serialize(ArraySegment<byte> buffer)
        {
            serializer.Open(buffer, SerializeMode.Serialize);
            OnSerialize(serializer);
            serializer.WriteID(packetID, buffer);
            serializer.WriteSize(buffer);
            return serializer.Success ? serializer.Close() : -1;
        }

        public bool Deserialize(ArraySegment<byte> buffer)
        {
            serializer.Open(buffer, SerializeMode.Deserialize);
            OnSerialize(serializer);
            return serializer.Success && serializer.Close() == buffer.Count;
        }

        public Packet CreatePacket()
        {
            return Activator.CreateInstance(GetType()) as Packet;
        }

        public abstract void OnSerialize(Serializer serializer); // use serializer for easy serialize
        public abstract void OnReceived(Session session);
    }
}
