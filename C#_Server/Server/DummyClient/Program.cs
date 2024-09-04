using Donet;
using DummyClient.Packets;
using System.Net;

namespace DummyClient
{
    public class Program
    {
        public static object locker = new();
        private static int connected = 0;
        private static ServerSession session;

        public static int perSec = 0;

        public static void Main(string[] args)
        {
            Initialize();

            while (true)
            {
                string message = Console.ReadLine();
                TestPacket packet = new TestPacket();
                packet.message = message;
                session.SendPacket(packet);
            }
        }

        public static void Initialize()
        {
            if (!PacketFactory.InitializePacket<PacketTypeEnum>())
                return;

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress addr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(addr, 7779);

            Func<Session> factory = () => new ServerSession();

            Connector connector = new();
            connector.Connect(endPoint, factory, OnConnected);
        }

        public static void OnConnected(Session session)
        {
            Console.WriteLine("Connected!");
            ServerSession server = session as ServerSession;
            Program.session = server;
        }
    }
}
