#include "config.h"
#include "GameManager.h"
#include "GameRoom.h"
#include "Session.h"

GameManager* GameManager::instance = nullptr;

GameManager::~GameManager()
{
	instance = nullptr;
	roomLock.lock();
	for (auto room : rooms)
		room->Close();
	rooms.clear();
	roomLock.unlock();

	matchingLock.lock();
	matchingQueue.clear();
	matchingLock.unlock();
}

void GameManager::EnterMatchQueue(Session* session)
{
	if (session->GetMatching())
		return;
	session->SetMatching(true);

	matchingLock.lock();
	matchingQueue.push_back(session);
	Matching();
	matchingLock.unlock();
}

void GameManager::ExitMatchingQueue(Session* session)
{
	if (!session->GetMatching())
		return;
	session->SetMatching(true);

	matchingLock.lock();
	for (auto iter = matchingQueue.begin(); iter != matchingQueue.end(); iter++)
		if (*iter == session) {
			matchingQueue.erase(iter);
			break;
		}
	matchingLock.unlock();
}

void GameManager::Matching()
{
	while (matchingQueue.size() > 1) {
		Session* c1 = matchingQueue[0];
		matchingQueue.erase(matchingQueue.begin());
		Session* c2 = matchingQueue[0];
		matchingQueue.erase(matchingQueue.begin());

		roomLock.lock();
		GameRoom* room = new GameRoom(c1, c2);
		rooms.push_back(room);
		roomLock.unlock();
	}
}

void GameManager::Update()
{
	roomLock.lock();
	for (GameRoom* room : rooms) {
		room->Update();
	}
	roomLock.unlock();
}

void GameManager::EndRoom(GameRoom* room)
{
	roomLock.lock();
	for (auto iter = rooms.begin(); iter != rooms.end(); iter++) {
		if (*iter == room) {
			rooms.erase(iter);
			break;
		}
	}
	roomLock.unlock();
}
