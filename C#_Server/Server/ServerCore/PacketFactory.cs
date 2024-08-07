using System.Reflection;

namespace ServerCore
{
    public static class PacketFactory
    {
        private static Dictionary<string, ushort> packetDictionary = new Dictionary<string, ushort>();
        private static Dictionary<ushort, Func<Packet>> packetFactory = new Dictionary<ushort, Func<Packet>>();
        private static ushort nextID = 0;
        private static ushort NextID => nextID++;

        private static bool initalized = false;

        public static bool InitializePacket<T>() where T : Enum
        {
            if (initalized)
                return true;

            try
            {
                Type packetEnum = typeof(T);

                string space = packetEnum.Namespace;
                if (!string.IsNullOrEmpty(space))
                    space = $"{space}.";

                Assembly assembly = Assembly.GetAssembly(packetEnum);

                foreach (T type in Enum.GetValues(typeof(T)))
                {
                    string typeName = $"{space}{type}";
                    Packet packet = null;

                    if (assembly != null)
                        packet = assembly.CreateInstance(typeName) as Packet;

                    ushort id = NextID;
                    packetDictionary.Add(type.ToString(), id);
                    packetFactory.Add(id, packet.CreatePacket);
                }
                initalized = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static ushort GetID(Type packet)
        {
            if (initalized)
                return packetDictionary[packet.Name];
            else
                return 0;
        }

        public static Packet GetPacket(ushort id)
        {
            return packetFactory[id]();
        }

        public static ushort ReadPacketID(ArraySegment<byte> buffer)
        {
            return BitConverter.ToUInt16(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + 2, sizeof(ushort)));
        }
    }
}
