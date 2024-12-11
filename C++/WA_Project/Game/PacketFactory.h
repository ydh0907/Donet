#pragma once
#include "pch.h"
#include "config.h"

class Packet;

static class PacketFactory
{
private:
	static vector<std::pair<ushort, Packet*>> factory;
public:
	static void Initialize();
	static void Clear();
	static Packet* CreatePacket(ushort id);
	static ushort GetMaxID();
};
