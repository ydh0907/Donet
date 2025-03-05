using System.Net;
using System.Net.Sockets;

namespace Donet
{
    public interface IListener
    {
        public delegate void AcceptCallback(Socket client);
        public Socket Socket { get; }
        public abstract void Listen(EndPoint endPoint, SocketType socketType, ProtocolType protocolType, AcceptCallback callback);
        public abstract void Close();
    }
}
