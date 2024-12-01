#pragma once
#include "config.h"
#include "Session.h"

class SocketServer
{
private:
	WSADATA wsaData;
	addrinfo* addr = nullptr;
	SOCKET listener = INVALID_SOCKET;
	thread acceptThread;
	bool listening = false;
public:
	bool IsListening() { return listening; }
public:
	mutex serverLock;
	vector<Session*> sessions;
public:
	SocketServer() {
		SetWSA();
	}
	~SocketServer() {
		for (Session* session : sessions)
			delete session;
		sessions.clear();
		WSACleanup();
	}
private:
	int SetWSA();
	int SetAddr();
	SOCKET SetSocket();
	int Bind();
	void AcceptLoop();
public:
	bool Initialize();
	int Listen();
	void StartAccept();
	void Close();
};
