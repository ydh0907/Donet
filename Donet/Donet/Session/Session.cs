using System;
using System.Net.Sockets;

using Donet.Utils;

namespace Donet.Sessions
{
    public class Session
    {
        private ulong id = 0;
        private Socket socket = null;

        public ulong Id => id;
        public Socket Socket => socket;

        private PacketSender sender = null;
        private PacketReceiver receiver = null;

        public void Initialize(ulong id, Socket socket, SendHandle send = null, ReceiveHandle receive = null)
        {
            this.id = id;
            this.socket = socket;

            sender = new PacketSender();
            sender.Initialize(socket, send);

            receiver = new PacketReceiver();
            receiver.Initialize(socket, receive);
        }

        public struct TestPacket : IPacket
        {
            public IPacket CreateInstance()
            {
                return new TestPacket();
            }

            public void OnReceived(Session session)
            {
                Console.WriteLine("Packet Received!");
            }

            public bool Serialize(Serializer serializer)
            {
                return true;
            }
        }

        public void Send(PacketHeader header, IPacket packet)
        {
            sender.Send(header, packet);
        }

        public async void Close()
        {
            id = 0;

            await sender.Dispose();
            await receiver.Dispose();

            sender = null;
            receiver = null;

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket = null;
        }
    }
}
