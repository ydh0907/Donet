#include "pch.h"
#include "MatchingPacket.h"

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
	cout << "Matching... : " << session->GetID() << endl;
}
