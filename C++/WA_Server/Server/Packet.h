#pragma once
#include "config.h"
#include "SocketServer.h"

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
