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
        private static ReaderWriterLock rwLock = new ReaderWriterLock();

        private static void Main(string[] args)
        {
            Initialize();

            while (true)
            {
                string message = Console.ReadLine();
                TestPacket packet = new TestPacket();
                packet.message = message;
                Broadcast(packet);
            }
        }

        private static void Initialize()
        {
            if (!PacketFactory.InitializePacket<PacketTypeEnum>())
                return;

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress addr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(addr, 7779);

            Func<Session> factory = () => new ClientSession();

            listener.Listen(endPoint, factory, OnConnected);
            Console.WriteLine("Listening...");
        }

        public static void OnConnected(Session session)
        {
            rwLock.AcquireWriterLock(10);
            clients.Add(session as PacketSession);
            rwLock.ReleaseWriterLock();
        }

        public static void OnDisconnected(PacketSession session)
        {
            rwLock.AcquireWriterLock(10);
            clients.Remove(session);
            rwLock.ReleaseWriterLock();
        }

        public static void Broadcast(Packet packet, PacketSession ignore = null)
        {
            rwLock.AcquireReaderLock(10);
            foreach (PacketSession session in clients)
            {
                if (session == ignore)
                    continue;
                session.SendPacket(packet);
            }
            rwLock.ReleaseReaderLock();
        }
    }
}
