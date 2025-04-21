using System;
using System.Net;
using System.Net.Sockets;

using Donet.Utils;

namespace Donet.Connection
{
    public delegate void AcceptHandle(Socket socket);

    public class Listener
    {
        private readonly Socket socket;
        private readonly IPEndPoint endPoint;
        private readonly AcceptHandle handler;
        private readonly SocketAsyncEventArgs listenArgs;

        public Listener(Socket socket, IPEndPoint endPoint, AcceptHandle handler)
        {
            this.socket = socket;
            this.endPoint = endPoint;
            this.handler = handler;
            this.listenArgs = new SocketAsyncEventArgs();
            listenArgs.Completed += HandleAccept;
        }

        public void Listen(int backlog)
        {
            socket.Bind(endPoint);
            socket.Listen(backlog);

            Logger.Log(LogLevel.Notify, "[Listener] listener opened.");
            Accept(listenArgs);
        }

        private void Accept(SocketAsyncEventArgs args)
        {
            try
            {
                bool pending = socket.AcceptAsync(args);
                if (!pending)
                    HandleAccept(socket, args);
            }
            catch (ObjectDisposedException ex)
            {
                Logger.Log(LogLevel.Notify, "[Listener] listener closed.");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"[Listener] listen failed. {ex.Message}");
            }
        }

        private void HandleAccept(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
                handler?.Invoke(args.AcceptSocket);
            else
                Logger.Log(LogLevel.Warning, "[Listener] Accept failed please check connector.");

            args.AcceptSocket = null;
            Accept(args);
        }
    }
}
