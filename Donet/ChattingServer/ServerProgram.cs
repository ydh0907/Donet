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
            ThreadPool.SetMaxThreads(8192 << 1, 8192 << 1);
            ThreadPool.SetMinThreads(8192, 8192);

            Logger.Initialize();
            MemoryPool.Initialize();
            PacketFactory.Initialize(new ServerChatPacket());

            Task.Run(StartDebugTimer);
            for (int i = 0; i < 4; i++)
                StartListener(9977 + i);
            ConsoleOperAsync();

            PacketFactory.Dispose();
            MemoryPool.Dispose();
            Logger.Dispose();

            Console.ReadKey();
        }

        public static ulong total = 0;
        private static void StartDebugTimer()
        {
            while (true)
            {
                ulong sum = 0;
                Thread.Sleep(3000);
                using (var local = sessions.Lock())
                {
                    foreach (var session in local.Value)
                    {
                        using var count = session.receiveCount.Lock();
                        using var activeL = session.Active.Lock();
                        using var closeingL = session.Closing.Lock();

                        if (activeL.Value && !closeingL.Value)
                            sum += count.Value - session.lastReceiveCount;
                        session.lastReceiveCount = count.Value;
                    }
                    Logger.Log(LogLevel.Notify, $"[Server] {local.Value.Count} session actived.");
                }
                total += sum;
                Logger.Log(LogLevel.Notify, $"[Server] {sum / 3}p/s. total {total} packet received.");
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
            try
            {
                using var local = sessions.Lock();
                foreach (var session in local.Value)
                {
                    session.Send(packet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                            string id = Console.ReadLine();
                            using (var local = sessions.Lock())
                                foreach (var session in local.Value)
                                {
                                    if (session.Id == ulong.Parse(id))
                                        Logger.Log(LogLevel.Notify, $"{session.Id} : {session.Socket.RemoteEndPoint}");
                                }
                            break;
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

        private static void StartListener(int port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener = new Listener(socket, HandleAccept);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            listener.Listen(4096, endPoint);

            Logger.Log(LogLevel.Notify, $"[Server] listener opened on {port}");
        }

        private static void HandleAccept(Socket socket)
        {
            ulong next = 0;
            using (var local = id.Lock())
            {
                local.Set(local.Value + 1);
                next = local.Value;
            }
            Session session = new Session();
            session.Initialize(next, socket);
            session.closed += HandleDisconnect;
            using (var local = sessions.Lock())
                local.Value.Add(session);

            // Logger.Log(LogLevel.Notify, $"[Enter] {session.Id} : {session.Socket.RemoteEndPoint}");
        }

        private static void HandleDisconnect(Session session)
        {
            using (var local = sessions.Lock())
                local.Value.Remove(session);

            // Logger.Log(LogLevel.Notify, $"[Exit] {session.Id} : {session.Socket.RemoteEndPoint})");
        }
    }
}
