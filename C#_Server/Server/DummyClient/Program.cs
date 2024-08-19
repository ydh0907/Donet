using Server.Packets;
using ServerCore;
using System.Net;

namespace DummyClient
{
    public class Program
    {
        public static object locker = new();
        private static int connected = 0;
        private static List<ServerSession> sessions = new List<ServerSession>();

        public static int perSec = 0;

        public static void Main(string[] args)
        {
            int count = 1100;
            for (int i = 0; i < count; i++)
            {
                Start();
            }
            while (sessions.Count != count) { }
            Console.WriteLine($"Connected : {connected} Clients");

            for (int i = 0; i < sessions.Count; i++)
            {
                ClientPingPacket packet = new ClientPingPacket();
                packet.hash = sessions[i].GetHashCode();
                sessions[i].SendPacket(packet);
            }

            while (true)
            {
                Thread.Sleep(1000);
                lock (locker)
                {
                    Console.WriteLine(perSec);
                    perSec = 0;
                }
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
            ServerSession pSessiont = session as ServerSession;
            lock (locker)
            {
                connected++;
                sessions.Add(pSessiont);
            }
        }
    }
}
