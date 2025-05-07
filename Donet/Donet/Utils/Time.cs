using System;

namespace Donet.Utils
{
    public class Time
    {
        public static long syncTick = 0050_0000;

        public static float delta { get; private set; }

        private static long last = DateTime.Now.Ticks;

        public static void SyncTimer()
        {
            while (DateTime.Now.Ticks - last < syncTick) ;
            delta = (DateTime.Now.Ticks - last) / 1000_0000f;
            last = DateTime.Now.Ticks;
        }
    }
}
