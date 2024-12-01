#pragma once
#include "config.h"
#include "Packet.h"

static class PacketFactory
{
private:
	static vector<uint16_t, void*> factory;
public:
	template <class p>
	static void RegisterPacket(uint16_t id) {
		factory.push_back(id, []() {}));
		if (std::is_base_of(Packet, p)) {
		}
	}
};
