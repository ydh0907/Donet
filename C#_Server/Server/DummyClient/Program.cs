using ServerCore;
using System.Net;

namespace DummyClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress addr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(addr, 7779);

            Func<Session> factory = () => new ServerSession();
            Func<int, long, int> verify = (rend, tick) => (rend - (int)(tick % 10) * 907) % int.MaxValue;

            Connector connector = new();
            connector.Connect(endPoint, factory, verify, OnConnected);

            while (true)
            {
                Thread.Sleep(100);
            }
        }

        public static void OnConnected(Session session)
        {

        }
    }
}
