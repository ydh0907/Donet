#undef UNICODE

#define WIN32_LEAN_AND_MEAN

#include <windows.h>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdlib.h>
#include <stdio.h>

#include <thread>
#include <string>
#include <iostream>
#include <vector>
using namespace std;

// Need to link with Ws2_32.lib
#pragma comment (lib, "Ws2_32.lib")
// #pragma comment (lib, "Mswsock.lib")

#define DEFAULT_BUFLEN 512
#define DEFAULT_PORT "27015"

struct client_type
{
	int id;
	SOCKET socket;
};

const int MAX_CLIENT = 5;

int process_client(client_type& new_client, std::vector<client_type>& client_array, std::thread& thread);
int main();

int process_client(client_type& new_client, std::vector<client_type>& client_array, std::thread& thread) {
	string msg = "";
	char tempmsg[DEFAULT_BUFLEN] = "";
	int iResult;

	do {
		memset(tempmsg, 0, DEFAULT_BUFLEN);
		if (new_client.socket != 0) {
			iResult = recv(new_client.socket, tempmsg, DEFAULT_BUFLEN, 0);
			if (iResult != SOCKET_ERROR) {
				if (strcmp("", tempmsg)) {
					msg = "Client #" + to_string(new_client.id) + ": " + tempmsg;
					cout << msg.c_str() << endl;

					for (int i = 0; i < MAX_CLIENT; i++) {
						if (client_array[i].socket != INVALID_SOCKET) {
							if (new_client.id != client_array[i].id) {
								iResult = send(client_array[i].socket, msg.c_str(), strlen(msg.c_str()), 0);
							}
						}
					}
				}
			}
			else {
				msg = "Client #" + to_string(new_client.id) + " Disconnected";
				cout << msg << endl;

				closesocket(new_client.socket);
				client_array[new_client.id].socket = INVALID_SOCKET;

				for (int i = 0; i < MAX_CLIENT; i++) {
					if (client_array[i].socket != INVALID_SOCKET) {
						iResult = send(client_array[i].socket, msg.c_str(), strlen(msg.c_str()), 0);
					}
				}
				break;
			}
		}
	} while (iResult > 0);

	thread.detach();
	return 0;
}

int __cdecl main(void)
{
	WSADATA wsaData; // 윈도우 소켓 구현에 대한 정보
	int iResult;

	SOCKET ListenSocket = INVALID_SOCKET; // 서버 소켓

	struct addrinfo* result = NULL;
	struct addrinfo hints;

	std::string msg = "";
	std::vector<client_type> client(MAX_CLIENT);
	int num_clients = 0;
	int temp_id = -1;
	std::thread my_thread[MAX_CLIENT];

	int iSendResult;
	char recvbuf[DEFAULT_BUFLEN];
	int recvbuflen = DEFAULT_BUFLEN;

	// Initialize Winsock
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != 0) {
		printf("WSAStartup failed with error: %d\n", iResult);
		return 1;
	}

	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_INET; // IPv4
	hints.ai_socktype = SOCK_STREAM; // TCP = STREAM
	hints.ai_protocol = IPPROTO_TCP; // TCP
	hints.ai_flags = AI_PASSIVE;

	// Resolve the server address and port
	// 서버의 IP Port 가져오기
	iResult = getaddrinfo(NULL, DEFAULT_PORT, &hints, &result);
	if (iResult != 0) {
		printf("getaddrinfo failed with error: %d\n", iResult);
		WSACleanup();
		return 1;
	}

	// Create a SOCKET for the server to listen for client connections.
	// Socket 호출
	cout << "Create Socket..." << endl;
	ListenSocket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
	if (ListenSocket == INVALID_SOCKET) {
		printf("socket failed with error: %ld\n", WSAGetLastError());
		freeaddrinfo(result);
		WSACleanup();
		return 1;
	}

	// socket option
	// rebind
	setsockopt(ListenSocket, SOL_SOCKET, SO_REUSEADDR, "1", sizeof(int));
	// 불필요한 전송 줄이기
	setsockopt(ListenSocket, IPPROTO_TCP, TCP_NODELAY, "1", sizeof(int));

	// Setup the TCP listening socket
	// 바인드. IP Port 지정
	cout << "Bind Socket..." << endl;
	iResult = bind(ListenSocket, result->ai_addr, (int)result->ai_addrlen);
	if (iResult == SOCKET_ERROR) {
		printf("bind failed with error: %d\n", WSAGetLastError());
		freeaddrinfo(result);
		closesocket(ListenSocket);
		WSACleanup();
		return 1;
	}

	freeaddrinfo(result);

	// 리슨
	cout << "Listening..." << endl;
	iResult = listen(ListenSocket, SOMAXCONN);
	if (iResult == SOCKET_ERROR) {
		printf("listen failed with error: %d\n", WSAGetLastError());
		closesocket(ListenSocket);
		WSACleanup();
		return 1;
	}

	for (int i = 0; i < MAX_CLIENT; i++) {
		client[i] = { -1, INVALID_SOCKET };
	}

	while (true) {
		SOCKET incoming = INVALID_SOCKET;
		incoming = accept(ListenSocket, NULL, NULL);
		if (incoming == INVALID_SOCKET) continue;

		temp_id = -1;

		for (int i = 0; i < MAX_CLIENT; i++) {
			if (client[i].socket == INVALID_SOCKET && temp_id == -1) {
				client[i].socket = incoming;
				client[i].id = i;
				temp_id = i;
			}
			if (client[i].socket != INVALID_SOCKET) {
				num_clients++;
				break;
			}
		}

		if (temp_id != -1) {
			cout << "Client #" << client[temp_id].id << "Accpted" << endl;
			msg = to_string(client[temp_id].id);
			send(client[temp_id].socket, msg.c_str(), strlen(msg.c_str()), 0);

			my_thread[temp_id] = thread(process_client, ref(client[temp_id]), ref(client), ref(my_thread[temp_id]));
		}
		else {
			msg = "Server is full";
			send(incoming, msg.c_str(), strlen(msg.c_str()), 0);
			cout << msg << endl;
		}
	}

	// No longer need server socket
	closesocket(ListenSocket);

	for (int i = 0; i < MAX_CLIENT; i++) {
		my_thread[i].detach();
		closesocket(client[i].socket);
	}

	WSACleanup();

	return 0;
}