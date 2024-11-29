namespace Donet.RUDP
{
    public struct Header : INetworkSerializable
    {
        private byte[] header;

        public void Serialize(Serializer serializer)
        {

        }

        public Header(HeaderType type = HeaderType.Null)
        {
            header = new byte[0];
        }
    }
}
