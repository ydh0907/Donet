using Donet;
using System.Net;

namespace Server
{
    internal class Program
    {
        public static List<ClientSession> clients = new List<ClientSession>();
        public static object locker = new object();

        static void Main(string[] args)
        {
            if (!PacketFactory.InitializePacket<PacketEnum>())
                return;

            Listener listener = new Listener();

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 3000);
            Func<Session> factory = () => new ClientSession();

            listener.Listen(endPoint, factory, OnClientConnected);

            while (true)
            {
                Console.ReadKey();
                string message;
                lock (locker)
                {
                    Console.WriteLine("Server : ");
                    message = Console.ReadLine();
                }
                MessagePacket packet = new MessagePacket();
                packet.message = $"Server : {message}";
                foreach (ClientSession session in clients)
                {
                    session.SendPacket(packet);
                }
            }
        }

        public static void Broadcast(Packet packet, ClientSession sender = null)
        {
            foreach (ClientSession session in clients)
            {
                if (session == sender)
                    continue;
                session.SendPacket(packet);
            }
        }

        static void OnClientConnected(Session session)
        {
            clients.Add(session as ClientSession);
        }
    }
}
