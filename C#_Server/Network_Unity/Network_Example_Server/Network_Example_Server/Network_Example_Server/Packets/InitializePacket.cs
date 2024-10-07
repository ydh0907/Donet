using Donet;

namespace Network_Example_Server.Packets
{
    public class InitializePacket : Packet
    {
        public string nickname;

        public override void OnReceived(Session session)
        {
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref nickname);
        }
    }
}
