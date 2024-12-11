#pragma once
#include "Scene.h"
#include "NetworkManager.h"
#include "MatchingPacket.h"

class InitializeScene : public Scene
{
public:
	void Init() override;
private:
	static void Matching(void* sender) {
		cout << "Matching" << endl;
		MatchingPacket* packet = new MatchingPacket();
		Single(NetworkManager)->GetSession()->Send(packet);
	}
};
