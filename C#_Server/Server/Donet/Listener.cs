using System;
using System.Net;
using System.Net.Sockets;

namespace Donet
{
    public class Listener
    {
        private Socket listener;
        private Func<Session> factory;
        private Func<int, long, int> verify;
        private Action<Session> callback;

        public void Listen(IPEndPoint endPoint, Func<Session> factory, Func<int, long, int> verify, Action<Session> callback)
        {
            this.factory = factory;
            this.verify = verify;
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
                Verification(args.AcceptSocket);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            RegisterAccept(args);
        }

        private async void Verification(Socket client)
        {
            if (true)
            {
                Session session = factory();
                session.Initialize(client);
                session.Initialize(client);
                callback?.Invoke(session);
            }
            else
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }
    }
}
