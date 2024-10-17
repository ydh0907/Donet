using System;
using System.Net;
using System.Net.Sockets;

namespace Donet.TCP
{
    public class Listener
    {
        private Socket listener;
        private Func<Session> factory;
        private Action<Session> callback;

        public void Listen(IPEndPoint endPoint, Func<Session> factory, Action<Session> callback)
        {
            this.factory = factory;
            this.callback = callback;

            listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endPoint);
            listener.Listen(10);

            for (int i = 0; i < 3; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += AcceptHandle;

                RegisterAccept(args);
            }
        }

        private void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending = listener.AcceptAsync(args);
            if (!pending)
            {
                AcceptHandle(null, args);
            }
        }

        private void AcceptHandle(object? sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                InitializeSession(args.AcceptSocket);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            RegisterAccept(args);
        }

        private void InitializeSession(Socket client)
        {
            Session session = factory();
            session.Initialize(client);
            callback?.Invoke(session);
        }
    }
}
