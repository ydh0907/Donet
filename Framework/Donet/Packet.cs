namespace Donet
{
    public interface Packet
    {
        public void OnReceived(Session session);
        public void OnSerialize(Serializer serializer); // use serializer for easy serialize
    }
}
