using System;
using System.Threading;

namespace Donet.RUDP
{
    public static class PacketSerializer
    {
        private static ThreadLocal<Serializer> localSerializer = new ThreadLocal<Serializer>();
        private static Serializer serializer => localSerializer.Value;

        public static HeaderArgs SerializePacket(ArraySegment<byte> buffer, Packet packet, HeaderType type = HeaderType.None, HeaderArgs args = default)
        {
            return args;
        }
    }
}
