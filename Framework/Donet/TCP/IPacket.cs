namespace Donet.TCP
{
    public interface IPacket
    {
        public void OnReceived(Session session);
        public void OnSerialize(Serializer serializer); // use serializer for easy serialize
    }
}
