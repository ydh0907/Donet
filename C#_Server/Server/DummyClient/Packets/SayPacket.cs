using Donet;

namespace DummyClient.Packets
{
    public class SayPacket : Packet
    {
        public string message;

        public override void OnReceived(Session session)
        {
            Console.WriteLine("Server : " + message);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref message);
        }
    }
}
