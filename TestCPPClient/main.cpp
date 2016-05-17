// TestCPPClient.cpp : Defines the entry point for the console application.
//
#include "..\CommmonCPP\stdafx.h"
#include "ClientHandler.h"

DWORD WINAPI ThreadFunc(void* p)
{
	SOCKET sock = (SOCKET)p;

	while (1)
	{
		_PACKET_STRUCT packet = { 0 };
		int n = recv(sock, (char*)&packet, 4, 0);
		int total = packet.size;

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
		ProtocolHandler* handler = new ClientHandler(peer);
		string received_packet = string((char*)packet.buffer);
		if (!handler->Process(received_packet))
		{
			cout << "error process packet." << endl;
			delete peer;
			delete handler;
			return 0;
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
	addr.sin_addr.s_addr = inet_addr("127.0.0.1");
	if (connect(sock, (SOCKADDR*)&addr, sizeof(addr)) == -1)
	{
		cout << "failed connect." << endl;
		return;
	}

	// create recv thread
	CreateThread(0, 0, ThreadFunc, (void*)sock, 0, 0);

	// send to server
	while (1)
	{
		char inputString[256];
		scanf("%s", inputString);
		if (strcmp(inputString, "send") == 0)
		{
			NetPeer* peer = new NetPeer(sock);
			ProtocolHandler* handler = new ClientHandler(peer);
			if (!handler->sfReqTest("test", 0.23f, 0.45, 23, 500, 30000, 200000000))
			{
				cout << "failed send packet." << endl;
				return;
			}
		}
	}

	return;

}