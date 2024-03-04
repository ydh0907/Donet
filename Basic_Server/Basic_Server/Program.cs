using System.Net;
using System.Net.Sockets;

namespace MyFramework
{
    internal class Program
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
            Listener.Open();

            Connecter = new Connecter(NetworkUtiles.GetLocalIPAddress().Address, 9070);
            Connecter.Connect();
            Connecter.Disconnect();

            Console.ReadKey();

            Listener.Close();
        }
    }
}
