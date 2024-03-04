using System.Net.Sockets;

namespace MyFramework
{
    public class Receiver
    {
        private readonly Session _session;
        private readonly Socket _socket;
        private readonly SocketAsyncEventArgs _receiveArgs;

        public Receiver(Session session)
        {
            _session = session;
            _socket = session.Socket;

            _receiveArgs = new SocketAsyncEventArgs();
        }
    }
}
