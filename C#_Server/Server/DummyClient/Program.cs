using ServerCore;
using System.Net;

namespace DummyClient
{
    internal class Program
    {
        public class ServerSession : Session
        {
            public override void OnConnected(EndPoint endPoint)
            {
                Console.WriteLine($"[SESSION] Connected On :{endPoint}");
            }

            public override void OnDisconnected(EndPoint endPoint)
            {
                Console.WriteLine($"[SESSION] Disconnected By :{endPoint}");
            }

            public override void OnReceive(ArraySegment<byte> buffer)
            {
                Console.WriteLine($"[SESSION] Received : {buffer.Count}B");
            }

            public override void OnSend(int transferred)
            {
                Console.WriteLine($"[SESSION] Transferred : {transferred}B");
            }
        }

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress addr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(addr, 7779);

            Connector<ServerSession> connector = new Connector<ServerSession>((rend, tick) => (rend - (int)(tick % 10) * 907) % int.MaxValue);
            connector.Connect(endPoint);

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
