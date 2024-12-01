#include "config.h"
#include "Session.h"
#include "SocketServer.h"
#include "Packet.h"

Session::Session(SocketServer* server, SOCKET socket)
{
	server->serverLock.lock();
	server->sessions.push_back(this);
	server->serverLock.unlock();
	this->server = server;
	client = socket;
	recvbuff = new char[recvBufLen];
	sendbuff = new char[sendBufLen];
}

Session::~Session()
{
	delete[] recvbuff;
	delete[] sendbuff;
	if (client != INVALID_SOCKET)
		closesocket(client);

	server->serverLock.lock();
	for (vector<Session*>::iterator i = server->sessions.begin(); i != server->sessions.end(); i++) {
		if (*i == this) {
			server->sessions.erase(i);
			break;
		}
	}
	server->serverLock.unlock();

	receiveThread = thread(&Session::Receive, this);
}

void Session::Close()
{
	delete this;
}

void Session::Send(Packet* packet)
{
	sessionLock.lock();
	int size = packet->Serialize(sendbuff, 0, sendBufLen);
	int result = send(client, sendbuff, size, 0);
	sessionLock.unlock();
	if (result != 0) {
		cout << "[SEND FAILED] : " << result;
		Close();
	}
}

void Session::Receive()
{

}
