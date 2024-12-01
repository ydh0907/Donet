#include "config.h"
#include "SocketServer.h"
#include "Session.h"
#include "Packet.h"

class InitPacket : public Packet {

};

int __cdecl main()
{
	Packet* (*func)() = []() {return new InitPacket(); };
	SocketServer listener;
	listener.Initialize();
	listener.Listen();
	listener.StartAccept();
	while (listener.IsListening()) {

	}
}
