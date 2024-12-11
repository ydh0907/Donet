#pragma once
#include "pch.h"
#include "config.h"
#include "NetworkManager.h"

class Session;

class Packet
{
public:
	virtual ~Packet() {

	}
public:
	virtual ushort GetPacketID() abstract;
	virtual Packet* CreatePacket() abstract;
	virtual int Serialize(char* buf, int offset, int size) abstract;
	virtual int Deserialize(char* buf, int offset, int size) abstract;
	virtual void OnReceived(Session* session) abstract;
};
