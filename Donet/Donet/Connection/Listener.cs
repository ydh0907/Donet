using System.Net;
using System.Net.Sockets;

using Donet.Utils;

namespace Donet.Connection
{
    public delegate void AcceptHandle(Socket socket);

    public class Listener
    {
        private readonly Socket socket;
        private readonly AcceptHandle handler;

        public Listener(Socket socket, AcceptHandle handler)
        {
            this.socket = socket;
            this.handler = handler;
        }

        public void Listen(int backlog, IPEndPoint endPoint)
        {
            socket.Bind(endPoint);
            socket.Listen(backlog);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += HandleAccept;
            Accept(args);
        }

        private void Accept(SocketAsyncEventArgs args)
        {
            bool pending = socket.AcceptAsync(args);
            if (!pending)
                HandleAccept(socket, args);
        }

        private void HandleAccept(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
                handler?.Invoke(args.AcceptSocket);
            else
                Logger.Log(LogLevel.Warning, "[Listener] Accept failed please check connector.");

            args = new SocketAsyncEventArgs();
            args.Completed += HandleAccept;
            Accept(args);
        }
    }
}
