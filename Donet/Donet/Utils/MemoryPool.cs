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
        public const ushort sendBufSize = 1 << 8;
        public const ushort receiveBufSize = 1 << 12;
        public const int bufferSize = 1 << 25;

        private static byte[] sendBuffer = null;
        private static byte[] receiveBuffer = null;

        public static ConcurrentQueue<MemorySegment> sendPool = null;
        public static ConcurrentQueue<MemorySegment> receivePool = null;

        public static void Initialize()
        {
            sendBuffer = new byte[bufferSize];
            receiveBuffer = new byte[bufferSize];

            sendPool = new ConcurrentQueue<MemorySegment>();
            receivePool = new ConcurrentQueue<MemorySegment>();

            int sendSegCnt = bufferSize / sendBufSize;
            for (int i = 0; i < sendSegCnt; i++)
            {
                sendPool.Enqueue(new MemorySegment(sendBuffer, i * sendBufSize, sendBufSize));
            }

            int receiveSegCnt = bufferSize / receiveBufSize;
            for (int i = 0; i < receiveSegCnt; i++)
            {
                receivePool.Enqueue(new MemorySegment(receiveBuffer, i * receiveBufSize, receiveBufSize));
            }
        }

        public static void Dispose()
        {
            int sendSegCnt = bufferSize / sendBufSize;
            if (sendPool.Count < sendSegCnt)
                Logger.Log(LogLevel.Warning, "SendPool Memory Segment Count Not Matched!");
            sendPool.Clear();
            sendPool = null;

            int receiveSegCnt = bufferSize / receiveBufSize;
            if (receivePool.Count < receiveSegCnt)
                Logger.Log(LogLevel.Warning, "ReceivePool Memory Segment Count Not Matched!");
            receivePool.Clear();
            receivePool = null;
        }
    }
}
