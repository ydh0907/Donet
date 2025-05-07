using Donet.Sessions;
using Donet.Utils;

namespace Donet
{
    public static class DonetFramework
    {
        public static void Initialize(bool checkMemoryUsage = true, int packetPoolCount = 64, params IPacket[] packets)
        {
            Logger.Initialize();
            MemoryPool.Initialize(checkMemoryUsage);
            PacketFactory.Initialize(packetPoolCount, packets);
        }

        public static void Dispose()
        {
            PacketFactory.Dispose();
            MemoryPool.Dispose();
            Logger.Dispose();
        }
    }
}
