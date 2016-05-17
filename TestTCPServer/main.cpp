// TestCPPClient.cpp : Defines the entry point for the console application.
//
#include "..\CommmonCPP\stdafx.h"
#include "ServerHandler.h"

DWORD WINAPI ThreadFunc(void* p)
{
	SOCKET sock = (SOCKET)p;

	while (1)
	{
		_PACKET_STRUCT packet = { 0 };
		int n = recv(sock, (char*)&packet, 4, 0);
		int total = packet.size;
		printf("recv\n");

		int current = 0;
		current += n;
		while (total > current)
		{
			n = recv(sock, (char*)&packet + current, total - current, 0);
			if (n > 0)
			{
				current += n;
				continue;
			}
			else if (n <= 0)
			{
				cout << "error recv packet." << WSAGetLastError() << endl;
				return -1;
			}
		}

		NetPeer* peer = new NetPeer(sock);
		ProtocolHandler* handler = new ServerHandler(peer);
		string received_packet = string((char*)packet.buffer);
		if (!handler->Process(received_packet))
		{
			cout << "error process packet." << endl;
			delete peer;
			delete handler;
			return -1;
		}
	}

	return 0;
}

void main()
{
	WSADATA wsadata;
	if (WSAStartup(MAKEWORD(2, 2), &wsadata) != 0)
	{
		cout << "failed winsock init." << WSAGetLastError() << endl;
		return;
	}

	SOCKET sock = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (sock == SOCKET_ERROR)
	{
		cout << "failed create socket." << endl;
		return;
	}

	SOCKADDR_IN addr;
	addr.sin_family = AF_INET;
	addr.sin_port = htons(4500);
	addr.sin_addr.s_addr = INADDR_ANY;
	if (bind(sock, (SOCKADDR*)&addr, sizeof(addr)) == -1)
	{
		cout << "failed binding." << endl;
		return;
	}

	if (listen(sock, 5) == -1)
	{
		cout << "failed listening." << endl;
		return;
	}

	while (1)
	{
		int size = sizeof(addr);
		SOCKET client_sock = accept(sock, (SOCKADDR*)&addr, &size);
		cout << "connected client:" << inet_ntoa(addr.sin_addr) << endl;
		CreateThread(0, 0, ThreadFunc, (void*)client_sock, 0, 0);
		Sleep(1);
	}
	closesocket(sock);

	return;
}

