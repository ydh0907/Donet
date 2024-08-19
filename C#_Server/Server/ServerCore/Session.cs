using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public abstract class Session
    {
        private Socket socket;
        public Socket Socket => socket;
        private int connected = 0;
        public bool Connected => connected == 1;

        private SocketAsyncEventArgs recvArgs = new();
        private ReceiveBuffer receiver;
        private int readPoint = 0;

        private SocketAsyncEventArgs sendArgs = new();
        private Queue<ArraySegment<byte>> sendQueue = new();
        private List<ArraySegment<byte>> pendingList = new();
        private object locker = new();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnReceive(ArraySegment<byte> buffer);
        public abstract void OnSend(int transferred);
        public abstract void OnDisconnected(EndPoint endPoint);

        public void Initialize(Socket socket, int receiveBufferSize = 16384)
        {
            if (Interlocked.Exchange(ref connected, 1) == 1)
                return;

            this.socket = socket;

            recvArgs.Completed += ReceiveHandler;
            receiver = new ReceiveBuffer(receiveBufferSize);
            RegisterReceive();

            sendArgs.Completed += OnSendCompleted;

            OnConnected(socket.RemoteEndPoint);
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref connected, 0) == 0)
                return;

            OnDisconnected(socket.RemoteEndPoint);
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        #region Send
        public void Send(ArraySegment<byte> buffer)
        {
            if (!Connected)
                return;

            lock (locker)
            {
                sendQueue.Enqueue(buffer);
                if (pendingList.Count == 0)
                    RegisterSend();
            }
        }

        private void RegisterSend()
        {
            if (!Connected)
                return;

            while (sendQueue.Count > 0)
            {
                pendingList.Add(sendQueue.Dequeue());
            }
            sendArgs.BufferList = pendingList;

            bool pending = socket.SendAsync(sendArgs);
            if (!pending)
            {
                OnSendCompleted(null, sendArgs);
            }
        }

        private void OnSendCompleted(object? sender, SocketAsyncEventArgs args)
        {
            lock (locker)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        sendArgs.BufferList = null;
                        pendingList.Clear();

                        OnSend(sendArgs.BytesTransferred);

                        if (sendQueue.Count > 0)
                            RegisterSend();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Send Failed : {ex.Message}");
                        Disconnect();
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }
        #endregion

        #region Receive
        private void RegisterReceive()
        {
            receiver.Clean();
            ArraySegment<byte> writeSegment = receiver.WriteSegment;
            recvArgs.SetBuffer(writeSegment.Array, writeSegment.Offset, writeSegment.Count);
            bool pending = socket.ReceiveAsync(recvArgs);
            if (!pending)
            {
                ReceiveHandler(null, recvArgs);
            }
        }

        private void ReceiveHandler(object? sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    HandleReceiveData(args);
                    RegisterReceive();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Receive Failed : {ex.Message}");
                    Disconnect();
                }
            }
            else
            {
                Disconnect();
            }
        }

        private void HandleReceiveData(SocketAsyncEventArgs args)
        {
            if (!receiver.Write(args.BytesTransferred))
                Disconnect();

            while (receiver.DataCount >= 2)
            {
                ArraySegment<byte> data = receiver.DataSegment;
                ushort length = BitConverter.ToUInt16(data.Array, data.Offset);

                if (length <= receiver.DataCount)
                {
                    OnReceive(new ArraySegment<byte>(data.Array, data.Offset, length));
                    if (!receiver.Read(length))
                        Disconnect();
                }
                else
                    break;
            }
        }
        #endregion
    }
}
