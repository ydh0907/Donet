namespace UdpTestServer
{
    public class MovePacket : Packet
    {
        public static int Size = 16;

        public override int Serialize(byte[] buffer, Player player)
        {
            int offset = 0;
            Array.Copy(BitConverter.GetBytes(player.id), 0, buffer, offset, 4);
            offset += 4;
            Array.Copy(BitConverter.GetBytes((int)PacketID.Movement), 0, buffer, offset, 4);
            offset += 4;
            Array.Copy(BitConverter.GetBytes(player.x), 0, buffer, offset, 4);
            offset += 4;
            Array.Copy(BitConverter.GetBytes(player.y), 0, buffer, offset, 4);
            offset += 4;
            return offset;
        }

        public override int Deserialize(byte[] buffer, Player player)
        {
            int offset = 8;
            player.x = BitConverter.ToInt32(buffer, offset);
            offset += 4;
            player.y = BitConverter.ToInt32(buffer, offset);
            offset += 4;
            return offset;
        }
    }
}
