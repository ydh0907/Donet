using System;
using System.Threading;

namespace Donet.TCP
{
    public static class TCPPacketSerializer
    {
        private static ThreadLocal<Serializer> serializer = new ThreadLocal<Serializer>();
        public static Serializer Serializer => serializer.Value;

        public static int Serialize(ArraySegment<byte> buffer, Packet packet)
        {
            Serializer.Open(NetworkSerializeMode.Serialize, buffer);
            packet.OnSerialize(Serializer);
            Serializer.WriteID(PacketFactory.GetID(packet.GetType()), buffer);
            Serializer.WriteSize(buffer);
            return Serializer.Success ? Serializer.Close() : -1;
        }

        public static bool Deserialize(ArraySegment<byte> buffer, Packet packet)
        {
            Serializer.Open(NetworkSerializeMode.Deserialize, buffer);
            packet.OnSerialize(Serializer);
            return Serializer.Success && Serializer.Close() == buffer.Count;
        }
    }
}
