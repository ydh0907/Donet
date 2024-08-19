namespace ServerCore
{
    public class SendBuffer
    {
        public static SendBuffer UniqueBuffer
        {
            get
            {
                if (!uniqueBuffer.IsValueCreated)
                    uniqueBuffer.Value = new SendBuffer(262144);
                return uniqueBuffer.Value;
            }
        }
        private static ThreadLocal<SendBuffer> uniqueBuffer = new ThreadLocal<SendBuffer>();

        private ArraySegment<byte> buffer;
        private int pointer = 0;

        public int remain => buffer.Count - pointer;

        public SendBuffer(int size)
        {
            buffer = RAB.AcquireFromPool(size);
        }

        ~SendBuffer()
        {
            RAB.ReleaseToPool(buffer);
        }

        public ArraySegment<byte> Open(int size)
        {
            if (size > remain)
            {
                pointer = 0;
                if (size > buffer.Count)
                    return null;
            }
            return new ArraySegment<byte>(buffer.Array, buffer.Offset + pointer, size);
        }

        public ArraySegment<byte> Close(int size)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer.Array, buffer.Offset + pointer, size);
            pointer += size;
            return segment;
        }
    }
}
