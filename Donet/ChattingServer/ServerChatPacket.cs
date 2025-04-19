using Donet.Sessions;
using Donet.Utils;

namespace ChattingServer
{
    public struct ServerChatPacket : IPacket
    {
        public string message;

        public IPacket CreateInstance()
        {
            return new ServerChatPacket();
        }

        public void OnReceived(Session session)
        {
            //message = $"[Client {session.Id}:{session.Socket.RemoteEndPoint}] {message}";
            //Logger.Log(LogLevel.Notify, message);
            //ServerProgram.Broadcast(this);
        }

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref message);
        }
    }
}
