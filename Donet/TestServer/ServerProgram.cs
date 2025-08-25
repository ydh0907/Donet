using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Donet;
using Donet.Connection;
using Donet.Sessions;
using Donet.Utils;

namespace TestServer
{
    internal class ServerProgram
    {
        private static volatile int id = 0;

        private static ThreadLocal<ulong> received = new ThreadLocal<ulong>(true);

        private static List<Session> sessions = new List<Session>(4096);
        private static ConcurrentQueue<Session> pendingAdd = new ConcurrentQueue<Session>();
        private static ConcurrentQueue<Session> pendingRemove = new ConcurrentQueue<Session>();

        struct TestPacket : IPacket
        {
            public int info;

            public IPacket Create()
            {
                return new TestPacket();
            }
            public void OnReceived(Session session)
            {
                received.Value++;
            }
            public void Serialize(ref Serializer serializer)
            {
                serializer.Serialize(ref info);
            }
        }

        static void Main(string[] args)
        {
            DonetFramework.Initialize(false, false, new TestPacket());

            ThreadPool.SetMaxThreads(24, 24);
            ThreadPool.SetMinThreads(16, 16);

            for (int i = 0; i < 3; i++)
                StartServer(i);

            Console.WriteLine("[Server] started on 9977");

            Task.Run(ServerLog);

            while (true)
            {
                Time.SyncTimer();
                while (pendingAdd.Count > 0)
                    if (pendingAdd.TryDequeue(out Session session))
                        sessions.Add(session);
                while (pendingRemove.Count > 0)
                    if (pendingRemove.TryDequeue(out Session session))
                        sessions.Remove(session);

                int count = sessions.Count / 512;
                Task[] tasks = new Task[count + 1];
                for (int i = 0; i <= count; i++)
                {
                    tasks[i] = UpdateSession(i);
                }
                Task.WaitAll();
            }
        }

        private static Task UpdateSession(int index)
        {
            return Task.Run(() =>
            {
                for (int i = index * 512; i < Math.Min(sessions.Count, (index + 1) * 512); i++)
                    sessions[i].Send(new TestPacket() { info = Random.Shared.Next() });
            });
        }

        private static void ServerLog()
        {
            ulong last = 0;
            StringBuilder stringBuilder = new StringBuilder();
            while (true)
            {
                Thread.Sleep(3000);
                ulong sum = 0;
                foreach (var v in received.Values)
                    sum += v;
                stringBuilder.AppendLine("[Server Stats]");
                stringBuilder.AppendLine($"Sum : {sum}");
                stringBuilder.AppendLine($"PPS : {(sum - last) / 3}");
                stringBuilder.AppendLine($"DLT : {Time.delta}");
                stringBuilder.Append($"CID : {sessions.Count}");
                Console.WriteLine(stringBuilder.ToString());
                stringBuilder.Clear();
                last = sum;
            }
        }

        private static void StartServer(int i)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 9977 + i);
            Listener listener = new Listener(socket, endPoint, HandleAccept);
            listener.Listen(1000);
        }

        static void HandleAccept(Socket socket)
        {
            Session session = new Session();
            session.closed += HandleClose;
            session.Initialize(Interlocked.Increment(ref id), socket);
            pendingAdd.Enqueue(session);
        }

        private static void HandleClose(Session session)
        {
            pendingRemove.Enqueue(session);
        }
    }
}
