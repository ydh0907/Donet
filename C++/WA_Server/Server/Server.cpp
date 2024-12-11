#include "config.h"
#include "SocketServer.h"
#include "PacketFactory.h"
#include "GameManager.h"

int __cdecl main()
{
	PacketFactory::Initialize();
	GameManager game;

	SocketServer listener;
	listener.Initialize();
	listener.Listen();
	listener.StartAccept();
	while (listener.IsListening()) {
		game.Update();
		sleep_for(milliseconds(25));
	}

	PacketFactory::Clear();
}
