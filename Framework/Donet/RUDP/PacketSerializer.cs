using System;
using System.Threading;

namespace Donet.RUDP
{
    public static class PacketSerializer
    {
        private static ThreadLocal<Serializer> localSerializer = new ThreadLocal<Serializer>();
        private static Serializer serializer => localSerializer.Value;

        public static int SerializePacket(ArraySegment<byte> buffer, IPacket packet)
        {
            serializer.Open(NetworkSerializeMode.Serialize, buffer);
            packet.OnSerialize(serializer);
            serializer.WriteID(PacketFactory<IPacket>.GetID(packet.GetType()), buffer);
            serializer.WriteSize(buffer);
            return serializer.Success ? serializer.Close() : -1;
        }

        public static bool Deserialize(ArraySegment<byte> buffer, IPacket packet)
        {
            serializer.Open(NetworkSerializeMode.Deserialize, buffer);
            packet.OnSerialize(serializer);
            return serializer.Success && serializer.Close() == buffer.Count;
        }
    }
}
