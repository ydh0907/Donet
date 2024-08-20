using Donet;

namespace DummyClient.Packets
{
    public class PingPacket : Packet
    {
        public int hash;

        public override void OnReceived(Session session)
        {

        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref hash);
        }
    }
}
