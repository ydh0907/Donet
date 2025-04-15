using Donet.Utils;

namespace ChattingServer
{
    internal class ServerProgram
    {
        static void Main(string[] args)
        {
            Logger.Initialize();
            MemoryPool.Initialize();

            Logger.Log(LogLevel.Notify, "Utils Loaded");

            MemoryPool.Dispose();
            Logger.Dispose();
        }
    }
}
