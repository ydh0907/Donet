using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Donet.Udp
{
    public abstract class UdpSession : Session
    {
        private Socket socket;
        private EndPoint remoteEndPoint;
        private int connected = 0;

        public override Socket Socket => socket;
        public EndPoint RemoteEndPoint => remoteEndPoint;
        public bool Connected => connected == 1;

        private SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
        private ReceiveBuffer receiver;

        private SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        private Queue<ArraySegment<byte>> sendQueue = new Queue<ArraySegment<byte>>();
        private List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();
        private object locker = new object();

        public UdpSession(EndPoint bindingPoint = null, int receiveBufferSize = 16384)
        {
            socket = new Socket(bindingPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            if (bindingPoint != null)
                socket.Bind(bindingPoint);

            receiver = new ReceiveBuffer(receiveBufferSize);

            remoteEndPoint = null;
        }

        public async Task Connect(IPEndPoint remoteEndPoint, Action<UdpSession>? callback = null)
        {
            if (Interlocked.Exchange(ref connected, 1) == 1 && this.remoteEndPoint != null)
                return;

            this.remoteEndPoint = remoteEndPoint;

            await socket.ConnectAsync(remoteEndPoint);
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref connected, 0) == 0)
                return;

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            remoteEndPoint = null;
        }
    }
}