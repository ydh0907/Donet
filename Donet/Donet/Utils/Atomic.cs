using System;
using System.Threading;

namespace Donet.Utils
{
    public class Atomic<T>
    {
        internal T value;
        internal volatile int locked = 0;
        internal volatile int thread = -1;

        public Atomic() => value = default;
        public Atomic(T value) => this.value = value;

        public Locker<T> Lock(uint timeout = 838860800) => new Locker<T>(this, timeout);
        public Locker<T> Locker => new Locker<T>(this, 838860800);
    }

    public struct Locker<T> : IDisposable
    {
        private readonly Atomic<T> locker;

        public T Value
        {
            get
            {
                Thread.MemoryBarrier();
                return locker.value;
            }
        }

        public void Set(T value)
        {
            Thread.MemoryBarrier();
            locker.value = value;
        }

        public Locker(Atomic<T> locker, uint timeout = 838860800)
        {
            Thread.MemoryBarrier();
            this.locker = locker;
            uint count = 0;

            if (locker.thread == Thread.CurrentThread.ManagedThreadId)
            {
                Interlocked.Increment(ref locker.locked);
                return;
            }

            while (Interlocked.CompareExchange(ref locker.locked, 1, 0) != 0)
            {
                if (++count >= timeout)
                    throw new TimeoutException("deadlock detected while acquiring the lock.");
                if (count % 4096 == 0)
                    Thread.Sleep(0);
            }
            locker.thread = Thread.CurrentThread.ManagedThreadId;
        }

        public void Dispose()
        {
            Thread.MemoryBarrier();
            if (locker.locked == 1)
                locker.thread = -1;

            Interlocked.Decrement(ref locker.locked);
        }
    }
}
