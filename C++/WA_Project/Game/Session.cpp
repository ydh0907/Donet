#include "pch.h"
#include "Session.h"
#include "Packet.h"
#include "NetworkManager.h"
#include "PacketFactory.h"
#include "PacketUtility.h"

Session::Session(NetworkManager* manager, SOCKET socket)
{
	this->manager = manager;
	client = socket;
	recvbuff = new char[recvBufLen];

	actived = true;
	receiveThread = thread(&Session::Receive, this);
	sendThread = thread(&Session::SendLoop, this);
	receiveThread.detach();
	sendThread.detach();
}

Session::~Session()
{
	delete[] recvbuff;
	if (client != INVALID_SOCKET)
		closesocket(client);
}

void Session::Close()
{
	sessionLock.lock();
	if (actived) {
		cout << "[SESSION] : Closed-" << id << endl;
		actived = false;
		authed = false;
		sessionLock.unlock();
		delete this;
		return;
	}
	sessionLock.unlock();
}

void Session::Send(Packet* packet)
{
	sessionLock.lock();
	sendQueue.push(packet);
	sessionLock.unlock();
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
	milliseconds wait = milliseconds(0);
	while (actived) {
		sessionLock.lock();
		if (sendQueue.empty())
		{
			sessionLock.unlock();
			sleep_for(wait);
			continue;
		}
		Packet* packet = sendQueue.front();
		sendQueue.pop();
		sessionLock.unlock();

		Sending(packet);
	}
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
