using Donet.Sessions;
using Donet.Utils;

namespace Donet
{
    public static class DonetFramework
    {
        public static void Initialize(bool useLogger = true, bool logMemoryUsage = true, params IPacket[] packets)
        {
            if (useLogger)
                Logger.Initialize();
            MemoryPool.Initialize(logMemoryUsage && useLogger);
            PacketFactory.Initialize(packets);
        }

        public static void Dispose()
        {
            PacketFactory.Dispose();
            MemoryPool.Dispose();
            Logger.Dispose();
        }
    }
}
