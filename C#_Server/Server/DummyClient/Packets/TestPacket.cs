using Donet;

namespace DummyClient.Packets
{
    internal class TestPacket : Packet
    {
        public string message;

        public override void OnReceived(Session session)
        {
            Console.WriteLine(message);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref message);
        }
    }
}
