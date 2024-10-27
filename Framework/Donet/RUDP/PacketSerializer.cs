using System;
using System.Threading;

namespace Donet.RUDP
{
    public static class PacketSerializer
    {
        private static ThreadLocal<Serializer> localSerializer = new ThreadLocal<Serializer>();
        private static Serializer serializer => localSerializer.Value;

        public static int SerializePacket(ArraySegment<byte> buffer, RUDPPacket packet)
        {
            switch (packet.Type)
            {

            }
            return -1;
        }
    }
}
