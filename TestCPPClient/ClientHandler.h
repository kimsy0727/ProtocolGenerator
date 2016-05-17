#pragma once
#include "ProtocolHandler.h"
class ClientHandler :
	public ProtocolHandler
{
public:
	ClientHandler(NetPeer* peer);
	~ClientHandler();

	bool rfAckResult(uint8_t result, string msg);

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
	bool rfReqTest(string test0, float test1, double test2, uint8_t test3, uint16_t test4, uint32_t test5, uint64_t test6);

	/*	recv func
	�׽�Ʈ �������ݿ� ���� ����
	result: ��������, 0�̸� ����
	msg: ���� �޽���
	*/
	bool rfAckTest(uint8_t result, string msg);
};

