namespace ServerCore
{
    internal abstract class Packet
    {

    }

    public abstract class Packet<T> where T : Packet<T>, new()
    {
        private static T packet = new();

        private Serializer serializer = new();
        private ushort packetID;

        public Packet()
        {
            packetID = PacketFactory.GetID<T>(GetType().GUID);
        }

        public int Serialize(ArraySegment<byte> buffer)
        {
            serializer.Open(buffer, SerializeMode.Read);
            OnSerialize(serializer);
            serializer.WriteSize(buffer);
            serializer.WriteID(packetID, buffer);
            return serializer.Success ? serializer.Close() : -1;
        }

        public bool Deserialize(ArraySegment<byte> buffer)
        {
            serializer.Open(buffer, SerializeMode.Write);
            OnSerialize(serializer);
            return serializer.Success && serializer.Close() == buffer.Count;
        }

        public abstract void OnSerialize(Serializer serializer); // use serializer for easy serialize
        public abstract void OnPacketReceived();
    }
}
