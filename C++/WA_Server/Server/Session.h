#pragma once

class SocketServer;
class GameRoom;
class Packet;

class Session
{
private:
	static ushort session_id;
private:
	ushort id;
	mutex sessionLock;
	SOCKET client;
	SocketServer* server;
	atomic<bool> actived = false;
private:
	atomic<bool> matching = false;
	GameRoom* room = nullptr;
private:
	char* recvbuff;
	thread receiveThread;
	thread sendThread;
	queue<Packet*> sendQueue;
public:
	Session(SocketServer* server, SOCKET socket);
	virtual ~Session();
public:
	ushort GetID() { return id; }
	void SetMatching(bool matching) { this->matching = matching; }
	bool GetMatching() { return matching.load(); }
public:
	void Close();
	void Send(Packet* packet);
	void Send(queue<Packet*>& packets); // touch add
private:
	void Receive();
	void ReceiveHandle(ushort& offset);
	void SendLoop();
	void Sending(Packet* packet);
public:
	void SetRoom(GameRoom* room);
	void CloseRoom();
};
