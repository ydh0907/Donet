using System.Net.Sockets;
using System.Threading.Tasks;

using Donet.Utils;

namespace Donet.Sessions
{
    public delegate void ReceiveHandle(PacketHeader header, IPacket packet);

    public class PacketReceiver
    {
        private Socket socket = null;
        private ReceiveHandle handler = null;

        public void Initialize(Socket socket, ReceiveHandle handler)
        {
            this.socket = socket;
            this.handler = handler;
        }

        public async ValueTask Dispose()
        {
            socket = null;
            handler = null;
        }
    }
}
