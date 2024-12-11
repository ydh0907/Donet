#pragma once
#include "Packet.h"

class InitPacket : public Packet
{
private:
	ushort id;
public:
	InitPacket* SetID(ushort id) {
		this->id = id;
		return this;
	}
public:
	ushort GetPacketID() override;
	Packet* CreatePacket() override;
	int Serialize(char* buf, int offset, int size) override;
	int Deserialize(char* buf, int offset, int size) override;
	void OnReceived(Session* session) override;
};
