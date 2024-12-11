#pragma once
#include "netobject.h"

class Session;
class Packet;
class NetworkObject;

class GameRoom
{
private:
	Session* c1;
	Session* c2;
	queue<Packet*> c1queue;
	queue<Packet*> c2queue;
	mutex packetLocker;
private:
	vector<NetworkObject*> objects;
	mutex objLocker;
public:
	GameRoom(Session* c1, Session* c2);
	~GameRoom();
public:
	void Update();
	void Close();
	void SendOther(Packet* packet, Session* sender);
	void Broadcast(Packet* packet);
	void CreateObject(ushort id, network_id obj_id);
	void DeleteObject(ushort id);
};
