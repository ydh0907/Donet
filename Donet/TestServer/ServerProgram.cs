using System.Net;
using System.Net.Sockets;
using System.Text;

using Donet.Connection;
using Donet.Sessions;
using Donet.Utils;

namespace TestServer
{
    internal class ServerProgram
    {
        private static volatile int id = 0;

        private static ThreadLocal<ulong> received = new ThreadLocal<ulong>(true);

        struct TestPacket : IPacket
        {
            public int info;

            public void OnReceived(Session session)
            {
                received.Value++;
                session.Send(this);
            }
            public void Serialize(Serializer serializer)
            {
                serializer.Serialize(ref info);
            }
        }

        static void Main(string[] args)
        {
            Logger.Initialize();
            MemoryPool.Initialize();
            PacketFactory.Initialize(new TestPacket());

            ThreadPool.SetMaxThreads(24, 24);
            ThreadPool.SetMinThreads(16, 16);

            for (int i = 0; i < 3; i++)
                StartServer(i);

            ulong last = 0;
            StringBuilder stringBuilder = new StringBuilder();
            while (true)
            {
                Thread.Sleep(3000);
                ulong sum = 0;
                foreach (var v in received.Values)
                    sum += v;
                stringBuilder.AppendLine($"Sum : {sum}");
                stringBuilder.AppendLine($"PPS : {(sum - last) / 3}");
                stringBuilder.Append($"CID : {id}");
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
            session.Initialize(Interlocked.Increment(ref id), socket);
            session.Send(new TestPacket() { info = Random.Shared.Next() });
        }
    }
}
