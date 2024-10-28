namespace Donet.RUDP
{
    public interface Packet : Donet.Packet
    {
        public HeaderType Type { get; set; }
    }
}
