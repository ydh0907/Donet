using System;
using System.Net.Sockets;
using System.Threading;

namespace Donet.Sessions
{
    public delegate void SessionClose(Session session);

    [Serializable]
    public class Session
    {
        public int id = 0;
        private Socket socket = null;

        public Socket Socket => socket;

        public bool Active => active == 1;

        private SessionSender sender = new SessionSender();
        private SessionReceiver receiver = new SessionReceiver();

        public SendHandle sended = null;
        public ReceiveHandle received = null;
        public SessionClose closed = null;

        private volatile int active = 0;

        public void Initialize(int id, Socket socket)
        {
            if (Interlocked.CompareExchange(ref active, 1, 0) == 1)
                return;

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
            if (Active)
                sender.Send(packet);
        }

        public void Close()
        {
            if (Interlocked.CompareExchange(ref active, 0, 1) == 0)
                return;

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
        }
    }
}
