#include "Socket.h"

Socket::Socket(UINT BUFFER_LENGTH)
{
	TryInitialize();
	this->BUFFER_LENGTH = BUFFER_LENGTH;
}



#pragma region static

void Socket::TryInitialize()
{
	if (wsaInitialized)
		return;

	int result = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (result != 0) {

	}
}

#pragma endregion
