using System.Net;
using System.Net.Sockets;

namespace UdpTestServer
{
    public enum PacketID
    {
        Heartbeat,
        Movement,
    }

    internal class Program
    {
        static Socket socket;
        static object locker = new object();

        public static List<Player> players = new List<Player>();

        static int nextID = 1;

        static void Main(string[] args)
        {
            IPEndPoint bindPoint = new IPEndPoint(IPAddress.Any, 9907);
            socket = new Socket(bindPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(bindPoint);

            Console.WriteLine("Server Started");
            Receive();
            FlushCycle();
        }

        private static void FlushCycle()
        {
            Console.WriteLine("Cycle Started");
            int flushTick = 20;
            int lastTick = 0;
            int currentTime;

            while (true)
            {
                currentTime = Environment.TickCount;
                if (currentTime - lastTick > flushTick)
                {
                    foreach (var player in players)
                    {
                        Broadcast(new MovePacket(), player);
                        if (Environment.TickCount - player.lastTick > 5000)
                            players.Remove(player);
                    }
                    lastTick = currentTime;
                }
            }
        }

        private static void Broadcast(Packet packet, Player sender, bool sendToSelf = false)
        {
            byte[] buffer = new byte[1024];
            int offset = packet.Serialize(buffer, sender);
            foreach (var other in players)
            {
                if (other == sender && !sendToSelf)
                    continue;
                socket.SendTo(buffer, 0, offset, SocketFlags.None, other.remote);
            }
        }

        private async static void Receive()
        {
            Console.WriteLine("Receive Started");
            byte[] buffer = new byte[1024];
            IPEndPoint endPoint = new IPEndPoint(IPAddress.None, 0);

            while (true)
            {
                await socket.ReceiveFromAsync(new ArraySegment<byte>(buffer, 0, 1024), endPoint);
                int remoteID = BitConverter.ToInt32(buffer, 0);

                if (remoteID == 0)
                    HandleConnect(endPoint);
                else
                {
                    Player player = players.Find(x => x.id == remoteID);
                    if (player != null)
                    {
                        PacketID type = (PacketID)BitConverter.ToInt32(buffer, 4);
                        HandlePacket(player, buffer, type);
                    }
                }
            }
        }

        private static void HandleConnect(IPEndPoint endPoint)
        {
            Player player = new Player(endPoint);
            player.id = nextID++;
            player.lastTick = Environment.TickCount;
            players.Add(player);

            HeartbeatPacket packet = new HeartbeatPacket();
            byte[] buffer = new byte[1024];
            int offset = packet.Serialize(buffer, player);
            socket.SendTo(buffer, 0, offset, SocketFlags.None, player.remote);

            Console.WriteLine("Player Connected : " + player.id);
        }

        private static void HandlePacket(Player player, byte[] buffer, PacketID type)
        {
            switch (type)
            {
                case PacketID.Heartbeat:
                    player.lastTick = Environment.TickCount;
                    break;
                case PacketID.Movement:
                    MovePacket MovePacket = new MovePacket();
                    MovePacket.Deserialize(buffer, player);
                    break;
            }
        }
    }
}
