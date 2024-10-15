using System;

namespace Donet.Udp
{
    /// <summary>
    /// Write or Read RUDPHeader
    /// </summary>
    public static class RUDPHeader
    {
        /*         1 2 3
        +---------------+
        |N|S|A|R|F|     |
        |U|Y|C|E|I| crc | 1byte
        |L|N|K|S|N|     |
        +---------------+
        |      crc      | 
        +---------------+ 2byte
        |      crc      |
        +---------------+
        |    payload    | ~byte
        +---------------+

        nul
        non header : 1byte

        syn
        request connection  : null or info

        nul + syn
        heart beat : null

        ack
        connection allow    : null
        packet received     : packet index

        syn + ack
        accept connection   : null or info

        res
        wrong packet        : restart index

        fin
        2 way disconnect handshake : packet index
        |s----c| connection
        |s -> c| send fin packet
        |s <- c| send ack packet with packet index
        |f    f| both side wait for packet arrive or fin

        crc
        packet header crc   : 3bit
        if you send nul packet, check crc and bitflag is valid

        trailer
        crc or checksum     : 2byte
        reverse calculate?
        */

        private static byte nul = 0b10000000;
        private static byte syn = 0b01000000;
        private static byte ack = 0b00100000;
        private static byte res = 0b00010000;
        private static byte fin = 0b00001000;

        /// <summary>
        /// add header and return length
        /// error return -1
        /// </summary>
        public static int AddHeader(ArraySegment<byte> buffer)
        {
            return -1;
        }

        /// <summary>
        /// read header and return length
        /// error return -1
        /// </summary>
        public static int ReadHeader(ArraySegment<byte> buffer)
        {
            return -1;
        }

        /// <summary>
        /// Make CRC for one byte
        /// payload : 5bit, key : 3bit
        /// </summary>
        static byte ByteCRC(byte crc, byte value)
        {
            while (value > 7)
            {
                byte left = 1;
                byte temp = value;
                while (temp > 1)
                {
                    temp >>= 1;
                    left++;
                }
                value ^= (byte)(crc << (left - 4));
            }
            return value;
        }
    }
}
