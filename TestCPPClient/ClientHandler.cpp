#include "../CommmonCPP/stdafx.h"
#include "ClientHandler.h"

ClientHandler::ClientHandler(NetPeer* peer) : ProtocolHandler(peer)
{
}


ClientHandler::~ClientHandler()
{
}

bool ClientHandler::rfAckResult(uint8_t result, string msg)
{
	return true;
}

/*	recv func
�׽�Ʈ �������� Ÿ��
test0: ���ڿ�
test1: �Ҽ��� 4����Ʈ
test2: �Ҽ��� 8����Ʈ
test3: ���� 1����Ʈ
test4: ���� 2����Ʈ
test5: ���� 4����Ʈ
test6: ���� 8����Ʈ
*/
bool ClientHandler::rfReqTest(string test0, float test1, double test2, uint8_t test3, uint16_t test4, uint32_t test5, uint64_t test6)
{
	return true;
}

/*	recv func
�׽�Ʈ �������ݿ� ���� ����
result: ��������, 0�̸� ����
msg: ���� �޽���
*/
bool ClientHandler::rfAckTest(uint8_t result, string msg)
{
	printf("received ack packet1:%d\n", result);
	printf("received ack packet2:%s\n", (char*)msg.c_str());

	return true;
}
