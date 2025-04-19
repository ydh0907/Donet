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

        public Locker<T> Lock(uint timeout = 134217728) => new Locker<T>(this, timeout);
        public Locker<T> Locker => new Locker<T>(this, 134217728);
    }

    public struct Locker<T> : IDisposable
    {
        public static int thread = -1;

        private readonly Atomic<T> locker;

        public T Value { get => locker.value; }

        public void Set(T value) => locker.value = value;

        public Locker(Atomic<T> locker, uint timeout = 134217728)
        {
            this.locker = locker;

            uint count = 0;
            if (thread == Thread.CurrentThread.ManagedThreadId)
            {
                Interlocked.Increment(ref locker.locked);
            }
            else
                while (Interlocked.CompareExchange(ref locker.locked, 1, 0) != 0)
                {
                    if (++count >= timeout)
                        throw new TimeoutException("deadlock detected while acquiring the lock.");
                    if (count % 4096 == 0)
                        Thread.SpinWait(1);
                }

            Thread.MemoryBarrier();
            thread = Thread.CurrentThread.ManagedThreadId;
        }

        public void Dispose()
        {
            if (locker.locked == 1)
                thread = -1;
            Thread.MemoryBarrier();
            Interlocked.Decrement(ref locker.locked);
        }
    }
}
