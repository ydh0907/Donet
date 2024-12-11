#include "pch.h"
#include "NetworkManager.h"
#include "PacketFactory.h"

bool NetworkManager::Initialize()
{
	PacketFactory::Initialize();
	if (SetWSA() && SetAddr() && SetSocket()) {
		connected = true;
		return true;
	}
	Close();
	return false;
}

void NetworkManager::Close()
{
	connected = false;

	if (session != nullptr) {
		session->Close();
		session = nullptr;
	}
	if (socket != INVALID_SOCKET) {
		int result = closesocket(socket);
		if (result != 0)
			cout << result;
	}
	if (addr != nullptr) {
		freeaddrinfo(addr);
	}

	PacketFactory::Clear();
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
	if (result == SOCKET_ERROR) {
		cout << "[NETOWRK] : Connect Failed-" << WSAGetLastError() << endl;
		Close();
		return false;
	}

	session = new Session(this, socket);

	return true;
}
