#pragma once
#include "config.h"

class SocketServer;

class Session
{
private:
	SocketServer* server;
	SOCKET client;
	thread sendThread;
	thread receiveThread;
private:
	uint8_t* recvbuff;
	uint8_t* sendbuff;
public:
	Session(SocketServer* server, SOCKET socket) {
		this->server = server;
		client = socket;
		recvbuff = new uint8_t[recvBufLen];
		sendbuff = new uint8_t[sendBufLen];
	}
	virtual ~Session() {
		delete[] recvbuff;
		delete[] sendbuff;
		if (client != INVALID_SOCKET)
			closesocket(client);
	}
public:
	void Initialize();
	void Close();
private:
	void Send();
	void RegisterReceive();
	void HandleSend();
	void RegisterReceive();
	void HandleReceive();
};
