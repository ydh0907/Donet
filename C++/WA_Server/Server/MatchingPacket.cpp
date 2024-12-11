#include "config.h"
#include "MatchingPacket.h"
#include "GameManager.h"

ushort MatchingPacket::GetPacketID()
{
	return 1;
}

Packet* MatchingPacket::CreatePacket()
{
	return new MatchingPacket();
}

int MatchingPacket::Serialize(char* buf, int offset, int size)
{
	return 0;
}

int MatchingPacket::Deserialize(char* buf, int offset, int size)
{
	return 0;
}

void MatchingPacket::OnReceived(Session* session)
{
	GameManager::instance->EnterMatchQueue(session);
}
