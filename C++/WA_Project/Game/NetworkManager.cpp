#include "NetworkManager.h"

bool NetworkManager::Initialize()
{
	if (SetWSA() && SetAddr() && SetSocket()) {
		return true;
	}
	Close();
	return false;
}

void NetworkManager::Close()
{
	connected = false;

	if (socket != INVALID_SOCKET) {
		int result = closesocket(socket);
		if (result != 0)
			cout << result;
	}
	if (addr != nullptr) {
		freeaddrinfo(addr);
	}

	WSACleanup();
}

bool NetworkManager::SetWSA()
{
	int result = ::WSAStartup(MAKEWORD(2, 2), &data);
	if (result != 0) {
		Close();
		return false;
	}
	return true;
}

bool NetworkManager::SetAddr()
{
	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;
	int result = getaddrinfo(ip, port, &hints, &addr);
	if (result != 0) {
		Close();
		return false;
	}
	return true;
}

bool NetworkManager::SetSocket()
{
	socket = ::socket(addr->ai_family, addr->ai_socktype, addr->ai_protocol);
	if (socket == INVALID_SOCKET) {
		Close();
		return false;
	}
	return true;
}

bool NetworkManager::Connect()
{
	int result = connect(socket, addr->ai_addr, addr->ai_addrlen);
	if (result != 0) {
		Close();
		return false;
	}

	session = Session();

	return true;
}
