namespace Donet.Utils
{
    public struct PacketHeader : INetworkSerializable
    {
        public ushort size;
        public ushort type;

        public bool Serialize(Serializer serializer)
        {
            return serializer.Serialize(ref size) && serializer.Serialize(ref type);
        }
    }
}
