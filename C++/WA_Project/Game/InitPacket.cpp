#include "pch.h"
#include "InitPacket.h"
#include "PacketUtility.h"

ushort InitPacket::GetPacketID()
{
	return 0;
}

Packet* InitPacket::CreatePacket()
{
	return new InitPacket();
}

int InitPacket::Serialize(char* buf, int offset, int size)
{
	int count = 0;
	count += PacketUtility::Write(buf, offset + count, size, id);
	return count;
}

int InitPacket::Deserialize(char* buf, int offset, int size)
{
	int count = 0;
	count += PacketUtility::Read(buf, offset + count, size, id);
	return count;
}

void InitPacket::OnReceived(Session* session)
{
	session->SetAuth(id);
}
