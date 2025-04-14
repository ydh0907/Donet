using System;
using System.Net.Sockets;

namespace Donet.Session
{
    public delegate void PacketHandler(ReadOnlyMemory<byte> rawPacket);

    public class PacketReceiver
    {
        private PacketHandler handler;

        public void Initialize(Socket socket, PacketHandler handler)
        {
            this.handler = handler;
        }
    }
}
