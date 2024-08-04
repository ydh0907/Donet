﻿using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public abstract class Session
    {
        private Socket socket;
        private int connected = 0;

        private SocketAsyncEventArgs recvArgs = new();
        private ReceiveBuffer recvBuffer;
        private int readPoint = 0;

        private SocketAsyncEventArgs sendArgs = new();
        private Queue<byte[]> sendQueue = new();
        private List<ArraySegment<byte>> pendingList = new();
        private object locker = new();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnReceive(ArraySegment<byte> buffer);
        public abstract void OnSend(int transferred);
        public abstract void OnDisconnected(EndPoint endPoint);

        ~Session()
        {
            RAB.SharedBuffer.Release(ref recvBuffer.buffer);
        }

        public void Init(Socket socket)
        {
            if (Interlocked.Exchange(ref connected, 1) == 1)
                return;

            this.socket = socket;

            recvArgs.Completed += ReceiveHandler;
            this.recvBuffer = new ReceiveBuffer(RAB.SharedBuffer.Acquire(8192));
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
        public void Send(byte[] buffer)
        {
            lock (locker)
            {
                sendQueue.Enqueue(buffer);
                if (pendingList.Count == 0)
                {
                    RegisterSend();
                }
                else
                {

                }
            }
        }

        private void RegisterSend()
        {
            while (sendQueue.Count > 0)
            {
                byte[] buffer = sendQueue.Dequeue();
                pendingList.Add(new ArraySegment<byte>(buffer, 0, buffer.Length));
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
                        {
                            RegisterSend();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Send Failed : {ex.Message}");
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
            ArraySegment<byte> writeSegment = recvBuffer.WriteSegment;
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
                    HandleReceive(args);
                    RegisterReceive();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Receive Failed : {ex.Message}");
                }
            }
            else
            {
                Disconnect();
            }
        }

        private void HandleReceive(SocketAsyncEventArgs args)
        {
            if (!recvBuffer.Write(args.BytesTransferred))
                Disconnect();

            Console.WriteLine($"[Session] Handle Size : {args.BytesTransferred}");

            while (recvBuffer.DataCount >= 2)
            {
                ArraySegment<byte> data = recvBuffer.DataSegment;
                ushort length = BitConverter.ToUInt16(data.Array, data.Offset);

                if (length <= recvBuffer.DataCount)
                {
                    OnReceive(new ArraySegment<byte>(data.Array, data.Offset, length));
                    recvBuffer.Read(length);
                }
                else
                    break;
            }

            recvBuffer.Clean();
        }
        #endregion
    }
}
