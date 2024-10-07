using System;

namespace Donet.Tcp
{
    public abstract class TcpPacketSession : TcpSession
    {
        public TcpPacketSession(int sendBufferSize = 1024)
        {
            this.sendBufferSize = sendBufferSize;
        }

        private readonly int sendBufferSize;

        public void SendPacket(Packet packet)
        {
            ArraySegment<byte> buffer = SendBuffer.UniqueBuffer.Open(sendBufferSize);
            int size = packet.Serialize(buffer);
            buffer = SendBuffer.UniqueBuffer.Close(size);
            Send(buffer);
        }

        public sealed override void OnReceive(ArraySegment<byte> buffer)
        {
            ushort id = PacketFactory.ReadPacketID(buffer);
            Packet packet = PacketFactory.GetPacket(id);
            if (packet.Deserialize(buffer))
            {
                packet.OnReceived(this);
                OnPacketReceived(packet);
            }
            else
                Disconnect();
        }

        public abstract void OnPacketReceived(Packet packet);
    }
}
