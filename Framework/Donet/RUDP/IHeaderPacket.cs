namespace Donet.RUDP
{
    public interface IHeaderPacket : ISerializablePacket
    {
        public Header Header { get; set; }
    }
}
