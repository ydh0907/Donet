using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

using Donet.Utils;

namespace Donet.Sessions
{
    public delegate void SendHandle(int byteTransferred);

    /// <summary>
    /// after using. automatically return to memory pool.
    /// </summary>
    public struct RawPacket
    {
        public MemorySegment memory;
        public ArraySegment<byte> data;
    }

    public class SessionSender : IDisposable
    {
        private Socket socket = null;
        private Session session = null;

        private SendHandle handler => session.sended;

        private SocketAsyncEventArgs sendArgs = null;
        private ConcurrentQueue<RawPacket> pendingPackets = new ConcurrentQueue<RawPacket>();
        private Queue<RawPacket> sendingPackets = new Queue<RawPacket>();
        private List<ArraySegment<byte>> buffers = new List<ArraySegment<byte>>();

        private volatile int sending = 0;

        public ConcurrentQueue<MemorySegment> memoryContainer = new ConcurrentQueue<MemorySegment>();

        public void Initialize(Socket socket, Session session)
        {
            this.socket = socket;
            this.session = session;

            sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += HandleSendCompleted;
            pendingPackets.Clear();
            sendingPackets.Clear();
            buffers.Clear();
        }

        public void Dispose()
        {
            socket = null;
            session = null;

            sendArgs.Dispose();
            sendArgs = null;
            pendingPackets.Clear();
            sendingPackets.Clear();
            buffers.Clear();
        }

        public void Send(RawPacket packet)
        {
            pendingPackets.Enqueue(packet);

            if (Interlocked.CompareExchange(ref sending, 1, 0) == 0)
                Flash();
        }

        private void Flash()
        {
            try
            {
                buffers.Clear();
                sendArgs.BufferList = null;

                while (pendingPackets.TryDequeue(out RawPacket packet))
                {
                    buffers.Add(packet.data);
                    sendingPackets.Enqueue(packet);
                }

                if (buffers.Count > 0)
                {
                    sendArgs.BufferList = buffers;
                    bool pending = socket.SendAsync(sendArgs);
                    if (!pending)
                        HandleSendCompleted(socket, sendArgs);
                }
            }
            catch (Exception ex)
            {
                HandleError(LogLevel.Warning, ex.ToString());
            }
        }

        private void HandleSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success || args.BytesTransferred == 0)
            {
                HandleError(LogLevel.Warning, $"[Session {args.SocketError}] packet sending failed please check session");
                return;
            }

            try
            {
                while (memoryContainer.Count > 0)
                    if (memoryContainer.TryDequeue(out MemorySegment memory))
                        MemoryPool.EnqueueSendMemory(memory);

                while (sendingPackets.Count > 0)
                    memoryContainer.Enqueue(sendingPackets.Dequeue().memory);

                handler?.Invoke(args.BytesTransferred);

                if (!pendingPackets.IsEmpty)
                    Flash();
                else
                {
                    Interlocked.Exchange(ref sending, 0);
                    if (!pendingPackets.IsEmpty && Interlocked.CompareExchange(ref sending, 1, 0) == 0)
                        Flash();
                }
            }
            catch (Exception ex)
            {
                HandleError(LogLevel.Warning, ex.Message);
            }
        }

        private void HandleError(LogLevel level, string message)
        {
            if (session == null)
                return;

            Logger.Log(level, message);

            session?.Close();
        }
    }
}
