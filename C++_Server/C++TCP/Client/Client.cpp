#define WIN32_LEAN_AND_MEAN

#include <windows.h>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdlib.h>
#include <stdio.h>

#include <thread>
#include <string>
#include <iostream>

using namespace std;

#pragma comment (lib, "Ws2_32.lib")
#pragma comment (lib, "Mswsock.lib")
#pragma comment (lib, "AdvApi32.lib")

#define DEFAULT_BUFLEN 512
#define DEFAULT_PORT "27015"
#define IPADDRESS "localhost"

struct client_type
{
	SOCKET socket;
	int id;
	char recv_msg[DEFAULT_BUFLEN];
};

int process_client(client_type& newClient);
int main();

int __cdecl main()
{
	WSADATA wsaData;
	struct addrinfo* result = NULL,
		* ptr = NULL,
		hints;
	string sent_message = "";
	string message = "";
	client_type client = { INVALID_SOCKET, -1, "" };
	int iResult = 0;

	cout << "Starting Client..." << endl;

	// Initialize Winsock
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != 0) {
		printf("WSAStartup failed with error: %d\n", iResult);
		return 1;
	}

	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_UNSPEC;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;

	cout << "Connecting..." << endl;

	// Resolve the server address and port
	iResult = getaddrinfo(static_cast<PCSTR>(IPADDRESS), DEFAULT_PORT, &hints, &result);
	if (iResult != 0) {
		printf("getaddrinfo failed with error: %d\n", iResult);
		WSACleanup();
		return 1;
	}

	// Attempt to connect to an address until one succeeds
	for (ptr = result; ptr != NULL; ptr = ptr->ai_next) {

		// Create a SOCKET for connecting to server
		client.socket = socket(ptr->ai_family, ptr->ai_socktype,
			ptr->ai_protocol);
		if (client.socket == INVALID_SOCKET) {
			printf("socket failed with error: %ld\n", WSAGetLastError());
			WSACleanup();
			return 1;
		}

		// Connect to server.
		iResult = connect(client.socket, ptr->ai_addr, (int)ptr->ai_addrlen);
		if (iResult == SOCKET_ERROR) {
			closesocket(client.socket);
			client.socket = INVALID_SOCKET;
			continue;
		}
		break;
	}

	freeaddrinfo(result);

	if (client.socket == INVALID_SOCKET) {
		printf("Unable to connect to server!\n");
		WSACleanup();
		return 1;
	}

	cout << "Successfully Connected" << endl;

	recv(client.socket, client.recv_msg, DEFAULT_BUFLEN, 0);
	message = client.recv_msg;

	if (message != "Server is full") {
		client.id = atoi(client.recv_msg);
		thread receive = thread(process_client, ref(client));

		do {
			getline(cin, sent_message);
			iResult = send(client.socket, sent_message.c_str(), strlen(sent_message.c_str()), 0);
			if (iResult <= 0) {
				cout << "send failed : " << WSAGetLastError() << endl;
				break;
			}
		} while (iResult > 0);

		receive.detach();
	}
	else {
		cout << client.recv_msg << endl;
		cout << "shutting down socket..." << endl;
		iResult = shutdown(client.socket, SD_SEND);
		if (iResult == SOCKET_ERROR) {
			cout << "shutdown failed : " << WSAGetLastError() << endl;
			closesocket(client.socket);
			WSACleanup();
			system("pause");
			return 0;
		}
	}

	// cleanup
	closesocket(client.socket);
	WSACleanup();
	return 0;
}

int process_client(client_type& client) {
	int iResult = 0;
	do {
		memset(client.recv_msg, 0, DEFAULT_BUFLEN);
		if (client.socket != INVALID_SOCKET) {
			iResult = recv(client.socket, client.recv_msg, DEFAULT_BUFLEN, 0);
			if (iResult != SOCKET_ERROR) {
				cout << client.recv_msg << endl;
			}
			else {
				cout << "recv failed : " << WSAGetLastError() << endl;
				break;
			}
		}
	} while (iResult > 0);

	if (WSAGetLastError() == WSAECONNRESET) {
		cout << "server disconnected" << endl;
	}
	return 0;
}