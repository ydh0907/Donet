using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Donet.RUDP
{
    public abstract class Session
    {
        protected Socket socket;
        private EndPoint? remoteEndPoint;
        private int connected = 0;

        public Socket Socket => socket;
        public EndPoint? RemoteEndPoint => remoteEndPoint;
        public bool Connected => connected == 1;

        private ushort sendedIndex;
        private Queue<Donet.Packet> sended;

        public Session(AddressFamily addressFamily, EndPoint? bindingPoint = null, int receiveBufferSize = 16384)
        {
            socket = new Socket(addressFamily, SocketType.Dgram, ProtocolType.Udp);

            if (bindingPoint != null)
                socket.Bind(bindingPoint);

            remoteEndPoint = null;
        }

        public async Task Connect(IPEndPoint remoteEndPoint, Action<Session>? callback = null)
        {
            if (Interlocked.Exchange(ref connected, 1) == 1)
                return;

            this.remoteEndPoint = remoteEndPoint;

            await socket.ConnectAsync(remoteEndPoint);
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref connected, 0) == 0)
                return;

            remoteEndPoint = null;
        }

        public void Close()
        {
            Interlocked.Exchange(ref connected, 0);
            remoteEndPoint = null;
            socket.Close();
        }
    }
}