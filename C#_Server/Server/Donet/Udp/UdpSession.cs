using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Donet.Udp
{
    public abstract class UdpSession
    {
        private Socket socket;
        private EndPoint remoteEndPoint;
        private int connected = 0;

        public Socket Socket => socket;
        public EndPoint RemoteEndPoint => remoteEndPoint;
        public bool Connected => connected == 1;

        private SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
        private ReceiveBuffer receiver;

        private SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        private Queue<ArraySegment<byte>> sendQueue = new Queue<ArraySegment<byte>>();
        private List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();
        private object locker = new object();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract void OnReceive(ArraySegment<byte> buffer);
        public abstract void OnSend(int transferred);

        public UdpSession(EndPoint bindingPoint, int receiveBufferSize = 16384)
        {
            socket = new Socket(bindingPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(bindingPoint);

            receiver = new ReceiveBuffer(receiveBufferSize);
        }

        public void Connect()
        {

        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref connected, 0) == 0)
                return;

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}