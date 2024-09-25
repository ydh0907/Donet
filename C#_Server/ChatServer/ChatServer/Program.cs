using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    internal class Program
    {
        //사용자 관리 해시테이블
        public static Hashtable clientsList = new Hashtable();
        private static int userCnt = 0; //고유 ID
        private static Mutex mut = new Mutex();

        static void Main(string[] args)
        {
            try
            {
                //Socket(), Bind()
                TcpListener serverSocket = new TcpListener(IPAddress.Any, 3001);
                TcpClient clientSocket = default;
                int counter = 0;
                byte[] bytesFrom = new byte[1024];

                //Listen()
                serverSocket.Start();
                Console.WriteLine("C# Server Started");

                while (true)
                {
                    //Accept()
                    clientSocket = serverSocket.AcceptTcpClient();
                    NetworkStream networkStream = clientSocket.GetStream();

                    counter = userCnt++;

                    //Accept한 클라이언트 처리
                    HandleClient client = new HandleClient();
                    clientsList.Add(counter, client);
                    client.StartClient(clientSocket, clientsList, counter);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static TcpClient GetSocket(int id)
        {
            TcpClient socket = null;
            if (clientsList.ContainsKey(id))
            {
                HandleClient hc = (HandleClient)clientsList[id];
                socket = hc.clientSocket;
            }
            return socket;
        }

        public static void Broadcast(string msg, string username, bool flag)
        {
            byte[] broadcastBytes;

            if (flag)
            {
                broadcastBytes = Encoding.UTF8.GetBytes($"{username}${msg}");
            }
            else
            {
                broadcastBytes = Encoding.UTF8.GetBytes(msg);
            }
            mut.WaitOne();
            foreach (DictionaryEntry item in clientsList)
            {
                TcpClient broadcastSocket;
                HandleClient hc = (HandleClient)item.Value;
                broadcastSocket = hc.clientSocket;
                NetworkStream broadcastStream = broadcastSocket.GetStream();

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
            mut.ReleaseMutex();
        }

        public static void AddUser(string username)
        {
            string msg = $"{username} Joined";
            Broadcast(msg, "", false);
            Console.WriteLine(msg);
        }

        public static void RemoveUser(string username, int userID)
        {
            if (clientsList.ContainsKey(userID))
            {
                string msg = $"{username} Left";
                Broadcast(msg, "", false);
                Console.WriteLine(msg);
                TcpClient clientSocket = GetSocket(userID);
                clientSocket.Close();
                clientsList.Remove(userID);
            }
        }
    }

    //클라이언트 관리
    class HandleClient
    {
        public TcpClient clientSocket;
        public string userName;
        public int userID;

        public Hashtable clientsList;
        private bool isConnected = true;

        public void StartClient(TcpClient inClient, Hashtable clientList, int userSerial)
        {
            userID = userSerial;
            clientSocket = inClient;
            clientsList = clientList;

            //스레드한테 채팅 일 시키기
            Thread ctThread = new Thread(DoChat);
            ctThread.Start();
        }

        //소켓 연결 상태 확인
        public bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);  //읽을 수 있는 데이터가 없음
            if (part1 && part2)
            {
                return false;
            }
            else return true;
        }

        private void DoChat()
        {
            byte[] buffer = new byte[1024];
            string data;
            NetworkStream networkStream = clientSocket.GetStream();

            while (isConnected)
            {
                int numBytesRead;
                if (!SocketConnected(clientSocket.Client))
                {
                    isConnected = false;
                }
                else
                {
                    data = "";
                    while (networkStream.DataAvailable)
                    {
                        numBytesRead = networkStream.Read(buffer, 0, buffer.Length);
                        data = Encoding.UTF8.GetString(buffer, 0, numBytesRead);
                    }

                    int idx = data.IndexOf('$');

                    if (userName == null && idx > 0)
                    {
                        userName = data.Substring(0, idx);
                        Program.AddUser(userName);
                    }
                    else if (idx > 1)
                    {
                        data = data.Substring(0, data.Length - 1);
                        Console.WriteLine(userName + " : " + data);
                        Program.Broadcast(data, userName, true);
                    }
                    else
                    {
                        data = "";
                    }
                }
            }

            Program.RemoveUser(userName, userID);
        }
    }
}
