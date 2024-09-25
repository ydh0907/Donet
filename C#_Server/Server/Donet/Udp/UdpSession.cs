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

        private SocketAsyncEventArgs recvArgs;
        private ReceiveBuffer receiver;

        private SocketAsyncEventArgs sendArgs;
        private Queue<ArraySegment<byte>> sendQueue;
        private List<ArraySegment<byte>> pendingList;
        private object locker = new object();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract void OnReceive(ArraySegment<byte> buffer);
        public abstract void OnSend(int transferred);

        public void Initialize(Socket socket, EndPoint remoteEndPoint, int receiveBufferSize = 16384)
        {
            if (Interlocked.Exchange(ref connected, 1) == 1)
                return;

            this.socket = socket;
            this.remoteEndPoint = remoteEndPoint;

            recvArgs = new SocketAsyncEventArgs();
            receiver = new ReceiveBuffer(receiveBufferSize);
            recvArgs.Completed += ReceiveHandler;
            RegisterReceive();
        }

        public void SetEndPoint(EndPoint remoteEndPoint)
        {
            this.remoteEndPoint = remoteEndPoint;
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref connected, 0) == 0)
                return;

        }

        #region Receive

        private void RegisterReceive()
        {
            if (Interlocked.CompareExchange(ref connected, 0, 0) == 0)
                return;
        }

        private void ReceiveHandler(object? sender, SocketAsyncEventArgs args)
        {
            if (Interlocked.CompareExchange(ref connected, 0, 0) == 0)
                return;
        }

        #endregion
    }
}