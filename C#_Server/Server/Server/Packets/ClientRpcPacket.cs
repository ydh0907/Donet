using Donet;

namespace Server.Packets
{
    internal class ClientRpcPacket : Packet
    {
        public int id;
        public string name;

        public override void OnReceived(Session session)
        {
            Program.Broadcast(this);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref id);
            serializer.SerializeValue(ref name);
        }
    }
}
