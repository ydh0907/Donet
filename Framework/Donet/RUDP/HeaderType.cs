namespace Donet.RUDP
{
    public enum HeaderType
    {
        Error = -1,
        None = 0,       // empty (heart beat)
        Null = 1,       // don't use RUDP
        Connnect = 2,   // connect
        Accept = 4,     // accept
        Send = 8,       // send
        Receive = 16,   // receive (success)
        Sequence = 32,  // receive (fail)
        Fin = 64,       // disconnect
    }
}
