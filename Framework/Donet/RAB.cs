using System;
using System.Collections.Generic;

namespace Donet
{
    /// <summary>
    /// Use Responsibly And Carefully
    /// </summary>
    public class RAB
    {
        public static ArraySegment<byte> AcquireFromPool(int size)
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
        public static bool ReleaseToPool(ArraySegment<byte> segment)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].buffer == segment.Array && pool[i].Release(segment))
                    return true;
            }
            return false;
        }

        private static List<RAB> pool = new List<RAB>
        {
            new RAB(16777216)
        };

        private object locker = new object();
        private byte[] buffer;
        private List<ArraySegment<byte>> segments;

        private RAB(int size)
        {
            buffer = new byte[size];
            segments = new List<ArraySegment<byte>>(256);
        }

        private ArraySegment<byte> Acquire(int size)
        {
            lock (locker)
            {
                return FindSegment(size);
            }
        }
        private ArraySegment<byte> FindSegment(int size)
        {
            #region CheckEmpty
            if (segments.Count == 0)
            {
                ArraySegment<byte> segment = new ArraySegment<byte>(buffer, 0, size);
                segments.Add(segment);
                return segment;
            }
            #endregion
            #region CheckLast
            int current = segments[^1].Offset + segments[^1].Count;
            int space = buffer.Length - current;
            if (space >= size)
            {
                ArraySegment<byte> segment = new ArraySegment<byte>(buffer, current, size);
                segments.Add(segment);
                return segment;
            }
            #endregion
            #region CheckFirst
            current = 0;
            space = segments[0].Offset;
            if (space >= size)
            {
                ArraySegment<byte> segment = new ArraySegment<byte>(buffer, current, size);
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
                    ArraySegment<byte> segment = new ArraySegment<byte>(buffer, current, size);
                    segments.Insert(i + 1, segment);
                    return segment;
                }
            }
            #endregion
            return null;
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
