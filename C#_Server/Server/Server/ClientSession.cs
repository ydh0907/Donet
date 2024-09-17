using Donet;
using System.Net;

namespace Server
{
    internal class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Program.clients.Remove(this);
        }

        public override void OnPacketReceived(Packet packet)
        {

        }

        public override void OnSend(int transferred)
        {
        }
    }
}
