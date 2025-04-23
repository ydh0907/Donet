
using System.Net;
using System.Net.Sockets;

using Donet.Connection;
using Donet.Sessions;
using Donet.Utils;

using Server.Packets;
using Server.Pool;
using Server.Utils;

namespace Server
{
    internal class Server
    {
        private static Atomic<ulong> nextId = new Atomic<ulong>(0);
        private static Atomic<List<Session>> sessions = new Atomic<List<Session>>(new List<Session>());

        static void Main(string[] args)
        {
            ThreadPool.SetMaxThreads(8192, 8192);
            ThreadPool.SetMinThreads(4096, 8192);

            Logger.Initialize();
            MemoryPool.Initialize();
            PacketFactory.Initialize(
                new InitializePacket()
            );
            SessionPool.Initialize(1024);

            for (int i = 0; i < 3; i++)
                StartListener(9977 + i);

            try
            {
                while (true)
                {
                    Time.Instance.SyncTimer();

                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"[Server] error detected. {ex.ToString()}");
            }
            finally
            {
                SessionPool.Dispose();
                PacketFactory.Dispose();
                MemoryPool.Dispose();
                Logger.Dispose();
            }
        }

        private static void StartListener(int port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            Listener listener = new Listener(socket, endPoint, HandleAccept);
            listener.Listen(512);
        }

        private static void HandleAccept(Socket socket)
        {
            Session session = SessionPool.PopSession();
            using (var nextId = Server.nextId.Lock())
            {
                nextId.Set(nextId.Value + 1);
                session.Initialize(nextId.Value, socket);
            }
            session.closed += HandleSessionClose;

            string message = $"[Server] client connected. {session.Id}:{session.Socket.RemoteEndPoint}";
            Logger.Log(LogLevel.Notify, message);
            Console.WriteLine(message);

            using (var sessions = Server.sessions.Lock())
                sessions.Value.Add(session);

            session.Send(new InitializePacket() { id = session.Id });
        }

        private static void HandleSessionClose(Session session)
        {
            string message = $"[Server] client disconnected. {session.Id}:{session.Socket.RemoteEndPoint}";
            Logger.Log(LogLevel.Notify, message);
            Console.WriteLine(message);

            SessionPool.PushSession(session);
        }
    }
}
