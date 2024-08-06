namespace ServerCore
{
    public class ReceiveBuffer
    {
        public ArraySegment<byte> buffer;
        private int write = 0;
        private int read = 0;

        public ReceiveBuffer(int size)
        {
            buffer = RAB.AcquireFromPool(size);
        }

        ~ReceiveBuffer()
        {
            RAB.ReleaseToPool(buffer);
        }

        public int WriteCount => buffer.Count - write;
        public int DataCount => write - read;

        public ArraySegment<byte> WriteSegment => new ArraySegment<byte>(buffer.Array, buffer.Offset + write, WriteCount);
        public ArraySegment<byte> DataSegment => new ArraySegment<byte>(buffer.Array, buffer.Offset + read, DataCount);

        public bool Write(int count)
        {
            if (write + count > buffer.Count)
                return false;
            write += count;
            return true;
        }

        public bool Read(int count)
        {
            if (read + count > write)
                return false;
            read += count;
            return true;
        }

        public void Clean()
        {
            if (read == 0) return;

            Array.Copy(buffer.Array, buffer.Offset + read, buffer.Array, buffer.Offset, DataCount);
            write -= read;
            read = 0;
        }
    }
}
