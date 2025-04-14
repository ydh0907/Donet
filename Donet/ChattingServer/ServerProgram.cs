using Donet.Utils;

namespace ChattingServer
{
    internal class ServerProgram
    {
        static void Main(string[] args)
        {
            Logger.Initialize();
            MemoryPool.Initialize();



            MemoryPool.Dispose();
            Logger.Initialize();
        }
    }
}
