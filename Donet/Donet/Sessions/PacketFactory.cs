using System;
using System.Collections.Generic;

namespace Donet.Sessions
{
    public static class PacketFactory
    {
        private static Dictionary<ushort, IPacket> id2p = null;
        private static Dictionary<Type, ushort> p2id = null;

        public static void Initialize(params IPacket[] packets)
        {
            id2p = new Dictionary<ushort, IPacket>();
            p2id = new Dictionary<Type, ushort>();

            for (ushort i = 0; i < packets.Length; i++)
            {
                id2p.Add(i, packets[i]);
                p2id.Add(packets[i].GetType(), i);
            }
        }

        public static void Dispose()
        {
            id2p?.Clear();
            p2id?.Clear();
            id2p = null;
            p2id = null;
        }

        public static ushort GetPacketId(IPacket packet)
        {
            return p2id[packet.GetType()];
        }

        internal static IPacket GetPacket(ushort id)
        {
            return id2p[id];
        }
    }
}
