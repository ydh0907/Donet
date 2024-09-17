using Donet;

namespace Client
{
    internal class MessagePacket : Packet
    {
        public string message;

        public override void OnReceived(Session session)
        {
            lock (Program.locker)
            {
                Console.WriteLine(message);
            }
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.SerializeValue(ref message);
        }
    }
}
