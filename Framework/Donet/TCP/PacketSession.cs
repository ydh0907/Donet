using System;

namespace Donet.TCP
{
    public abstract class PacketSession : Session
    {
        public PacketSession(int sendBufferSize = 1024)
        {
            this.sendBufferSize = sendBufferSize;
        }

        private readonly int sendBufferSize;

        public void SendPacket(ISerializablePacket packet)
        {
            ArraySegment<byte> buffer = SendBuffer.UniqueBuffer.Open(sendBufferSize);
            int size = PacketSerializer.Serialize(buffer, packet);
            buffer = SendBuffer.UniqueBuffer.Close(size);
            Send(buffer);
        }

        public sealed override void OnReceive(ArraySegment<byte> buffer)
        {
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ISerializablePacket packet = PacketFactory.GetPacket(id);
            if (PacketSerializer.Deserialize(buffer, packet))
            {
                packet.OnReceived(this);
                OnPacketReceived(packet);
            }
            else
                Disconnect();
        }

        public abstract void OnPacketReceived(ISerializablePacket packet);
    }
}
