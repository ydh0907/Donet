using System;
using System.Net;
using System.Net.Sockets;

namespace Donet
{
    public interface ISession
    {
        public Socket Socket { get; }
        public bool Connected { get; }
        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnReceive(ArraySegment<byte> buffer);
        public abstract void OnSend(int transferred);
        public abstract void OnDisconnected(EndPoint endPoint);
    }
}
