using Server.Packets;
using ServerCore;
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
                Console.ReadKey();
                Console.WriteLine($"Connected : {clients.Count} Clients");
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
    }
}
