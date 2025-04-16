using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;

using Donet.Utils;

namespace Donet.Sessions
{
    public delegate void SendHandle(int byteTransferred);

    public class SessionSender
    {
        private Socket socket = null;
        private Session session = null;
        private SendHandle handler = null;

        private Serializer serializer = new Serializer();

        private Atomic<bool> sending = new Atomic<bool>(false);
        private List<ArraySegment<byte>> buffers = new List<ArraySegment<byte>>();
        private ConcurrentQueue<IPacket> pendingQueue = new ConcurrentQueue<IPacket>();
        private Queue<IPacket> sendingQueue = new Queue<IPacket>();
        private Queue<MemorySegment> pendingMemory = new Queue<MemorySegment>();
        private Queue<MemorySegment> sendingMemory = new Queue<MemorySegment>();

        public void Initialize(Socket socket, Session session, SendHandle handler)
        {
            this.socket = socket;
            this.session = session;
            this.handler = handler;

            buffers.Clear();
            pendingQueue.Clear();
            sendingQueue.Clear();

            pendingMemory.Clear();
            sendingMemory.Clear();

            using (var local = sending.Lock())
                local.Set(false);
        }

        public void Dispose()
        {
            // send remain packet
            using (var local = sending.Lock())
                while (local.Value) ;
            Flash();
            using (var local = sending.Lock())
                while (local.Value) ;

            socket = null;
            session = null;
            handler = null;

            buffers.Clear();
            pendingQueue.Clear();
            sendingQueue.Clear();

            pendingMemory.Clear();
            while (pendingMemory.Count > 0)
                MemoryPool.EnqueueReceiveMemory(pendingMemory.Dequeue());

            sendingMemory.Clear();
            while (sendingMemory.Count > 0)
                MemoryPool.EnqueueReceiveMemory(sendingMemory.Dequeue());

            using (var local = sending.Lock())
                local.Set(false);
        }

        public void Send(IPacket packet)
        {
            pendingQueue.Enqueue(packet);
            using (var local = sending.Lock())
                if (local.Value)
                    return;
            Flash();
        }

        private void Flash()
        {
            using (var local = sending.Lock())
                local.Set(true);

            while (pendingQueue.Count > 0)
                if (pendingQueue.TryDequeue(out IPacket packet))
                    sendingQueue.Enqueue(packet);

            buffers.Clear();
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.BufferList = buffers;

            try
            {
                while (sendingQueue.Count > 0)
                {
                    IPacket packet = sendingQueue.Dequeue();
                    MemorySegment memory = GetPendingMemory();
                    sendingMemory.Enqueue(memory);

                    int packetId = PacketFactory.GetPacketId(packet);
                    serializer.Open(NetworkSerializeMode.Serialize, memory.segment, 2);
                    serializer.Serialize(ref packetId);
                    serializer.SerializeObject(ref packet);
                    int size = serializer.GetOffset();
                    serializer.SetOffset(0);
                    serializer.Serialize(ref size);
                    args.BufferList.Add(memory.segment.Slice(0, size));
                }
                bool pending = socket.SendAsync(args);
                if (!pending)
                    HandleSendCompleted(socket, args);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message);

                using (var local = sending.Lock())
                    local.Set(false);

                using (var active = session.Active.Lock())
                    if (active.Value)
                        session.Close();
            }
        }

        private MemorySegment GetPendingMemory()
        {
            if (pendingMemory.Count > 0)
                return pendingMemory.Dequeue();
            else
            {
                MemorySegment segment;
                segment = MemoryPool.DequeueReceiveMemory();
                return segment;
            }
        }

        private void HandleSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            using (var local = sending.Lock())
            {
                if (args.SocketError != SocketError.Success)
                {
                    Logger.Log(LogLevel.Warning, "[Session] packet sending failed please check connection");

                    local.Set(false);

                    using (var active = session.Active.Lock())
                        if (active.Value)
                            session.Close();
                    return;
                }

                while (sendingMemory.Count > 0)
                    pendingMemory.Enqueue(sendingMemory.Dequeue());

                if (pendingMemory.Count == 0)
                {
                    local.Set(false);
                    return;
                }
            }
            Flash();
        }
    }
}
