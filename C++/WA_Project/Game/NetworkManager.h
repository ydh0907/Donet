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
	addrinfo* addr;
	addrinfo hints;
	SOCKET socket;
	Session session;
	bool connected;
public:
	Session GetSession() { return session; }
	bool GetConnected() { return connected; }
};
