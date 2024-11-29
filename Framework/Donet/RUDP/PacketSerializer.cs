using System;
using System.Threading;

namespace Donet.RUDP
{
    public static class PacketSerializer
    {
        private static ThreadLocal<Serializer> localSerializer = new ThreadLocal<Serializer>();
        private static Serializer serializer => localSerializer.Value;

        public static int Serialize(ArraySegment<byte> buffer, ISerializablePacket packet)
        {

        }

        public static bool Deserialize(ArraySegment<byte> buffer, ISerializablePacket packet)
        {

        }
    }
}
