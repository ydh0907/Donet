using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Donet.Utils;

namespace Donet.Connection
{
    public class Connector
    {
        public static Socket Connect(IPEndPoint endPoint)
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);
                socket.NoDelay = true;
                return socket;
            }
            catch (SocketException ex)
            {
                Logger.Log(LogLevel.Error, $"[Connector] connect failed. {ex}");
                throw;
            }
        }

        public static async Task<Socket> ConnectAsync(IPEndPoint endPoint)
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(endPoint);
                socket.NoDelay = true;
                return socket;
            }
            catch (SocketException ex)
            {
                Logger.Log(LogLevel.Error, $"[Connector] async connect failed. {ex}");
                throw;
            }
        }
    }
}
