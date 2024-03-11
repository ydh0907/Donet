namespace ServerCore
{
    internal class Program
    {
        volatile static bool _stop = false;
        static void ThreadMain()
        {
            Console.WriteLine("Thread Start");

            while (_stop == false) { }

            Console.WriteLine("Thread End");
        }

        static void Main(string[] args)
        {
            Task t = new Task(ThreadMain);
            t.Start();

            Thread.Sleep(1000);

            _stop = true;

            Console.WriteLine("Stop Call");
            Console.WriteLine("Waiting Start");
            t.Wait();
            Console.WriteLine("Waiting End");
        }
    }
}
