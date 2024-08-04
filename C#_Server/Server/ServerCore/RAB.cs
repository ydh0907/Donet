namespace ServerCore
{
    public class RAB
    {
        public static RAB SharedBuffer = new RAB(1048576);
        public static RAB UniqueBuffer
        {
            get
            {
                if (!uniqueBuffer.IsValueCreated)
                    uniqueBuffer.Value = new RAB(32768);
                return uniqueBuffer.Value;
            }
        }
        private static ThreadLocal<RAB> uniqueBuffer = new ThreadLocal<RAB>();

        private byte[] buffer;
        private List<ArraySegment<byte>> segments;

        public RAB(int size)
        {
            buffer = new byte[size];
            segments = new(128);
        }

        public ArraySegment<byte> Acquire(int size)
        {
            lock (this)
            {
                ArraySegment<byte> segment = FindSegment(size);
                while (segment == null)
                {
                    Resize(buffer.Length * 2);
                    segment = FindSegment(size);
                }
                return segment;
            }
        }
        private ArraySegment<byte> FindSegment(int size)
        {
            lock (this)
            {
                if (segments.Count == 0)
                {
                    ArraySegment<byte> segment = new(buffer, 0, size);
                    segments.Add(segment);
                    return segment;
                }
                int current = 0;
                int space = segments[0].Offset;
                if (space >= size)
                {
                    ArraySegment<byte> segment = new(buffer, current, size);
                    segments.Insert(0, segment);
                    return segment;
                }
                for (int i = 0; i < segments.Count - 1; i++)
                {
                    current = segments[i].Offset + segments[i].Count;
                    space = segments[i + 1].Offset - current;
                    if (space >= size)
                    {
                        ArraySegment<byte> segment = new(buffer, current, size);
                        segments.Insert(i + 1, segment);
                        return segment;
                    }
                }
                current = segments[^1].Offset + segments[^1].Count;
                space = buffer.Length - current;
                if (space >= size)
                {
                    ArraySegment<byte> segment = new(buffer, current, size);
                    segments.Add(segment);
                    return segment;
                }
                return null;
            }
        }
        public void Resize(int size)
        {
            lock (this)
            {
                segments.Clear();
                buffer = new byte[size];
            }
        }
        public bool Release(ref ArraySegment<byte> segment)
        {
            lock (this)
            {
                bool success = segments.Remove(segment);
                if (success)
                    segment = new();
                return success;
            }
        }
    }
}
