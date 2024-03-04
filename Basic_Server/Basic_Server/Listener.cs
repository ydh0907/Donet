using System;
using System.Net;
using System.Net.Sockets;

namespace MyFramework
{
    public class Listener
    {
        private Socket _listener;
        public Action<Socket> OnConnected;

        public Listener(int port)
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            _listener.Bind(endPoint);
        }

        public void Open()
        {
            _listener.Listen(10);

            Console.WriteLine($"[Listener] : Server Opened on {_listener.LocalEndPoint}");

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnAccepted;

            Accept(args);
        }

        public void Close()
        {
            _listener.Close();
        }

        private void OnAccepted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
            {
                Console.WriteLine($"[Listener] : {args.SocketError} Error Find");
            }

            Socket clientSocket = args.AcceptSocket;

            OnConnected?.Invoke(clientSocket);

            Console.WriteLine($"[Listener] : {clientSocket.RemoteEndPoint} is Connected");

            Accept(args);
        }

        private void Accept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending = _listener.AcceptAsync(args);

            if (!pending)
            {
                OnAccepted(_listener, args);
            }
        }
    }
}
