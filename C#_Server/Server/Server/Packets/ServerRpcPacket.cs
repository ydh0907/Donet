using Donet;

namespace Server.Packets
{
    internal class ServerRpcPacket : Packet
    {
        public override void OnReceived(Session session)
        {
            Program.Print();
        }

        public override void OnSerialize(Serializer serializer)
        {
        }
    }
}
