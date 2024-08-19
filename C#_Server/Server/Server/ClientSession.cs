﻿using ServerCore;
using System.Net;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Program.OnDisconnected(this);
        }

        public override void OnPacketReceived(Packet packet)
        {
        }

        public override void OnSend(int transferred)
        {
        }
    }
}