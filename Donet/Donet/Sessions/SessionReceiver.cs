﻿using System;
using System.Net.Sockets;

using Donet.Utils;

namespace Donet.Sessions
{
    public delegate void ReceiveHandle(ushort id, IPacket packet);

    public class SessionReceiver
    {
        public Atomic<ulong> receiveCount = new Atomic<ulong>(0);

        private Socket socket = null;
        private Session session = null;

        private ReceiveHandle handler => session.received;

        private MemorySegment memory = null;
        private int left = 0;
        private int right = 0;

        private Serializer serializer = new Serializer();

        private Atomic<bool> receiving = new Atomic<bool>(false);

        public void Initialize(Socket socket, Session session)
        {
            using (var local = receiving.Locker)
                local.Set(false);

            this.socket = socket;
            this.session = session;

            memory = MemoryPool.DequeueReceiveMemory();
            left = 0;
            right = 0;

            Receive();
        }

        public void Dispose()
        {
            while (true)
                using (var local = receiving.Locker)
                    if (!local.Value)
                        break;

            socket = null;
            session = null;

            left = 0;
            right = 0;
            MemoryPool.EnqueueReceiveMemory(memory);
            memory = null;
        }

        private async void Receive()
        {
            try
            {
                using (var local = receiving.Locker)
                    local.Set(true);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += HandleReceiveCompleted;
                args.SetBuffer(new ArraySegment<byte>(memory.segment.Array, memory.segment.Offset + right, memory.segment.Count - right));

                bool pending = socket.ReceiveAsync(args);
                if (!pending)
                    HandleReceiveCompleted(socket, args);
            }
            catch (Exception ex)
            {
                HandleError(LogLevel.Warning, ex.Message);
            }
        }

        private void HandleReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success || args.BytesTransferred == 0)
            {
                HandleError(LogLevel.Warning, $"[Session {args.SocketError}] packet receiving failed please check session");
                return;
            }

            try
            {
                right += args.BytesTransferred;
                if (right == memory.segment.Count)
                    ClearMemory();

                DeserializePacket();
            }
            catch (Exception ex)
            {
                HandleError(LogLevel.Warning, ex.Message);
                return;
            }

            using (var local = receiving.Locker)
                local.Set(false);

            Receive();
        }

        private void ClearMemory()
        {
            Array.Copy(
                memory.segment.Array,
                memory.segment.Offset + left,
                memory.segment.Array,
                memory.segment.Offset,
                right - left);
            right -= left;
            left = 0;
        }

        private void DeserializePacket()
        {
            int raw = right - left;
            while (raw >= 4)
            {
                var seg = memory.segment;
                serializer.Open(
                    NetworkSerializeMode.Deserialize,
                    new ArraySegment<byte>(seg.Array, seg.Offset + left, raw)
                );

                ushort size = 0;
                serializer.Serialize(ref size);

                if (size == 0)
                    throw new Exception("packet size is zero.");

                if (size <= raw)
                {
                    ushort id = 0;
                    serializer.Serialize(ref id);

                    IPacket packet = PacketFactory.GetPacket(id);
                    serializer.SerializeObject(ref packet);

                    packet.OnReceived(session);
                    left += size;
                    raw -= size;

                    handler?.Invoke(id, packet);

                    using (var local = receiveCount.Locker)
                        local.Set(local.Value + 1);
                }
                else
                    break;
            }
        }

        private void HandleError(LogLevel level, string message)
        {
            if (session == null)
                return;

            using (var local = receiving.Locker)
                local.Set(false);

            Logger.Log(level, message);

            session?.Close();
        }
    }
}
