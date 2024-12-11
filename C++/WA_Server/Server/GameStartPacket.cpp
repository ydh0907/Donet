#include "config.h"
#include "GameStartPacket.h"

ushort GameStartPacket::GetPacketID()
{
	return 2;
}

Packet* GameStartPacket::CreatePacket()
{
	return new GameStartPacket();
}

int GameStartPacket::Serialize(char* buf, int offset, int size)
{
	return 0;
}

int GameStartPacket::Deserialize(char* buf, int offset, int size)
{
	return 0;
}

void GameStartPacket::OnReceived(Session* session)
{
}
