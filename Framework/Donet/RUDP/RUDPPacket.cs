namespace Donet.RUDP
{
    public interface RUDPPacket : Packet
    {
        public HeaderType Type { get; set; }
    }
}
