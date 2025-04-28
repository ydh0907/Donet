using System.Net;
using System.Net.Sockets;

using Donet.Connection;
using Donet.Sessions;
using Donet.Utils;

namespace TestServer
{
    internal class ServerProgram
    {
        private static volatile int id = 0;

        private static ulong received = 0;

        struct TestPacket : IPacket
        {
            public int info;

            public IPacket CreateInstance()
            {
                return new TestPacket();
            }
            public void OnReceived(Session session)
            {
                Interlocked.Increment(ref received);
                session.Send(this);
            }
            public void Serialize(Serializer serializer)
            {
                serializer.Serialize(ref info);
            }
        }

        static void Main(string[] args)
        {
            Logger.Initialize();
            MemoryPool.Initialize();
            PacketFactory.Initialize(new TestPacket());

            ThreadPool.SetMaxThreads(24, 24);
            ThreadPool.SetMinThreads(16, 16);

            for (int i = 0; i < 3; i++)
                StartServer(i);

            while (true)
            {
                Thread.Sleep(1000);
                Console.WriteLine(received);
                Interlocked.Exchange(ref received, 0);
                Console.WriteLine(id);
            }
        }

        private static void StartServer(int i)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 9977 + i);
            Listener listener = new Listener(socket, endPoint, HandleAccept);
            listener.Listen(1000);
        }

        static void HandleAccept(Socket socket)
        {
            Session session = new Session();
            session.Initialize(Interlocked.Increment(ref id), socket);
            session.Send(new TestPacket() { info = Random.Shared.Next() });
        }
    }
}
