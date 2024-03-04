using MyFramework;
using System.Net.Sockets;

namespace Server
{
    internal class ClientSession : Session
    {
        public ClientSession(int id, Socket socket) : base(id, socket)
        {
        }

        public override void OnReceived(ArraySegment<byte> buffer)
        {
            Console.WriteLine($"[Client Session] : Received {buffer.Count} of Byte from {Socket.RemoteEndPoint}");
        }

        public override void OnSend(ArraySegment<byte> buffer)
        {
            Console.WriteLine($"[Client Session] : Send {buffer.Count} of Byte to {Socket.RemoteEndPoint}");
        }

        protected override void OnCreated(Socket socket)
        {
            Console.WriteLine($"[Client Session] : Session Created on {socket.RemoteEndPoint}");
        }

        protected override void OnDestroyed(Socket socket)
        {
            Console.WriteLine($"[Client Session] : Session Destroyed on {socket.RemoteEndPoint}");
        }
    }
}
