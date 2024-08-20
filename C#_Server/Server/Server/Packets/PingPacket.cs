using Donet;

namespace Server.Packets
{
    public class PingPacket : Packet
    {
        public int hash;

        public override void OnReceived(Session session)
        {
            Console.WriteLine("Ping : " + hash);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref hash);
        }
    }
}
