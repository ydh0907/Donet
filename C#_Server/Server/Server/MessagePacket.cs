using Donet;

namespace Server
{
    internal class MessagePacket : Packet
    {
        public string message;

        public override void OnReceived(Session session)
        {
            Program.Broadcast(this, session as ClientSession);
            lock (Program.locker)
                Console.WriteLine($"{session.Socket.RemoteEndPoint}:{message}");
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref message);
        }
    }
}
