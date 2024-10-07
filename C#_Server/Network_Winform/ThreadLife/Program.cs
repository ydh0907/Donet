namespace ThreadLife
{
    internal class Program
    {
        static class Constants
        {
            public const int mainThread = 3;
            public const int workerThread = 10;
        }

        static void Main(string[] args)
        {
            Thread thread = new Thread(DoWork);
            thread.Start();

            for (int i = 0; i < Constants.mainThread; i++)
            {
                Console.WriteLine($"메인 쓰레드 카운터: {i}");
                Thread.Sleep(10);
            }

            thread.Join();
            Console.WriteLine("퇴근");
        }

        static void DoWork()
        {
            for (int i = 0; i < Constants.workerThread; i++)
            {
                Console.WriteLine($"일꾼 쓰레드 카운터: {i}");
                Thread.Sleep(10);
            }
        }
    }
}
