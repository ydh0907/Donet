using System;

namespace Donet
{
    public class ReceiveBuffer
    {
        public ArraySegment<byte> buffer;
        private int write = 0;
        private int read = 0;

        public bool Active { get; private set; } = true;

        public ReceiveBuffer(int size)
        {
            buffer = RAB.AcquireFromPool(size);
        }

        public void Release()
        {
            if (Active)
                Active = !RAB.ReleaseToPool(buffer);
        }

        ~ReceiveBuffer()
        {
            if (Active)
                RAB.ReleaseToPool(buffer);
        }

        public int WriteCount => Active ? buffer.Count - write : -1;
        public int DataCount => Active ? write - read : -1;

        public ArraySegment<byte> WriteSegment => Active ? new ArraySegment<byte>(buffer.Array, buffer.Offset + write, WriteCount) : null;
        public ArraySegment<byte> DataSegment => Active ? new ArraySegment<byte>(buffer.Array, buffer.Offset + read, DataCount) : null;

        public bool Write(int count)
        {
            if (!Active || write + count > buffer.Count)
                return false;
            write += count;
            return true;
        }

        public bool Read(int count)
        {
            if (!Active || read + count > write)
                return false;
            read += count;
            return true;
        }

        public void Clean()
        {
            if (!Active && read == 0) return;

            Array.Copy(buffer.Array, buffer.Offset + read, buffer.Array, buffer.Offset, DataCount);
            write -= read;
            read = 0;
        }
    }
}
