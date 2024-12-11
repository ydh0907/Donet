#include "config.h"
#include "Session.h"
#include "SocketServer.h"
#include "Packet.h"
#include "Packets.h"
#include "PacketFactory.h"
#include "PacketUtility.h"

ushort Session::session_id = 0;

Session::Session(SocketServer* server, SOCKET socket) : id(session_id++)
{
	server->serverLock.lock();
	server->sessions.push_back(this);
	server->serverLock.unlock();

	this->server = server;
	client = socket;
	recvbuff = new char[recvBufLen];

	actived = true;
	receiveThread = thread(&Session::Receive, this);
	sendThread = thread(&Session::SendLoop, this);
	receiveThread.detach();
	sendThread.detach();

	cout << "[SESSION] : Opened-" << id << endl;

	InitPacket* packet = new InitPacket();
	packet->SetID(id);
	Send(packet);
}

Session::~Session()
{
	cout << "[SESSION] : Closed-" << id << endl;

	delete[] recvbuff;
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
}

void Session::Close()
{
	if (actived) {
		actived = false;
		delete this;
		return;
	}
}

void Session::Send(Packet* packet)
{
	sessionLock.lock();
	sendQueue.push(packet);
	sessionLock.unlock();

	SendLoop();
}

void Session::Send(queue<Packet*>& packets)
{
	sessionLock.lock();
	while (!packets.empty()) {
		sendQueue.push(packets.front());
		packets.pop();
	}
	sessionLock.unlock();

	SendLoop();
}

void Session::Receive()
{
	ushort offset = 0;
	while (actived) {
		int result = recv(client, recvbuff + offset, sendBufLen - offset, 0);
		if (result == SOCKET_ERROR) {
			cout << "[RECV FAILED] : " << WSAGetLastError() << endl;
			Close();
			return;
		}
		if (result > 0) {
			cout << "[RECV SUCCEED] : " << result << "bytes" << endl;
			offset += result;
			ReceiveHandle(offset);
		}
	}
}

void Session::ReceiveHandle(ushort& offset)
{
	ushort size;
	int sizeresult = PacketUtility::Read<ushort>(recvbuff, 0, offset, size);
	if (sizeresult == -1)
		return;
	if (offset >= size) {
		ushort id = 0;
		int idresult = PacketUtility::Read<ushort>(recvbuff, sizeresult, offset, id);
		if (idresult == -1 || id > PacketFactory::GetMaxID()) {
			cout << "[ID SERIALIZE FAILED] : " << id << endl;
			Close();
			return;
		}

		Packet* packet = PacketFactory::CreatePacket(id);
		packet->Deserialize(recvbuff, 4, size);
		packet->OnReceived(this);
		delete packet;

		memmove(recvbuff, recvbuff + size, offset - size);
		offset -= size;
	}
}

void Session::SendLoop()
{
	sessionLock.lock();
	while (!sendQueue.empty()) {
		Packet* packet = sendQueue.front();
		sendQueue.pop();
		sessionLock.unlock();
		Sending(packet);
		sessionLock.lock();
	}

	sessionLock.unlock();
}

void Session::Sending(Packet* packet)
{
	char* buffer = new char[sendBufLen];
	ushort offset = 2;
	offset += PacketUtility::Write<ushort>(buffer, offset, sendBufLen, packet->GetPacketID());
	offset += packet->Serialize(buffer, offset, sendBufLen);
	PacketUtility::Write(buffer, 0, sendBufLen, offset);

	int result = send(client, buffer, offset, 0);
	delete packet;
	delete[] buffer;
	if (result == SOCKET_ERROR) {
		cout << "[SEND FAILED] : " << WSAGetLastError() << endl;
		Close();
		return;
	}
	cout << "[SEND SUCCEED] : " << result << "bytes" << endl;
}

void Session::SetRoom(GameRoom* room)
{
	this->room = room;
}

void Session::CloseRoom()
{
	this->room = nullptr;
}
