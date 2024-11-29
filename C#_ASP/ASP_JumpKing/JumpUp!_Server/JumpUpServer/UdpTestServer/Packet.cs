namespace UdpTestServer
{
    public abstract class Packet
    {
        public abstract int Serialize(byte[] buffer, Player player);
        public abstract int Deserialize(byte[] buffer, Player player);
    }
}
