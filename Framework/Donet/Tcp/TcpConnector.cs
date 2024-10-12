using System;
using System.Net;
using System.Net.Sockets;

namespace Donet.Tcp
{
    public class TcpConnector
    {
        private Func<TcpSession> factory;
        private Action<TcpSession> callback;

        public void Connect(IPEndPoint endPoint, Func<TcpSession> factory, Action<TcpSession> callback)
        {
            this.factory = factory;
            this.callback = callback;

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = endPoint;

            RegisterConnect(args);
        }

        private void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = new Socket(args.RemoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            bool pending = socket.ConnectAsync(args);
            if (!pending)
            {
                OnConnectCompleted(null, args);
            }
        }

        private void OnConnectCompleted(object? sender, SocketAsyncEventArgs args)
        {
            TcpSession session = factory();
            session.Initialize(args.ConnectSocket);
            callback?.Invoke(session);
        }
    }
}
