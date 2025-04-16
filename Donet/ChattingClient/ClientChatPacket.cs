using Donet.Sessions;
using Donet.Utils;

namespace ChattingClient
{
    public struct ClientChatPacket : IPacket
    {
        public string message;

        public IPacket CreateInstance()
        {
            return new ClientChatPacket();
        }

        public void OnReceived(Session session)
        {
            Console.WriteLine(message);
        }

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref message);
        }
    }
}
