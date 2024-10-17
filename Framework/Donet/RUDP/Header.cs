using System;

namespace Donet.RUDP
{
    /// <summary>
    /// Write or Read RUDPHeader
    /// </summary>

    public static class Header
    {
        /*header
                   1 2 3
        +---------------+
        |s|f|s|r|s|a|c|n|
        |u|i|e|e|e|c|o|u| 1byte
        |c|n|q|c|n|c|n|l|
        +---------------+
        |      crc      | 1byte
        +---------------+
        |   check sum   | 1byte
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
        */



        /// <summary>
        /// add header and return length
        /// error return -1
        /// </summary>
        public static HeaderType AddHeader(ArraySegment<byte> buffer)
        {
            return HeaderType.Success | HeaderType.Accept;
        }

        /// <summary>
        /// read header and return length
        /// error return -1
        /// </summary>
        public static int ReadHeader(ArraySegment<byte> buffer)
        {
            return -1;
        }


    }
}
