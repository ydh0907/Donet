#include "config.h"
#include "SocketServer.h"
#include "Session.h"

int __cdecl main()
{
	SocketServer listener;
	listener.Initialize();
	listener.Listen();
	listener.StartAccept();
	while (listener.IsListening()) {

	}
}
