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
	atomic<bool> listening = false;
public:
	bool IsListening() { return listening.load(); }
public:
	mutex serverLock;
	vector<Session*> sessions;
private:
	static SocketServer* instance;
public:
	static SocketServer* Get() { return instance; }
public:
	SocketServer() {
		SetWSA();
		instance = this;
	}
	~SocketServer() {
		instance = nullptr;
		for (Session* session : sessions)
			session->Close();
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
