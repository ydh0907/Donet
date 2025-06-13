using System;
using System.Collections.Generic;
using System.Threading;

namespace Donet.Sessions
{
    public static class PacketFactory
    {
        private static Dictionary<ushort, IPacket> idPackets = new Dictionary<ushort, IPacket>();
        private static Dictionary<Type, ushort> typeId = new Dictionary<Type, ushort>();

        private static ThreadLocal<Dictionary<ushort, IPacket>> localPool = new ThreadLocal<Dictionary<ushort, IPacket>>();

        private static int poolCount = 0;
        private static IPacket[] packets = null;

        public static void Initialize(int count = 16, params IPacket[] packets)
        {
            poolCount = count;
            PacketFactory.packets = packets;

            for (int i = 0; i < packets.Length; i++)
            {
                idPackets.Add((ushort)i, packets[i]);

                Type type = packets[i].GetType();
                typeId[type] = (ushort)i;
            }
        }

        public static void Dispose()
        {
            idPackets.Clear();
            typeId.Clear();

            poolCount = 0;
            packets = null;

            localPool.Dispose();
        }

        public static ushort GetId<T>(T packet) where T : IPacket
        {
            return typeId[packet.GetType()];
        }

        public static IPacket GetPacket(ushort id)
        {
            if (!localPool.IsValueCreated)
                CreateLocalPool();

            if (localPool.Value.Count == 0)
                return idPackets[id].Create();

            return localPool.Value[id];
        }

        private static void CreateLocalPool()
        {
            localPool.Value = new Dictionary<ushort, IPacket>();
            for (int i = 0; i < packets.Length; i++)
            {
                localPool.Value[(ushort)i] = packets[i].Create();
            }
        }
    }
}
