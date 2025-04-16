using System;
using System.Collections.Concurrent;

namespace Donet.Utils
{
    public class MemorySegment
    {
        public readonly ArraySegment<byte> segment;

        public MemorySegment(byte[] buffer, int offset, int count)
        {
            segment = new ArraySegment<byte>(buffer, offset, count);
        }
    }

    public static class MemoryPool
    {
        public static ushort sendSize = 1 << 8;
        public static ushort receiveSize = 1 << 13;
        public static int sendBufferSize = 1 << 25;
        public static int receiveBufferSize = 1 << 26;

        private static byte[] sendBuffer = null;
        private static byte[] receiveBuffer = null;

        private static ConcurrentQueue<MemorySegment> sendPool = null;
        private static ConcurrentQueue<MemorySegment> receivePool = null;

        public static void EnqueueSendMemory(MemorySegment segment) => sendPool.Enqueue(segment);
        public static MemorySegment DequeueSendMemory()
        {
            MemorySegment memory;
            if (sendPool.Count > 0)
                while (!sendPool.TryDequeue(out memory)) ;
            else
                memory = new MemorySegment(new byte[sendSize], 0, sendSize);
            return memory;
        }

        public static void EnqueueReceiveMemory(MemorySegment segment) => receivePool.Enqueue(segment);
        public static MemorySegment DequeueReceiveMemory()
        {
            MemorySegment memory;
            if (receivePool.Count > 0)
                while (!receivePool.TryDequeue(out memory)) ;
            else
                memory = new MemorySegment(new byte[receiveSize], 0, receiveSize);
            return memory;
        }

        public static void Initialize()
        {
            sendBuffer = new byte[sendBufferSize];
            receiveBuffer = new byte[receiveBufferSize];

            sendPool = new ConcurrentQueue<MemorySegment>();
            receivePool = new ConcurrentQueue<MemorySegment>();

            AddSendPool();
            AddReceivePool();
        }

        private static void AddReceivePool()
        {
            int receiveSegCnt = receiveBufferSize / receiveSize;
            for (int i = 0; i < receiveSegCnt; i++)
            {
                receivePool.Enqueue(new MemorySegment(receiveBuffer, i * receiveSize, receiveSize));
            }
        }

        private static void AddSendPool()
        {
            int sendSegCnt = sendBufferSize / sendSize;
            for (int i = 0; i < sendSegCnt; i++)
            {
                sendPool.Enqueue(new MemorySegment(sendBuffer, i * sendSize, sendSize));
            }
        }

        public static void Dispose()
        {
            int sendSegCnt = sendBufferSize / sendSize;
            if (sendPool.Count < sendSegCnt)
                Logger.Log(LogLevel.Warning, "SendPool Memory Segment Count Not Matched!");
            sendPool.Clear();
            sendPool = null;

            int receiveSegCnt = receiveBufferSize / receiveSize;
            if (receivePool.Count < receiveSegCnt)
                Logger.Log(LogLevel.Warning, "ReceivePool Memory Segment Count Not Matched!");
            receivePool.Clear();
            receivePool = null;
        }
    }
}
