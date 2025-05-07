using Donet.Utils;

namespace Donet.Sessions
{
    public interface IPacket : INetworkSerializable
    {
        public IPacket Create();
        public void OnReceived(Session session);
    }
}
