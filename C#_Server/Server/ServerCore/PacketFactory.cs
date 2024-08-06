namespace ServerCore
{
    public static class PacketFactory
    {
        private static Dictionary<Guid, ushort> packetDictionary = new Dictionary<Guid, ushort>();
        private static Dictionary<ushort, Func<Packet<T>>> packetFactory = new Dictionary<ushort, Func<Packet<T>>>();
        private static ushort nextID = 0;
        private static ushort NextID => nextID++;

        public static ushort GetID<T>(Guid guid) where T : Packet<T>, new()
        {
            if (!packetDictionary.ContainsKey(guid))
            {
                ushort id = NextID;
                packetDictionary[guid] = id;
                packetFactory[id] = () => new T();
            }
            return packetDictionary[guid];
        }

        public static Packet<?> GetPacket(ushort id)
        {
            return packetFactory[id]() as Packet<?>;
        }
    }
}
