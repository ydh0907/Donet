using System.Net.Sockets;

using Donet.Utils;

namespace Donet.Sessions
{
    public delegate void SessionClose(Session session);

    public class Session
    {
        public Atomic<ulong> receiveCount => receiver.receiveCount;

        private ulong id = 0;
        private Socket socket = null;

        private Atomic<bool> active = new Atomic<bool>(false);
        private Atomic<bool> closing = new Atomic<bool>(false);

        public ulong Id => id;
        public Socket Socket => socket;

        public Atomic<bool> Active => active;
        public Atomic<bool> Closing => closing;

        private SessionSender sender = null;
        private SessionReceiver receiver = null;

        public SendHandle sended = null;
        public ReceiveHandle received = null;
        public SessionClose sessionClosed = null;

        public void Initialize(ulong id, Socket socket, SessionClose sessionClosed = null)
        {
            using (var local = active.Lock())
                local.Set(true);

            this.id = id;
            this.socket = socket;

            sender = new SessionSender();
            sender.Initialize(socket, this);

            receiver = new SessionReceiver();
            receiver.Initialize(socket, this);

            this.sessionClosed = sessionClosed;
        }

        public void Send(IPacket packet)
        {
            sender.Send(packet);
        }

        public void Close()
        {
            using (var local = closing.Lock())
            {
                if (local.Value)
                    return;
                local.Set(true);
            }

            socket.Shutdown(SocketShutdown.Both);

            sender.Dispose();
            receiver.Dispose();

            sended = null;
            received = null;
            sender = null;
            receiver = null;

            sessionClosed?.Invoke(this);

            id = 0;
            socket.Close();
            socket = null;

            using (var local = closing.Lock())
                local.Set(true);
            using (var local = active.Lock())
                local.Set(true);
        }
    }
}
