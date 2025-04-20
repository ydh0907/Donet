using Donet.Utils;

namespace Test
{
    internal class TestProgram
    {
        static Atomic<int> race = new Atomic<int>();

        public static void Main()
        {
            Task p = Task.Run(Plus);
            Task m = Task.Run(Minus);

            Task.WaitAll(p, m);

            using var local = race.Lock();
            Console.WriteLine(local.Value);
        }

        public static void Plus()
        {
            for (int i = 0; i < 1000000; i++)
            {
                using (var local = race.Lock())
                {
                    using (var local2 = race.Lock())
                    {
                        local2.Set(local2.Value + 1);
                    }
                }
            }
        }

        public static void Minus()
        {
            for (int i = 0; i < 1000000; i++)
            {
                using (var local = race.Lock())
                {
                    using (var local2 = race.Lock())
                    {
                        local2.Set(local2.Value - 1);
                    }
                }
            }
        }
    }
}
