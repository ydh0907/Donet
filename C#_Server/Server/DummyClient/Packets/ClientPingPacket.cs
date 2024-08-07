using ServerCore;

namespace Server.Packets
{
    internal class ClientPingPacket : Packet
    {
        public int hash;

        public override void OnReceived(Session session)
        {
            //ClientSession client = (ClientSession)session;
            //ServerPingPacket packet = new ServerPingPacket();
            //packet.hash = hash;
            //client.SendPacket(packet);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref hash);
        }
    }
}
