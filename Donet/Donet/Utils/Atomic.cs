using System;
using System.Threading;

namespace Donet.Utils
{
    public class Atomic<T>
    {
        internal T value;
        internal volatile int locked = 0;

        public Atomic() => value = default;
        public Atomic(T value) => this.value = value;

        public Locker<T> Lock(int timeout = 1048576) => new Locker<T>(this, timeout);
    }

    public struct Locker<T> : IDisposable
    {
        public static ulong lockCount = 0;

        private readonly Atomic<T> locker;

        public T Value { get => locker.value; }

        public void Set(T value) => locker.value = value;

        public Locker(Atomic<T> locker, int timeout = 1048576)
        {
            this.locker = locker;

            int count = 0;
            while (Interlocked.CompareExchange(ref locker.locked, 1, 0) != 0)
            {
                if (++count >= timeout)
                    throw new TimeoutException("deadlock detected while acquiring the lock.");
                if (count % 4096 == 0)
                    Thread.SpinWait(1);
            }

            Thread.MemoryBarrier();
            lockCount++;
        }

        public void Dispose()
        {
            Thread.MemoryBarrier();
            Interlocked.Exchange(ref locker.locked, 0);
        }
    }
}
