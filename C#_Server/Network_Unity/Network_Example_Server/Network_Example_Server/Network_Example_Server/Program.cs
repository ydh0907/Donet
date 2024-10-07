using Donet;
using Network_Example_Server.Packets;
using System.Net;

namespace Network_Example_Server
{
    internal class Program
    {
        private static Listener listener = new();

        public static Dictionary<ulong, ClientSession> clients = new();
        private static ulong lastID = 0;
        private static ulong nextID => lastID++;

        public static object locker = new object();

        private static void Main(string[] args)
        {
            if (PacketFactory.InitializePacket<PacketEnum>())
                return;

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3001);

            listener.Listen(endPoint, () => new ClientSession(), HandleClientConnect);
        }

        public static void Broadcast(Packet packet, ClientSession sender = null)
        {
            ArraySegment<byte> buffer = SendBuffer.UniqueBuffer.Open(1024);
            int size = packet.Serialize(buffer);
            buffer = SendBuffer.UniqueBuffer.Close(size);

            foreach (var client in clients.Values)
            {
                if (client == sender)
                    continue;
                client.Send(buffer);
            }
        }

        private static void HandleClientConnect(Session session)
        {
            ClientSession client = session as ClientSession;
            clients.Add(nextID, client);
        }
    }
}
