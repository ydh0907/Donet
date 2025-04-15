using Donet.Sessions;

namespace Donet.Utils
{
    public interface IPacket : INetworkSerializable
    {
        public IPacket CreateInstance();
        public void OnReceived(Session session);
    }
}
