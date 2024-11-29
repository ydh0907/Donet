namespace Donet.RUDP
{
    public enum HeaderType
    {
        Error = -1,
        Null = 0,
        Ack = 1,
        Nak = 2,
        Syn = 4,
        Fin = 8,
    }
}
