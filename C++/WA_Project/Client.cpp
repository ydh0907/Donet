#include <iostream>
#include <WinSock2.h>
#include <ws2tcpip.h>

#pragma comment (lib, "Ws2_32.lib")

#define port "19070"

int __cdecl main()
{
	WSAData data;
	addrinfo* addr = nullptr;
	addrinfo hints;
	int error = 0;

	error = ::WSAStartup(MAKEWORD(2, 2), &data);

	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;

	error = getaddrinfo("172.31.3.204", port, &hints, &addr);
	std::cout << addr->ai_addr << std::endl;

	SOCKET socket = ::socket(addr->ai_family, addr->ai_socktype, addr->ai_protocol);

	error = ::connect(socket, addr->ai_addr, addr->ai_addrlen);

	while (true);
}