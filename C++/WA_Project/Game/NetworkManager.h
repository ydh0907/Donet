#pragma once
#include "pch.h"
#include "config.h"
#include "Session.h"

class NetworkManager
{
	DECLARE_SINGLE(NetworkManager)
public:
	bool Initialize();
	void Close();
private:
	bool SetWSA();
	bool SetAddr();
	bool SetSocket();
public:
	bool Connect();
private:
	WSAData data;
	addrinfo* addr = nullptr;
	addrinfo hints;
	SOCKET socket = INVALID_SOCKET;
	Session* session = nullptr;
	bool connected = false;
public:
	Session* GetSession() { return session; }
	void SetSession(Session* session) { this->session = session; }
	bool GetConnected() { return connected; }
};
