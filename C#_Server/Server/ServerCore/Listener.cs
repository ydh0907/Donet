using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener<T> where T : Session, new()
    {
        private Socket listener;
        private Func<int, long, int> verify;

        public void Init(IPEndPoint endPoint, Func<int, long, int> verify)
        {
            this.verify = verify;

            SocketAsyncEventArgs args = new();
            args.Completed += AcceptHandle;

            listener = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endPoint);
            listener.Listen(10);

            RegisterAccept(args);
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
            int rend = Random.Shared.Next(int.MaxValue);

            DateTime time = DateTime.Now;
            long tick = time.Ticks;

            byte[] buffer = new byte[sizeof(int) + sizeof(long)];
            Array.Copy(BitConverter.GetBytes(rend), 0, buffer, 0, sizeof(int));
            Array.Copy(BitConverter.GetBytes(tick), 0, buffer, sizeof(int), sizeof(long));

            await client.SendAsync(buffer);
            int length = await client.ReceiveAsync(buffer);

            int clientValue = BitConverter.ToInt32(buffer, 0);
            int serverValue = verify(rend, tick);

            await client.SendAsync(BitConverter.GetBytes(clientValue == serverValue));

            if (clientValue == serverValue)
            {
                Console.WriteLine($"verify : {serverValue}");
                T session = new T();
                session.Init(client);
            }
            else
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }

        public Socket Accept()
        {
            return listener.Accept();
        }
    }
}
