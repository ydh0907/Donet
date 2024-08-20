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
            Start();

            while (session == null) { }

            PingPacket pingPacket = new PingPacket();
            pingPacket.hash = 100;
            session.SendPacket(pingPacket);

            while (true)
            {
                string s = Console.ReadLine();
                SayPacket sayPacket = new SayPacket();
                sayPacket.message = s;

                session.SendPacket(sayPacket);
            }
        }

        public static void Start()
        {
            if (!PacketFactory.InitializePacket<PacketTypeEnum>())
                return;

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress addr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(addr, 7779);

            Func<Session> factory = () => new ServerSession();
            Func<int, long, int> verify = (rend, tick) => (rend - (int)(tick % 10) * 907) % int.MaxValue;

            Connector connector = new();
            connector.Connect(endPoint, factory, verify, OnConnected);
        }

        public static void OnConnected(Session session)
        {
            Console.WriteLine("Connected!");
            ServerSession server = session as ServerSession;
            Program.session = server;
        }
    }
}
