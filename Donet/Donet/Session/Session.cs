using System.Net.Sockets;

namespace Donet.Session
{
    public class Session
    {
        private ulong id;
        private Socket socket;

        public ulong Id => id;
        public Socket Socket => socket;

        private PacketSender sender = null;
        private PacketReceiver receiver = null;

        public void Initialize(ulong id, Socket socket)
        {
            this.id = id;
            this.socket = socket;
        }

        public void Close()
        {
        }

        protected virtual void OnConnected() { }
        protected virtual void OnDisconnected() { }
        protected virtual void OnReceived() { }
        protected virtual void OnSent() { }
    }
}
