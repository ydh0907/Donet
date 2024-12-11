#include "config.h"
#include "GameRoom.h"
#include "Session.h"
#include "GameManager.h"

GameRoom::GameRoom(Session* c1, Session* c2)
{
	this->c1 = c1;
	this->c1->SetRoom(this);
	this->c2 = c2;
	this->c2->SetRoom(this);
}

GameRoom::~GameRoom()
{
	c1->CloseRoom();
	c2->CloseRoom();
	GameManager::instance->EndRoom(this);
}

void GameRoom::Update()
{
	packetLocker.lock();
	c1->Send(c1queue);
	c2->Send(c2queue);
	packetLocker.unlock();

	objLocker.lock();
	objLocker.unlock();
}

void GameRoom::Close()
{
	delete this;
}

void GameRoom::SendOther(Packet* packet, Session* sender)
{
	packetLocker.lock();
	if (sender == c1)
		c2queue.push(packet);
	else
		c1queue.push(packet);
	packetLocker.unlock();
}

void GameRoom::Broadcast(Packet* packet)
{
	packetLocker.lock();
	c1queue.push(packet);
	c2queue.push(packet);
	packetLocker.unlock();
}

void GameRoom::CreateObject(ushort id, network_id obj_id)
{

}

void GameRoom::DeleteObject(ushort id)
{
}
