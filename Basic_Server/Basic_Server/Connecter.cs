using System.Net;
using System.Net.Sockets;

namespace MyFramework
{
    public class Connecter
    {
        private Socket _connecter;
        private IPEndPoint _endPoint;

        public Connecter(long IPAdress, int port)
        {
            _connecter = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _endPoint = new IPEndPoint(IPAdress, port);
        }

        public Connecter(string IPAdress, int port)
        {
            _connecter = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _endPoint = new IPEndPoint(IPAddress.Parse(IPAdress), port);
        }

        public void SetConnection(long IPAdress, int port)
        {
            _endPoint = new IPEndPoint(IPAdress, port);
        }

        public void SetConnection(string IPAdress, int port)
        {
            _endPoint = new IPEndPoint(IPAddress.Parse(IPAdress), port);
        }

        public void Connect()
        {
            _connecter.ConnectAsync(_endPoint);
        }

        public void Disconnect()
        {
            _connecter.Disconnect(true);
        }
    }
}
