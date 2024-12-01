#include "config.h"
#include "SocketServer.h"
#include "Session.h"

bool SocketServer::Initialize()
{
	if (
		SetAddr() == 0 &&
		SetSocket() != INVALID_SOCKET &&
		Bind() == 0
		)
	{
		return true;
	}
	else {
		Close();
		return false;
	}
}

int SocketServer::SetWSA()
{
	int result = WSAStartup(MAKEWORD(2, 2), &wsaData);
	return result;
}

int SocketServer::SetAddr()
{
	addrinfo hints;
	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;
	hints.ai_flags = AI_PASSIVE;
	int result = getaddrinfo(NULL, port, &hints, &addr);
	return result;
}

SOCKET SocketServer::SetSocket()
{
	listener = socket(addr->ai_family, addr->ai_socktype, addr->ai_protocol);
	return listener;
}

int SocketServer::Bind()
{
	int result = bind(listener, addr->ai_addr, (int)addr->ai_addrlen);
	return result;
}

int SocketServer::Listen()
{
	int result = listen(listener, SOMAXCONN);
	listening = true;
	if (result != 0)
		Close();
	else
		cout << "Listening... " << port << endl;
	return result;
}

void SocketServer::StartAccept()
{
	acceptThread = thread(&SocketServer::AcceptLoop, this);
}

void SocketServer::AcceptLoop()
{
	while (listening) {
		SOCKET client = accept(listener, NULL, NULL);
		if (client == INVALID_SOCKET) {
			cout << "[ACCEPT FAILED] : " << WSAGetLastError();
			continue;
		}
		new Session(this, client);
	}
}

void SocketServer::Close()
{
	listening = false;

	if (listener != INVALID_SOCKET) {
		int result = closesocket(listener);
		if (result != 0)
			cout << result;
	}
	if (addr != nullptr) {
		freeaddrinfo(addr);
	}

	acceptThread.detach();

	cout << "Listener Closed..." << endl;
}
