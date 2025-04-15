using Donet.Utils;

namespace ChattingClient
{
    internal class ClientProgram
    {
        static void Main(string[] args)
        {
            Logger.Initialize();
            MemoryPool.Initialize();

            MemoryPool.Dispose();
            Logger.Dispose();
        }
    }
}
