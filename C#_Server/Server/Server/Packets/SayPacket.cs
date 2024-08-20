using Donet;

namespace Server.Packets
{
    public class SayPacket : Packet
    {
        public string message;

        public override void OnReceived(Session session)
        {
            Console.WriteLine("Client : " + message);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref message);
        }
    }
}
