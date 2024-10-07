using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 4000);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(endPoint);

            EndPoint remote = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                byte[] buffer = new byte[1024];
                int size = socket.ReceiveFrom(buffer, ref remote);
                Console.WriteLine(remote.ToString() + " : " + Encoding.UTF8.GetString(buffer, 0, size));

                buffer = Encoding.UTF8.GetBytes("Server : 반가워요!");
                socket.SendTo(buffer, 0, buffer.Length, SocketFlags.None, remote);
            }
        }
    }
}
