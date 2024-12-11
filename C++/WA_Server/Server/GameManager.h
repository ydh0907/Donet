#pragma once

class Session;
class GameRoom;

static class GameManager
{
public:
	static GameManager* instance;
	GameManager() {
		instance = this;
	}
	~GameManager();
private:
	mutex matchingLock;
	mutex roomLock;
	vector<Session*> matchingQueue;
	vector<GameRoom*> rooms;
private:
	void Matching();
public:
	void EnterMatchQueue(Session* session);
	void ExitMatchingQueue(Session* session);
	void Update();
	void EndRoom(GameRoom* room);
};
