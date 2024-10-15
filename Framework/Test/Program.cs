using System.Diagnostics;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {

        }

        static void Print(byte value)
        {
            string message = "";
            while (value > 1)
            {
                message = message.Insert(0, (value % 2).ToString());
                value >>= 1;
            }
            message = message.Insert(0, value.ToString());
            Console.WriteLine(message);
        }

        static void Timer()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            sw.Stop();
            Console.WriteLine("Timer : " + sw.ElapsedMilliseconds + "ms");
        }
    }
}
