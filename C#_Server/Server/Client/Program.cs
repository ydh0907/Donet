using Donet;
using System.Net;

namespace Client
{
    internal class Program
    {
        static ServerSession session = null;
        public static object locker = new object();

        static void Main(string[] args)
        {
            Console.Write("Enter Your Name : ");
            string name = Console.ReadLine();

            if (!PacketFactory.InitializePacket<PacketEnum>())
                return;

            Connector connector = new Connector();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("172.31.3.204"), 3000); // 127.0.0.1
            connector.Connect(endPoint, () => new ServerSession(), (server) => session = server as ServerSession);

            while (session == null) ;

            while (true)
            {
                Console.ReadKey();
                lock (locker)
                {
                    Console.Write($"{name} : ");
                    string message = Console.ReadLine();
                    MessagePacket packet = new MessagePacket();
                    packet.message = $"{name} : {message}";
                    session.SendPacket(packet);
                }
            }
        }
    }
}
