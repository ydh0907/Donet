#pragma once
#include "config.h"

class NetworkManager;
class Packet;

class Session
{
private:
	ushort id = 0;
	mutex sessionLock;
	SOCKET client;
	NetworkManager* manager;
	atomic<bool> actived = false;
	atomic<bool> authed = false;
private:
	char* recvbuff;
	thread receiveThread;
	thread sendThread;
	queue<Packet*> sendQueue;
public:
	Session(NetworkManager* manager, SOCKET socket);
	virtual ~Session();
public:
	ushort GetID() { return id; }
public:
	void Close();
	void Send(Packet* packet);
private:
	void Receive();
	void ReceiveHandle(ushort& offset);
	void SendLoop();
	void Sending(Packet* packet);
public:
	bool GetAuth() { return authed.load(); }
	void SetAuth(ushort id) {
		authed = true;
		this->id = id;
		cout << "[SESSION] : Authed-" << id << endl;
	}
};
