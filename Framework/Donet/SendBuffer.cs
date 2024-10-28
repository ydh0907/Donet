using System;
using System.Threading;

namespace Donet
{
    public class SendBuffer
    {
        private static object locker = new object();
        private static int bufferSize = 16384;
        public static int BufferSize
        {
            get => bufferSize;
            set
            {
                lock (locker)
                    bufferSize = value;
            }
        }

        public static SendBuffer UniqueBuffer
        {
            get
            {
                if (!uniqueBuffer.IsValueCreated || !uniqueBuffer.Value.Active)
                    uniqueBuffer.Value = new SendBuffer(bufferSize);
                return uniqueBuffer.Value;
            }
        }
        private static ThreadLocal<SendBuffer> uniqueBuffer = new ThreadLocal<SendBuffer>();

        private ArraySegment<byte> buffer;
        private int pointer = 0;

        public bool Active { get; private set; } = true;
        public bool Opened { get; private set; } = false;

        public int remain => Active ? buffer.Count - pointer : -1;

        public SendBuffer(int size)
        {
            buffer = RAB.AcquireFromPool(size);
        }

        public void Release()
        {
            if (Active)
                Active = !RAB.ReleaseToPool(buffer);
        }

        ~SendBuffer()
        {
            if (Active)
                Active = !RAB.ReleaseToPool(buffer);
        }

        public ArraySegment<byte> Open(int size)
        {
            if (!Active || Opened) return null;
            Opened = true;

            if (size > remain)
            {
                pointer = 0;
                if (size > buffer.Count)
                {
                    Release();
                    while (bufferSize < size)
                        bufferSize *= 2;
                    return UniqueBuffer.Open(size);
                }
            }
            return new ArraySegment<byte>(buffer.Array, buffer.Offset + pointer, size);
        }

        public ArraySegment<byte> Close(int size)
        {
            if (!Active || !Opened) return null;
            Opened = false;

            ArraySegment<byte> segment = new ArraySegment<byte>(buffer.Array, buffer.Offset + pointer, size);
            pointer += size;
            return segment;
        }
    }
}
