#include "config.h"
#include "PacketFactory.h"
#include "Packet.h"
#include "Packets.h"

vector <std::pair<ushort, Packet*>> PacketFactory::factory = vector<std::pair<ushort, Packet*>>();

void PacketFactory::Initialize()
{
	InitPacket* init = new InitPacket();
	factory.push_back({ init->GetPacketID(), init });
	MatchingPacket* matching = new MatchingPacket();
	factory.push_back({ matching->GetPacketID(), matching });
}

void PacketFactory::Clear()
{
	for (auto p : factory)
		delete p.second;
	factory.clear();
}

Packet* PacketFactory::CreatePacket(ushort id)
{
	for (auto p : factory) {
		if (p.first == id)
			return p.second->CreatePacket();
	}
	cout << "[FACTORY] : Packet Not Found - " << id << endl;
	return nullptr;
}

ushort PacketFactory::GetMaxID()
{
	return factory.size();
}
