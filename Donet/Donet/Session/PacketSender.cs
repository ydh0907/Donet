using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading.Tasks;

using Donet.Utils;

namespace Donet.Sessions
{
    public delegate void SendHandle(int byteTransferred);

    public class PacketSender
    {
        private Socket socket = null;
        private SendHandle handler = null;

        private Serializer serializer = null;
        private MemorySegment memory = null;

        private Atomic<bool> sending = new Atomic<bool>(false);
        private ConcurrentQueue<IPacket> pendingQueue = new ConcurrentQueue<IPacket>();
        private ConcurrentQueue<IPacket> sendingQueue = new ConcurrentQueue<IPacket>();

        public void Initialize(Socket socket, SendHandle handler)
        {
            this.socket = socket;
            this.handler = handler;

            while (!MemoryPool.sendPool.TryDequeue(out memory)) ;
            pendingQueue.Clear();
            sendingQueue.Clear();
            using (var local = sending.Lock())
                local.Set(false);
        }

        public async Task Dispose()
        {
            socket = null;
            handler = null;

            MemoryPool.sendPool.Enqueue(memory);
            pendingQueue.Clear();
            sendingQueue.Clear();
            using (var local = sending.Lock())
                local.Set(false);
        }

        public void Send(IPacket packet)
        {
            pendingQueue.Enqueue(packet);
            using var local = sending.Lock();
            if (!local.Value)
                Flash();
        }

        private void Flash()
        {
            using (var local = sending.Lock())
            {

                local.Set(true);
            }

        }
    }
}
