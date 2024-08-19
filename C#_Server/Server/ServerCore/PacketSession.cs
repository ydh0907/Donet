namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public void SendPacket(Packet packet)
        {
            ArraySegment<byte> buffer = SendBuffer.UniqueBuffer.Close(packet.Serialize(SendBuffer.UniqueBuffer.Open(1024)));
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
