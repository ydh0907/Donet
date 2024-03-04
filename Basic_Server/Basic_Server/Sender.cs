using System.Net.Sockets;

namespace MyFramework
{
    public class Sender
    {
        private readonly Session _session;
        private readonly Socket _socket;
        private readonly SocketAsyncEventArgs _sendArgs;

        private Queue<ArraySegment<byte>> _waitingQueue = new();
        private Queue<ArraySegment<byte>> _sendingQueue = new();

        private object _locker = new object();

        public Sender(Session session)
        {
            _session = session;
            _socket = session.Socket;

            _sendArgs = new SocketAsyncEventArgs();
        }

        public void Send(ArraySegment<byte> data)
        {
            lock (_locker)
            {
                _waitingQueue.Enqueue(data);
            }

            if (_sendingQueue.Count == 0)
            {
                for (int i = 0; i < _waitingQueue.Count; i++)
                {
                    _sendingQueue.Enqueue(_waitingQueue.Dequeue());
                    FlushSendingQueue();
                }
            }
        }

        private void FlushSendingQueue()
        {
            for (int i = 0; i < _sendingQueue.Count; i++)
            {
                
            }
        }
    }
}
