namespace Donet
{
    public interface ISerializablePacket
    {
        public void OnReceived(ISession session);
        public void OnSerialize(Serializer serializer); // use serializer for easy serialize
        public ISerializablePacket CreatePacket();
    }
}
