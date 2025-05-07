using Donet.Sessions;
using Donet.Utils;

namespace Donet
{
    public static class DonetFramework
    {
        public static void Initialize(int packetPoolCount = 64, params IPacket[] packets)
        {
            Logger.Initialize();
            MemoryPool.Initialize();
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
