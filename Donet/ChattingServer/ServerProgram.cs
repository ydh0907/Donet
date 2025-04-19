using System.Net;
using System.Net.Sockets;

using Donet.Connection;
using Donet.Sessions;
using Donet.Utils;

namespace ChattingServer
{
    internal class ServerProgram
    {
        private static Atomic<ulong> id = new Atomic<ulong>(0);

        private static Listener listener = null;
        private static Atomic<List<Session>> sessions = new Atomic<List<Session>>(new List<Session>());

        static void Main(string[] args)
        {
            Logger.Initialize();
            MemoryPool.Initialize();
            PacketFactory.Initialize(new ServerChatPacket());

            Task.Run(StartDebugTimer);
            StartServer();
            ConsoleOperAsync();

            PacketFactory.Dispose();
            MemoryPool.Dispose();
            Logger.Dispose();

            Console.ReadKey();
        }

        private static ulong lastReceiveCount = 0;
        private static void StartDebugTimer()
        {
            while (true)
            {
                Thread.Sleep(1000);
                ulong sum = 0;
                using (var local = sessions.Lock())
                    foreach (var session in local.Value)
                    {
                        using (var count = session.receiveCount.Lock())
                            sum += count.Value;
                    }
                Logger.Log(LogLevel.Notify, $"[Server] {sum - lastReceiveCount} packets per sec. total {sum} packet received.");
                lastReceiveCount = sum;
            }
        }

        public static void Close()
        {
            using var local = sessions.Lock();
            foreach (var session in local.Value)
            {
                session.Close();
            }
            Logger.Log(LogLevel.Notify, "[Server] server closed");
        }

        public static void Broadcast(IPacket packet)
        {
            using (var local = sessions.Lock())
                foreach (var session in local.Value)
                {
                    session.Send(packet);
                }
        }

        private static void ConsoleOperAsync()
        {
            bool active = true;
            while (active)
            {
                string oper = Console.ReadLine();
                if (!string.IsNullOrEmpty(oper))
                {
                    switch (oper)
                    {
                        case "exit":
                        case "Exit":
                            Close();
                            active = false;
                            break;
                        case "user":
                        case "User":
                        case "users":
                        case "Users":
                            using (var local = sessions.Lock())
                                foreach (var session in local.Value)
                                {
                                    Logger.Log(LogLevel.Notify, $"{session.Id} : {session.Socket.RemoteEndPoint}");
                                }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static void StartServer()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener = new Listener(socket, HandleAccept);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 9977);
            listener.Listen(4096, endPoint);

            Logger.Log(LogLevel.Notify, "[Server] server opened on 9977");
        }

        private static async void HandleAccept(Socket socket)
        {
            ulong next = 0;
            using (var local = id.Lock())
            {
                local.Set(local.Value + 1);
                next = local.Value;
            }
            Session session = new Session();
            await Task.Run(() => session.Initialize(next, socket, HandleDisconnect));
            using (var local = sessions.Lock())
                local.Value.Add(session);

            //ServerChatPacket packet = new ServerChatPacket();
            //packet.message = $"[Enter] {session.Id} : {session.Socket.RemoteEndPoint}";
            //Broadcast(packet);

            //Logger.Log(LogLevel.Notify, packet.message);
        }

        private static void HandleDisconnect(Session session)
        {
            using (var local = sessions.Lock())
                local.Value.Remove(session);

            ServerChatPacket packet = new ServerChatPacket();
            packet.message = $"[Exit] {session.Id} : {session.Socket.RemoteEndPoint}";
            Broadcast(packet);

            Logger.Log(LogLevel.Notify, packet.message);
        }
    }
}
