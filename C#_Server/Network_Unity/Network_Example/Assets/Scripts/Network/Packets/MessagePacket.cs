using Donet;

namespace Network_Example_Server.Packets
{
    public class MessagePacket : Packet
    {
        public string message;

        public override void OnReceived(Session session)
        {
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref message);
        }
    }
}
