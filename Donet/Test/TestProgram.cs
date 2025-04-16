using Donet.Utils;

namespace Test
{
    internal class TestProgram
    {
        private static Atomic<int> counter = new Atomic<int>(0);

        public static void Main()
        {
            Task t = Task.Run(() =>
            {
                for (int i = 0; i < 1000000; i++)
                {
                    using (var local1 = counter.Lock())
                    {
                        local1.Set(local1.Value + 1);
                        if (local1.Value % 2 == 0)
                            Console.WriteLine("!!!!!");
                        using (var local2 = counter.Lock())
                        {
                            local2.Set(local2.Value + 1);
                            if (local2.Value % 2 == 1)
                                Console.WriteLine("!!!!!");
                        }
                    }
                }
            });
            for (int i = 0; i < 1000000; i++)
            {
                using (var local1 = counter.Lock())
                {
                    local1.Set(local1.Value + 1);
                    if (local1.Value % 2 == 0)
                        Console.WriteLine("!!!!!");
                    using (var local2 = counter.Lock())
                    {
                        local2.Set(local2.Value + 1);
                        if (local2.Value % 2 == 1)
                            Console.WriteLine("!!!!!");
                    }
                }
            }
            Task.WaitAll(t);

            using (var local = counter.Lock())
                Console.WriteLine(local.Value);
        }
    }
}
