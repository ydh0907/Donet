using System;
using System.Collections.Generic;
using System.Threading;

namespace Donet.Sessions
{
    public static class PacketFactory
    {
        private static Dictionary<ushort, IPacket> idPackets = new Dictionary<ushort, IPacket>();
        private static Dictionary<Type, ushort> typeId = new Dictionary<Type, ushort>();

        private static ThreadLocal<Dictionary<ushort, Queue<IPacket>>> localPool = new ThreadLocal<Dictionary<ushort, Queue<IPacket>>>();

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

        public static IPacket PopPacket(ushort id)
        {
            if (!localPool.IsValueCreated)
                CreateLocalPool();

            if (localPool.Value.Count == 0)
                return idPackets[id].Create();

            return localPool.Value[id].Dequeue();
        }

        public static void PushPacket(IPacket packet)
        {
            ushort id = typeId[packet.GetType()];
            PushPacket(id, packet);
        }

        public static void PushPacket(ushort id, IPacket packet)
        {
            if (!localPool.IsValueCreated)
                CreateLocalPool();
            localPool.Value[id].Enqueue(packet);
        }

        private static void CreateLocalPool()
        {
            localPool.Value = new Dictionary<ushort, Queue<IPacket>>();
            for (int i = 0; i < packets.Length; i++)
            {
                Queue<IPacket> queue = new Queue<IPacket>();
                for (int j = 0; j < poolCount; j++)
                    queue.Enqueue(packets[i].Create());
                localPool.Value[(ushort)i] = queue;
            }
        }
    }
}
