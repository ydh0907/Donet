using Donet;

namespace Server.Packets
{
    internal class TestPacket : Packet
    {
        public string message;

        public override void OnReceived(Session session)
        {
            Console.WriteLine(message);
            Program.Broadcast(this, session as PacketSession);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref message);
        }
    }
}
