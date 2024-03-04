namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            byte[] data2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            ArraySegment<byte> a = new ArraySegment<byte>(data, 0, 5);
            ArraySegment<byte> b = new ArraySegment<byte>(data2, 5, 5);

            Console.WriteLine(a);
            Console.WriteLine(b);

            Console.ReadKey();
        }

        static void PrintArray<T>(T[] array) where T : struct
        {
            for(int i = 0; i < array.Length; i++)
            {
                Console.Write(array[i] + " ");
            }
            Console.WriteLine();
        }
    }
}
