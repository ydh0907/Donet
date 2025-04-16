using System.Net.Sockets;
using System.Threading.Tasks;

using Donet.Utils;

namespace Donet.Sessions
{
    public delegate void SessionClose(Session session);

    public class Session
    {
        private ulong id = 0;
        private Socket socket = null;
        private Atomic<bool> active = new Atomic<bool>(false);

        public ulong Id => id;
        public Socket Socket => socket;
        public Atomic<bool> Active => active;

        private SessionSender sender = null;
        private SessionReceiver receiver = null;

        public SendHandle sended = null;
        public ReceiveHandle received = null;
        public SessionClose sessionClosed = null;

        public void Initialize(ulong id, Socket socket, SessionClose sessionClosed = null)
        {
            this.id = id;
            this.socket = socket;
            using (var local = active.Lock())
                local.Set(true);

            sender = new SessionSender();
            sender.Initialize(socket, this, sended);

            receiver = new SessionReceiver();
            receiver.Initialize(socket, this, received);

            this.sessionClosed = sessionClosed;
        }

        public void Send(IPacket packet)
        {
            sender.Send(packet);
        }

        public void Close()
        {
            sessionClosed?.Invoke(this);

            using (var local = active.Lock())
                local.Set(false);

            sender.Dispose();
            receiver.Dispose();

            id = 0;
            sended = null;
            received = null;
            sender = null;
            receiver = null;

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket = null;
        }
    }
}
