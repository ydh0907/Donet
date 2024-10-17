using System;
using System.Net;
using System.Net.Sockets;

namespace Donet.TCP
{
    public class Connector
    {
        private Func<Session> factory;
        private Action<Session> callback;

        public void Connect(IPEndPoint endPoint, Func<Session> factory, Action<Session> callback)
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
            Session session = factory();
            session.Initialize(args.ConnectSocket);
            callback?.Invoke(session);
        }
    }
}
