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
            MemoryPool.sendBufferSize = 1 << 18;
            MemoryPool.receiveBufferSize = 1 << 20;
            MemoryPool.Initialize();
            PacketFactory.Initialize(new ClientChatPacket());

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
                string message = Console.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {
                    ClientChatPacket packet = new ClientChatPacket();
                    packet.message = message;
                    session.Send(packet);
                }
            }

            PacketFactory.Dispose();
            MemoryPool.Dispose();
            Logger.Dispose();
        }
    }
}
