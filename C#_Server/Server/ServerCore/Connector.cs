using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Connector
    {
        private Func<Session> factory;
        private Func<int, long, int> verify;
        private Action<Session> callback;

        public void Connect(IPEndPoint endPoint, Func<Session> factory, Func<int, long, int> verify, Action<Session> callback)
        {
            this.factory = factory;
            this.verify = verify;
            this.callback = callback;

            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs args = new();
            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = endPoint;
            args.UserToken = socket;

            RegisterConnect(args);
        }

        private void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;
            if (socket == null)
                return;

            bool pending = socket.ConnectAsync(args);
            if (!pending)
            {
                OnConnectCompleted(null, args);
            }
        }

        private void OnConnectCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Verification(args.ConnectSocket);
            }
            else
            {

            }
        }

        private async void Verification(Socket server)
        {
            //byte[] buffer = new byte[sizeof(int) + sizeof(long)];
            //int length = await server.ReceiveAsync(buffer);

            //int rend = BitConverter.ToInt32(buffer, 0);
            //long tick = BitConverter.ToInt64(buffer, sizeof(int));

            //int value = verify(rend, tick);

            //await server.SendAsync(BitConverter.GetBytes(value));
            //await server.ReceiveAsync(buffer);

            //bool accept = BitConverter.ToBoolean(buffer, 0);

            if (true)//accept)
            {
                Session session = factory();
                session.Initialize(server);
                callback(session);
            }
            else
            {
                server.Shutdown(SocketShutdown.Both);
                server.Close();
            }
        }
    }
}
