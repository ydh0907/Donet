#pragma once
#include <windows.h>
#include <winsock2.h>

#pragma comment (lib, "Ws2_32.lib")

class Socket
{
private:
	UINT BUFFER_LENGTH;
private:
	SOCKET socket = INVALID_SOCKET;
	addrinfo* result = NULL, * ptr = NULL, hints;
public:
	Socket(UINT BUFFER_LENGTH);
private:
	static WSADATA wsaData;
	static BOOL wsaInitialized;
private:
	static void TryInitialize();
};
