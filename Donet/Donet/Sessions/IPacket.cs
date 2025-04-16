using Donet.Utils;

namespace Donet.Sessions
{
    public interface IPacket : INetworkSerializable
    {
        public IPacket CreateInstance();
        public void OnReceived(Session session);
    }
}
