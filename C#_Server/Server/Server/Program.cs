using Donet;
using Server.Packets;
using System.Net;

namespace Server
{
    public class Program
    {
        private static Listener listener = new();
        private static byte[] buffer = new byte[1024];
        private static List<PacketSession> clients = new List<PacketSession>();
        private static object locker = new object();

        private static void Main(string[] args)
        {
            if (!PacketFactory.InitializePacket<PacketTypeEnum>())
                return;

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress addr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(addr, 7779);

            Func<Session> factory = () => new ClientSession();
            Func<int, long, int> verify = (rend, tick) => (rend - (int)(tick % 10) * 907) % int.MaxValue;

            listener.Listen(endPoint, factory, verify, OnConnected);
            Console.WriteLine("Listen Start");

            while (true)
            {
                string s = Console.ReadLine();
                if (s == "S")
                    PrintServerRpc();
                else if (s == "C")
                    PrintClientRpc();
            }
        }

        public static void OnConnected(Session session)
        {
            lock (locker)
                clients.Add(session as PacketSession);
        }

        public static void OnDisconnected(PacketSession session)
        {
            lock (locker)
                clients.Remove(session);
        }

        public static void Broadcast(Packet packet)
        {
            foreach (PacketSession session in clients)
            {
                session.SendPacket(packet);
            }
        }

        public static void PrintServerRpc()
        {
            Print();
        }

        public static void PrintClientRpc()
        {
            ClientRpcPacket packet = new ClientRpcPacket();
            Broadcast(packet);
        }

        public static void Print()
        {
            Console.WriteLine("나는 RPC");
        }
    }
}
