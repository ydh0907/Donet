using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] buffer = new byte[40000000];
            const int value = 0b1010101010101010101010101010101;

            Stopwatch timer = new Stopwatch();
            timer.Start();

            //TryWrite(buffer, value);
            Marshal(buffer, value);

            timer.Stop();
            Console.WriteLine($"{timer.ElapsedMilliseconds}ms");
        }

        private static void Marshal(byte[] buffer, int value)
        {
            for (int i = 0; i < 10000000; i++)
            {
                MemoryMarshal.Write(new Span<byte>(buffer, i * 4, 4), value);
            }
        }

        private static void TryWrite(byte[] buffer, int value)
        {
            for (int i = 0; i < 10000000; i++)
            {
                BitConverter.TryWriteBytes(new Span<byte>(buffer, i * 4, 4), value);
            }
        }
    }
}
