#pragma once
#include "config.h"

class SocketServer;
class Packet;

class Session
{
private:
	SocketServer* server;
	SOCKET client;
	thread sendThread;
	thread receiveThread;
	const char* recvbuff;
	const char* sendbuff;
public:
	mutex sessionLock;
public:
	Session(SocketServer* server, SOCKET socket);
	virtual ~Session();
public:
	void Initialize();
	void Close();
public:
	void Send(Packet* packet);
private:
	void Receive();
};
