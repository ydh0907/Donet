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

            while (true)
            {
                while (true)
                {
                    string s = Console.ReadLine();
                    if (s == "S")
                        PrintServerRpc();
                    else if (s == "C")
                        PrintClientRpc();
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
            Console.WriteLine("Connected!");
            ServerSession server = session as ServerSession;
            Program.session = server;
        }

        public static void PrintServerRpc()
        {
            ServerRpcPacket packet = new ServerRpcPacket();
            session.SendPacket(packet);
        }

        public static void PrintClientRpc()
        {
            ClientRpcPacket packet = new ClientRpcPacket();
            session.SendPacket(packet);
        }

        public static void Print()
        {
            Console.WriteLine("나는 RPC");
        }
    }
}
