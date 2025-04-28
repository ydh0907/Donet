using System.Net;
using System.Net.Sockets;

using Donet.Connection;
using Donet.Sessions;
using Donet.Utils;

namespace TestClient
{
    internal class ClientProgram
    {
        struct TestPacket : IPacket
        {
            public int info;

            public IPacket CreateInstance()
            {
                return new TestPacket();
            }
            public void OnReceived(Session session)
            {
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

            StartClient();

            while (true) ;
        }

        private static void StartClient()
        {
            Task.Run(async () =>
            {
                for (int i = 0; i < 3000; i++)
                {
                    IPEndPoint endPoint = new(IPAddress.Loopback, 9977 + Random.Shared.Next() % 3);
                    Socket socket = await Connector.ConnectAsync(endPoint);
                    Session session = new Session();
                    session.Initialize(0, socket);
                }
            });
        }
    }
}
