
using System.Net;
using System.Net.Sockets;

using Donet.Connection;
using Donet.Sessions;
using Donet.Utils;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ThreadPool.SetMaxThreads(8192, 8192);
            ThreadPool.SetMinThreads(4096, 8192);

            Logger.Initialize();
            MemoryPool.Initialize();
            PacketFactory.Initialize();

            for (int i = 0; i < 3; i++)
                StartListener(9977 + i);

            try
            {
                while (true)
                {

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                PacketFactory.Dispose();
                MemoryPool.Dispose();
                Logger.Dispose();
            }
        }

        private static void StartListener(int port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            Listener listener = new Listener(socket, endPoint, HandleAccept);
            listener.Listen(512);
        }

        private static void HandleAccept(Socket socket)
        {

        }
    }
}
