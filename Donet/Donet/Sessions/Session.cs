using System.Net.Sockets;

using Donet.Utils;

namespace Donet.Sessions
{
    public delegate void SessionClose(Session session);

    public class Session
    {
        private ulong id = 0;
        private Socket socket = null;

        private Atomic<bool> active = new Atomic<bool>(false);
        private Atomic<bool> closing = new Atomic<bool>(false);

        public ulong Id => id;
        public Socket Socket => socket;

        public Atomic<bool> Active => active;
        public Atomic<bool> Closing => closing;

        private SessionSender sender = new SessionSender();
        private SessionReceiver receiver = new SessionReceiver();

        public SendHandle sended = null;
        public ReceiveHandle received = null;
        public SessionClose closed = null;

        public void Initialize(ulong id, Socket socket)
        {
            using (var local = active.Lock())
                local.Set(true);

            this.id = id;
            this.socket = socket;

            sender.Initialize(socket, this);
            receiver.Initialize(socket, this);

            sended = null;
            received = null;
            closed = null;
        }

        public void Send(IPacket packet)
        {
            using var activeLocal = active.Lock();
            using var closingLocal = closing.Lock();
            if (activeLocal.Value && !closingLocal.Value)
                sender.Send(packet);
        }

        public void Close()
        {
            using (var local = active.Lock())
                if (!local.Value)
                    return;

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

            closed?.Invoke(this);
            closed = null;

            id = 0;
            socket.Close();
            socket = null;

            using (var local = closing.Lock())
                local.Set(false);
            using (var local = active.Lock())
                local.Set(false);
        }
    }
}
