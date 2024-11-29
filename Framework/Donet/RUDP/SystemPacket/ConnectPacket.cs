namespace Donet.RUDP.SystemPacket
{
    internal struct ConnectPacket : IHeaderPacket
    {
        private Header header;
        public Header Header
        {
            get => header;
            set => header = value;
        }

        public void OnReceived(ISession session)
        {
        }

        public void OnSerialize(Serializer serializer)
        {
        }
    }
}
