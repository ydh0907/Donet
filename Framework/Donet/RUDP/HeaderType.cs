namespace Donet.RUDP
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
        Success = 128,  // network : sync state & header : succeed
    }
}
