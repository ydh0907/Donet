#include "config.h"
#include "GameEndPacket.h"

ushort GameEndPacket::GetPacketID()
{
	return 3;
}

Packet* GameEndPacket::CreatePacket()
{
	return new GameEndPacket();
}

int GameEndPacket::Serialize(char* buf, int offset, int size)
{
	return 0;
}

int GameEndPacket::Deserialize(char* buf, int offset, int size)
{
	return 0;
}

void GameEndPacket::OnReceived(Session* session)
{
}
