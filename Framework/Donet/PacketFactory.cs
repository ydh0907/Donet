using System;
using System.Collections.Generic;
using System.Reflection;

namespace Donet
{
    public static class PacketFactory<T>
    {
        private static Dictionary<Type, ushort> packetDictionary = new Dictionary<Type, ushort>();
        private static Dictionary<ushort, T> packetFactory = new Dictionary<ushort, T>();

        private static ushort nextID = 0;
        private static ushort NextID => nextID++;

        private static bool initialized = false;

        public static bool InitializePacket<E>() where E : Enum
        {
            if (initialized)
                return true;

            try
            {
                Type packetEnum = typeof(E);

                string space = packetEnum.Namespace;
                if (!string.IsNullOrEmpty(space))
                    space = $"{space}.";

                Assembly assembly = Assembly.GetAssembly(packetEnum);

                foreach (E typeEnum in Enum.GetValues(typeof(E)))
                {
                    string typeName = $"{space}{typeEnum}";

                    T packet = (T)assembly.CreateInstance(typeName);

                    ushort id = NextID;
                    Type type = packet.GetType();

                    packetDictionary.Add(type, id);
                    packetFactory.Add(id, (T)Activator.CreateInstance(type));
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

        public static T GetPacket(ushort id)
        {
            return packetFactory[id];
        }

        public static ushort ReadPacketID(ArraySegment<byte> buffer)
        {
            return BitConverter.ToUInt16(new ReadOnlySpan<byte>(buffer.Array, buffer.Offset + 2, sizeof(ushort)));
        }
    }
}
