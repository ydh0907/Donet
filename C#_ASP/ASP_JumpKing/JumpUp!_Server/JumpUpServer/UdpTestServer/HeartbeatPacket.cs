namespace UdpTestServer
{
    public class HeartbeatPacket : Packet
    {
        public override int Deserialize(byte[] buffer, Player player)
        {
            player.lastTick = Environment.TickCount;
            return 8;
        }

        public override int Serialize(byte[] buffer, Player player)
        {
            int offset = 0;
            Array.Copy(BitConverter.GetBytes(player.id), 0, buffer, offset, 4);
            offset += 4;
            Array.Copy(BitConverter.GetBytes((int)PacketID.Heartbeat), 0, buffer, offset, 4);
            return offset;
        }
    }
}
