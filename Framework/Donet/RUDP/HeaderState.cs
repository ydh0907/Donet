namespace Donet.RUDP
{
    public struct HeaderState
    {
        private readonly HeaderType state;

        public bool Error => state == HeaderType.Error;
    }
}
