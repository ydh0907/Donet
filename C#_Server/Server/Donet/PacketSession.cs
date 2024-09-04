using System;

namespace Donet
{
    public abstract class PacketSession : Session
    {
        public new void Send(Packet packet)
        {
            SendPacket(packet);
        }

        public void SendPacket(Packet packet)
        {
            ArraySegment<byte> buffer = SendBuffer.UniqueBuffer.Open(1024);
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
