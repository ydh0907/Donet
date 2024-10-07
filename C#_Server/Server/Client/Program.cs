using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse("172.31.3.204"), 4000);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            while (true)
            {
                Console.ReadKey();
                byte[] buffer = new byte[1024];
                int size = Encoding.UTF8.GetBytes("안녕하세요!", new Span<byte>(buffer, 0, buffer.Length));
                socket.SendTo(buffer, 0, size, SocketFlags.None, endPoint);

                size = socket.ReceiveFrom(buffer, SocketFlags.None, ref endPoint);
                Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, size));
            }
        }

        static async void 
    }
}
