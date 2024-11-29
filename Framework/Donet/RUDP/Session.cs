using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Donet.RUDP
{
    public abstract class Session : ISession
    {
        protected Socket socket;
        private IPEndPoint? remoteEndPoint;
        private int connected = 0;

        public Socket Socket => socket;
        public EndPoint? RemoteEndPoint => remoteEndPoint;
        public bool Connected => connected == 1;

        private ushort sendedIndex;
        private Queue<ISerializablePacket> sended;

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnReceive(ArraySegment<byte> buffer);
        public abstract void OnSend(int transferred);
        public abstract void OnDisconnected(EndPoint endPoint);

        public Session(AddressFamily addressFamily, int receiveBufferSize = 16384)
        {
            socket = new Socket(addressFamily, SocketType.Dgram, ProtocolType.Udp);
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