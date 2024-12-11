#pragma once
#include "Packet.h"

class MatchingPacket : public Packet
{
public:
	ushort GetPacketID() override;
	Packet* CreatePacket() override;
	int Serialize(char* buf, int offset, int size) override;
	int Deserialize(char* buf, int offset, int size) override;
	void OnReceived(Session* session) override;
};
