using System;
using System.Collections.Generic;
using System.Reflection;

namespace Donet
{
    public static class PacketFactory
    {
        private static Dictionary<Type, ushort> packetDictionary = new Dictionary<Type, ushort>();
        private static Dictionary<ushort, Func<Packet>> packetFactory = new Dictionary<ushort, Func<Packet>>();

        private static ushort nextID = 0;
        private static ushort NextID => nextID++;

        private static bool initialized = false;

        public static bool InitializePacket<T>() where T : Enum
        {
            if (initialized)
                return true;

            try
            {
                Type packetEnum = typeof(T);

                string space = packetEnum.Namespace;
                if (!string.IsNullOrEmpty(space))
                    space = $"{space}.";

                Assembly assembly = Assembly.GetAssembly(packetEnum);

                foreach (T typeEnum in Enum.GetValues(typeof(T)))
                {
                    string typeName = $"{space}{typeEnum}";
                    Packet packet = null;

                    if (assembly != null)
                        packet = assembly.CreateInstance(typeName) as Packet;

                    ushort id = NextID;
                    Type type = packet.GetType();

                    packetDictionary.Add(type, id);
                    packetFactory.Add(id, packet.GetEmpty);
                }
                initialized = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static ushort GetID(Type packet)
        {
            if (initialized)
                return packetDictionary[packet];
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
