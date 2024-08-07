﻿namespace ServerCore
{
    public class RAB
    {
        internal static ArraySegment<byte> AcquireFromPool(int size)
        {
            ArraySegment<byte> segment;
            for (int i = 0; i < pool.Count; i++)
            {
                segment = pool[i].Acquire(size);
                if (segment != null)
                    return segment;
            }
            pool.Add(new RAB(16777216 > size ? 16777216 : size));
            return pool[^1].Acquire(size);
        }
        internal static bool ReleaseToPool(ArraySegment<byte> segment)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].Release(segment))
                    return true;
            }
            return false;
        }

        private static List<RAB> pool = new List<RAB>();

        private byte[] buffer;
        private List<ArraySegment<byte>> segments;

        private RAB(int size)
        {
            buffer = new byte[size];
            segments = new(128);
        }

        private ArraySegment<byte> Acquire(int size)
        {
            Console.WriteLine($"[RAB] Acquired {size}B");
            lock (this)
            {
                return FindSegment(size); ;
            }
        }

        private ArraySegment<byte> FindSegment(int size)
        {
            lock (this)
            {
                #region CheckEmpty
                if (segments.Count == 0)
                {
                    ArraySegment<byte> segment = new(buffer, 0, size);
                    segments.Add(segment);
                    return segment;
                }
                #endregion
                #region CheckLast
                int current = segments[^1].Offset + segments[^1].Count;
                int space = buffer.Length - current;
                if (space >= size)
                {
                    ArraySegment<byte> segment = new(buffer, current, size);
                    segments.Add(segment);
                    return segment;
                }
                #endregion
                #region CheckFirst
                current = 0;
                space = segments[0].Offset;
                if (space >= size)
                {
                    ArraySegment<byte> segment = new(buffer, current, size);
                    segments.Insert(0, segment);
                    return segment;
                }
                #endregion
                #region CheckMiddle
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
                #endregion
                return null;
            }
        }

        private bool Release(ArraySegment<byte> segment)
        {
            Console.WriteLine($"[RAB] Released {segment.Count}B");
            lock (this)
            {
                bool success = segments.Remove(segment);
                return success;
            }
        }
    }
}
