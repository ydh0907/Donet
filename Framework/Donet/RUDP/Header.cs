namespace Donet.RUDP
{
    /* header
    +---------------+
    |f|s|r|s|a|c|s|n|
    |i|e|e|e|c|o|y|u| 1byte
    |n|q|c|n|c|n|n|l|
    +---------------+
    |      crc      | 1byte
    +---------------+
    |   check sum   | 1byte
    +---------------+
    |  header info  | ~byte
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
    |s -> c| send fin packet with packet index
    |s <- c| send ack packet with packet index
    |f    f| both side wait for packet arrive or fin
    */

    public struct Header : INetworkSerializable
    {
        private byte[] header;

        public void Serialize(Serializer serializer)
        {

        }
    }
}
