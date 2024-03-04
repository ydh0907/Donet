using MyFramework;
using Server;
using System.Net.Sockets;

namespace Server
{
    public class Program
    {
        public static Listener Listener { get; private set; }
        public static Connecter Connecter { get; private set; }

        private static Dictionary<int, Session> users = new();
        public static Dictionary<int, Session> Users => users;

        private static int _currentID = int.MinValue;
        public static int NextID
        {
            get
            {
                while (Users.ContainsKey(_currentID))
                {
                    _currentID++;
                    if (_currentID == int.MaxValue)
                        _currentID = int.MinValue;
                }

                return _currentID;
            }
        }

        public static void Main(string[] args)
        {
            Listener = new Listener(9070);
            Listener.OnConnected += ConnectionCallback;
            Listener.Open();

            Console.ReadKey();

            Listener.Close();
        }

        private static void ConnectionCallback(Socket clientSocket)
        {
            ClientSession clientSession = new ClientSession(NextID, clientSocket);
            Users.Add(NextID, clientSession);
        }
    }
}
