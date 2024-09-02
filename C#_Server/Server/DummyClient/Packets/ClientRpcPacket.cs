using Donet;

namespace DummyClient.Packets
{
    internal class ClientRpcPacket : Packet
    {
        public int sender;
        public int receiver;

        public override void OnReceived(Session session)
        {
            Console.WriteLine(sender);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref sender);
            serializer.SerializeValue(ref receiver);
        }
    }
}
