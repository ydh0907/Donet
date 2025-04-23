using Donet.Sessions;
using Donet.Utils;

namespace Server.Packets
{
    public struct InitializePacket : IPacket
    {
        public ulong id;

        public IPacket CreateInstance()
        {
            return new InitializePacket();
        }

        public void OnReceived(Session session)
        {
        }

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref id);
        }
    }
}
