using DummyClient;
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

            ClientPingPacket packet = new ClientPingPacket();
            packet.hash = hash;
            (session as PacketSession).SendPacket(packet);

            lock (Program.locker)
            {
                Program.perSec++;
            }
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref hash);
        }
    }
}
