namespace Donet.RUDP
{
    public struct HeaderState : INetworkSerializable
    {
        public int header;
        public int length;

        public HeaderState(HeaderType type, int readbyte)
        {
            header = (int)type;
            length = readbyte;
        }

        public void Serialize(Serializer serializer)
        {
            serializer.SerializeValue(ref header);
            serializer.SerializeValue(ref length);
        }
    }
}
