using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        public static ushort sendSize = 1 << 12;
        public static ushort receiveSize = 1 << 15;
        public static int sendBufferSize = 1 << 25;
        public static int receiveBufferSize = 1 << 28;

        private static volatile byte[] sendBuffer = null;
        private static volatile byte[] receiveBuffer = null;

        private static volatile ConcurrentQueue<MemorySegment> sendPool = null;
        private static volatile ConcurrentQueue<MemorySegment> receivePool = null;

        public static void EnqueueSendMemory(MemorySegment segment)
        {
            sendPool.Enqueue(segment);
        }
        public static MemorySegment DequeueSendMemory()
        {
            MemorySegment memory;
            if (sendPool.Count > 0)
                while (!sendPool.TryDequeue(out memory)) ;
            else
            {
                memory = new MemorySegment(new byte[sendSize], 0, sendSize);
                Logger.Log(LogLevel.Warning, "[Memory] not enough send pooling.");
            }
            return memory;
        }

        public static void EnqueueReceiveMemory(MemorySegment segment)
        {
            receivePool.Enqueue(segment);
        }
        public static MemorySegment DequeueReceiveMemory()
        {
            MemorySegment memory;
            if (receivePool.Count > 0)
                while (!receivePool.TryDequeue(out memory)) ;
            else
            {
                memory = new MemorySegment(new byte[receiveSize], 0, receiveSize);
                Logger.Log(LogLevel.Notify, "[Memory] not enough receive pooling.");
            }
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

            Task.Run(MemoryUsage);
        }

        private static void MemoryUsage()
        {
            while (sendPool != null && receivePool != null)
            {
                Thread.Sleep(10000);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"[Memory Usage]");
                sb.AppendLine($"[Send    Pool] max: {sendBufferSize / sendSize}, remain: {sendPool.Count}");
                sb.Append($"[Receive Pool] max: {receiveBufferSize / receiveSize}, remain: {receivePool.Count}");
                Logger.Log(LogLevel.Notify, sb.ToString());
            }
        }

        private static void AddReceivePool()
        {
            int receiveSegCnt = receiveBufferSize / receiveSize;
            for (int i = 0; i < receiveSegCnt; i++)
            {
                MemorySegment segment = new MemorySegment(receiveBuffer, i * receiveSize, receiveSize);
                receivePool.Enqueue(segment);
            }
        }

        private static void AddSendPool()
        {
            int sendSegCnt = sendBufferSize / sendSize;
            for (int i = 0; i < sendSegCnt; i++)
            {
                MemorySegment segment = new MemorySegment(sendBuffer, i * sendSize, sendSize);
                sendPool.Enqueue(segment);
            }
        }

        public static void Dispose()
        {
            int sendSegCnt = sendBufferSize / sendSize;
            if (sendPool.Count < sendSegCnt)
                Logger.Log(LogLevel.Warning, "send pool memory segment count not matched!");
            sendPool.Clear();
            sendPool = null;

            int receiveSegCnt = receiveBufferSize / receiveSize;
            if (receivePool.Count < receiveSegCnt)
                Logger.Log(LogLevel.Warning, "receive pool memory segment count not matched!");
            receivePool.Clear();
            receivePool = null;
        }
    }
}
