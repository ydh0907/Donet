namespace Test
{
    public enum HeaderType
    {
        Error = -1,     // header error
        None = 0,       // empty
        Null = 1,       // don't use RUDP
        Connnect = 2,   // connect
        Accept = 4,     // accept
        Send = 8,       // send
        Receive = 16,   // receive (success)
        Sequence = 32,  // receive (fail)
        Fin = 64,       // disconnect
        Success = 128,  // connection flag | succeed
    }

    internal class Program
    {
        static byte[] table = new byte[256];

        static void Main(string[] args)
        {
            var buffer = BitConverter.GetBytes((int)HeaderType.Error);
            foreach (var b in buffer)
            {
                Print(b);
            }
            Console.WriteLine();
            Console.WriteLine(HeaderType.Error & HeaderType.Success);
            //Console.Write("Key : ");
            //Print(0x9B);
            //Table(0x9B);
            //for (int i = 0; i < 256; i++)
            //{
            //    Console.Write($"{Convert.ToString(table[i], 16)}, ");
            //    if (i % 8 == 7)
            //        Console.WriteLine();
            //}

            //byte[] data = Encoding.UTF8.GetBytes("12345678");
            //ushort key = CalculateCRC(0x00, data);
            //Console.WriteLine(Convert.ToString(key, 16));

            //byte[] receive = new byte[data.Length];
            //Array.Copy(data, receive, receive.Length);
            //ushort test = CalculateCRC(0x00, receive);
            //Console.WriteLine(Convert.ToString(test, 16));

            //Console.WriteLine(Convert.ToString(key ^ test, 16));
        }

        static void Table(byte poly)
        {
            byte oct = 0;
            for (ushort i = 0; i < 256; i++)
            {
                oct = (byte)i;
                for (byte bit = 0; bit < 8; bit++)
                {
                    oct = (byte)((oct & 0x80) > 0 ? poly ^ (oct << 1) : oct << 1);
                }
                table[i] = oct;
            }
        }

        static byte CalculateCRC(byte seed, byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
                seed = table[seed ^ data[i]];
            return seed;
        }

        static void Print(uint value)
        {
            string message = "";
            while (value > 1)
            {
                message = message.Insert(0, (value % 2).ToString());
                value >>= 1;
            }
            message = message.Insert(0, value.ToString());
            Console.Write(message);
        }
    }
}
