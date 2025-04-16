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
            MemoryPool.sendBufferSize = 1 << 18;
            MemoryPool.receiveBufferSize = 1 << 20;
            MemoryPool.Initialize();
            PacketFactory.Initialize(new ServerChatPacket());
            Logger.Log(LogLevel.Notify, "[Server] utils loaded");

            StartServer();
            ConsoleOper();

            PacketFactory.Dispose();
            MemoryPool.Dispose();
            Logger.Dispose();
        }

        public static void Broadcast(IPacket packet)
        {
            using (var local = sessions.Lock())
                foreach (var session in local.Value)
                {
                    session.Send(packet);
                }
        }

        private static void ConsoleOper()
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
                            active = false;
                            Logger.Log(LogLevel.Notify, "[Server] server closed");
                            break;
                        case "user":
                        case "User":
                        case "users":
                        case "Users":
                            using (var local = sessions.Lock())
                                foreach (var session in local.Value)
                                {
                                    Console.WriteLine($"{session.Id} : {session.Socket.RemoteEndPoint}");
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
            listener.Listen(3, endPoint);

            Logger.Log(LogLevel.Notify, "[Server] server opened on 9977");
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
            session.Initialize(next, socket, HandleDisconnect);
            using (var local = sessions.Lock())
                local.Value.Add(session);

            Logger.Log(LogLevel.Notify, $"[Enter] {session.Id} : {session.Socket.RemoteEndPoint}");
        }

        private static void HandleDisconnect(Session session)
        {
            using (var local = sessions.Lock())
                local.Value.Remove(session);

            Logger.Log(LogLevel.Notify, $"[Exit] {session.Id} : {session.Socket.RemoteEndPoint}");
        }
    }
}
