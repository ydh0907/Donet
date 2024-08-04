using ServerCore;
using System.Net;

namespace Server
{
    public class ClientSession : Session
    {
        private void Read<T>() where T : struct
        {

        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"[SESSION] Connected On : {endPoint}");
            try
            {
                ushort size = 1000;
                byte[] sbuffer = new byte[size * 4];
                int offset = 0;
                for (int i = 0; i < 4; i++, offset += 1000)
                {
                    Array.Copy(BitConverter.GetBytes(size), 0, sbuffer, offset, sizeof(ushort));
                }
                Send(sbuffer);
                Send(sbuffer);
                Send(sbuffer);
                Send(sbuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
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

    public class Program
    {
        static Listener<ClientSession> listener = new();
        static byte[] buffer = new byte[1024];

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress addr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(addr, 7779);

            listener.Init(endPoint, (rend, tick) => (rend - (int)(tick % 10) * 907) % int.MaxValue);
            Console.WriteLine("Listen Start");

            while (true)
            {
            }
        }
    }
}
