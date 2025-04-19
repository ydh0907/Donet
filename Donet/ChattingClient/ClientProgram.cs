using System.Net;
using System.Net.Sockets;

using Donet.Connection;
using Donet.Sessions;
using Donet.Utils;

namespace ChattingClient
{
    internal class ClientProgram
    {
        static void Main(string[] args)
        {
            Logger.Initialize();
            MemoryPool.Initialize();
            PacketFactory.Initialize(new ClientChatPacket());

            List<Task> clients = new List<Task>();

            for (int i = 0; i < 100; i++)
            {
                Task t = Task.Run(StartClient);
                clients.Add(t);
            }

            Task.WaitAll(clients.ToArray());

            PacketFactory.Dispose();
            MemoryPool.Dispose();
            Logger.Dispose();

            Console.ReadKey();
        }

        private static void StartClient()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9977);
            Socket socket = Connector.Connect(endPoint);

            Logger.Log(LogLevel.Notify, "[Client] connected to server.");

            bool active = true;
            Session session = new Session();
            session.Initialize(0, socket, (session) =>
            {
                active = false;
                Logger.Log(LogLevel.Notify, "[Client] disconnected from server.");
            });

            while (active)
            {
                Thread.Sleep(20);
                string message = Random.Shared.Next().ToString();
                if (!active)
                    break;
                ClientChatPacket packet = new ClientChatPacket();
                packet.message = message;
                session.Send(packet);
            }
        }
    }
}
