using ServerCore;
using System.Net;

namespace Server
{
    public class ClientSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"[SESSION] Connected On : {endPoint}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"[Session] Disconnected On : {endPoint}");
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
}
