using System.Net;
using System.Net.Sockets;

namespace Donet
{
    public interface IConnector
    {
        public delegate void ConnectCallback(Socket client);
        public abstract bool ConnectAsync(EndPoint endpoint, SocketType socketType, ProtocolType protocolType, ConnectCallback callback);
        public abstract void Close();
    }
}
