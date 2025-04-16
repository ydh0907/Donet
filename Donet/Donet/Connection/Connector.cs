using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Donet.Connection
{
    public class Connector
    {
        public static Socket Connect(IPEndPoint endPoint)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);
            return socket;
        }

        public static async Task<Socket> ConnectAsync(IPEndPoint endPoint)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(endPoint);
            return socket;
        }
    }
}
