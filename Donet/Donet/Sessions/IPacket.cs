using Donet.Utils;

namespace Donet.Sessions
{
    public interface IPacket : INetworkSerializable
    {
        public void OnReceived(Session session);
    }
}
