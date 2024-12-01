#pragma once
#pragma pack(1)
#include "config.h"

class Packet
{
public:
	virtual Packet* CreatePacket() abstract;
	virtual int Serialize(const char* buf, int offset, int size) abstract;
	virtual int Deserialize(const char* buf, int offset, int size) abstract;
};
