using System.Net;

namespace UdpTestServer
{
    public class Player
    {
        public Player(IPEndPoint endPoint) => this.remote = endPoint;

        public int id;
        public float x;
        public float y;
        public readonly IPEndPoint remote;
        public int lastTick;
    }
}
