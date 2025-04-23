namespace Server.Utils
{
    public class Time
    {
        public static Time Instance = new Time();

        public static float deltaTime => Instance.delta;

        private long last;
        public float delta { get; private set; }

        public long syncTick = 0050_0000;

        private Time()
        {
            last = DateTime.Now.Ticks;
        }

        public void SyncTimer()
        {
            while (DateTime.Now.Ticks - last < syncTick) ;
            delta = (DateTime.Now.Ticks - last) / 1000_0000f;
            last = DateTime.Now.Ticks;
        }
    }
}
