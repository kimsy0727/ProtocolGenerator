#include "../CommmonCPP/stdafx.h"
#include "ServerHandler.h"

ServerHandler::ServerHandler(NetPeer* peer) : ProtocolHandler(peer)
{
}


ServerHandler::~ServerHandler()
{
}

bool ServerHandler::rfAckResult(uint8_t result, string msg)
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
bool ServerHandler::rfReqTest(string test0, float test1, double test2, uint8_t test3, uint16_t test4, uint32_t test5, uint64_t test6)
{
	printf("received req packet1:%s\n", (char*)test0.c_str());
	printf("received req packet2:%f\n", test1);
	printf("received req packet3:%f\n", test2);
	printf("received req packet4:%d\n", test3);
	printf("received req packet5:%d\n", test4);
	printf("received req packet6:%d\n", test5);
	printf("received req packet7:%d\n", test6);

	sfAckTest(0, test0 + to_string(test1) + to_string(test2) + to_string(test3) + to_string(test4) + to_string(test5) + to_string(test6));
	return true;
}

/*	recv func
�׽�Ʈ �������ݿ� ���� ����
result: ��������, 0�̸� ����
msg: ���� �޽���
*/
bool ServerHandler::rfAckTest(uint8_t result, string msg)
{
	return true;
}