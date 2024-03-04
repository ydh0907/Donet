using System.Net;
using System.Net.Sockets;

namespace MyFramework
{
    public abstract class Session
    {
        private readonly Socket _socket;
        private readonly int _id;

        public Socket Socket => _socket;
        public int ID => _id;

        private Receiver _receiver;
        private Sender _sender;

        public Session(int id, Socket socket)
        {
            _socket = socket;
            _id = id;

            _receiver = new Receiver(this);
            _sender = new Sender(this);
        }

        public void Open()
        {
            Console.WriteLine("[Session] : Open");
            OnCreated(_socket);
        }

        public void Close()
        {
            Console.WriteLine("[Session] : Close");
            OnDestroyed(_socket);
            Socket.Close();
        }

        public void Send(ArraySegment<byte> buffer) => _sender.Send(buffer);

        protected abstract void OnCreated(Socket socket);
        protected abstract void OnDestroyed(Socket socket);
        public abstract void OnSend(ArraySegment<byte> buffer);
        public abstract void OnReceived(ArraySegment<byte> buffer);
    }
}
