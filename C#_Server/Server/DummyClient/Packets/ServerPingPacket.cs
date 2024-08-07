using ServerCore;

namespace Server.Packets
{
    internal class ServerPingPacket : Packet
    {
        public int hash;

        public override void OnReceived(Session session)
        {
            if (session.GetHashCode() != hash)
                Console.WriteLine($"ERROR! : {session.GetHashCode()}, {hash}");
            else
                Console.WriteLine($"SUCCESS! : {session.GetHashCode()}");

            Thread.Sleep(20);

            ClientPingPacket packet = new ClientPingPacket();
            packet.hash = hash;
            (session as PacketSession).SendPacket(packet);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref hash);
        }
    }
}
