using System.Diagnostics;
using System.Threading.Channels;

namespace Test
{
    interface IINTER
    {
        public void HandlePacket();
    }

    class Inter : IINTER
    {
        public void HandlePacket()
        {
            int a = 0;
            a++;
        }

        public void HandleCustom()
        {
            int b = 0;
            b++;
        }
    }

    internal class Program
    {
        static byte[] table = new byte[256];

        static void Main(string[] args)
        {

        }

        //static void Table(byte poly)
        //{
        //    byte oct = 0;
        //    for (ushort i = 0; i < 256; i++)
        //    {
        //        oct = (byte)i;
        //        for (byte bit = 0; bit < 8; bit++)
        //        {
        //            oct = (byte)((oct & 0x80) > 0 ? poly ^ (oct << 1) : oct << 1);
        //        }
        //        table[i] = oct;
        //    }
        //}

        //static byte CalculateCRC(byte seed, byte[] data)
        //{
        //    for (int i = 0; i < data.Length; i++)
        //        seed = table[seed ^ data[i]];
        //    return seed;
        //}
    }
}
