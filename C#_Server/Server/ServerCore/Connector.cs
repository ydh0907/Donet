using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Connector<T> where T : Session, new()
    {
        private Func<int, long, int> verify;

        public Connector(Func<int, long, int> verify)
        {
            this.verify = verify;
        }

        public void Connect(IPEndPoint endPoint)
        {
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

            args.UserToken = typeof(T);

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
            byte[] buffer = new byte[sizeof(int) + sizeof(long)];
            int length = await server.ReceiveAsync(buffer);

            int rend = BitConverter.ToInt32(buffer, 0);
            long tick = BitConverter.ToInt64(buffer, sizeof(int));

            int value = verify(rend, tick);

            await server.SendAsync(BitConverter.GetBytes(value));
            await server.ReceiveAsync(buffer);

            bool accept = BitConverter.ToBoolean(buffer, 0);

            if (accept)
            {
                T session = new T();
                session.Init(server);
            }
            else
            {
                Console.WriteLine("disconnect by wrong verify");
            }
        }
    }
}
