using ServerCore;
using System.Net;

namespace Server
{
    public class Program
    {
        static Listener listener = new();
        static byte[] buffer = new byte[1024];

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress addr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(addr, 7779);

            Func<Session> factory = () => new ClientSession();
            Func<int, long, int> verify = (rend, tick) => (rend - (int)(tick % 10) * 907) % int.MaxValue;

            listener.Listen(endPoint, factory, verify, OnAccepted);
            Console.WriteLine("Listen Start");

            while (true)
            {
            }
        }

        public static void OnAccepted(Session session)
        {

        }
    }
}
