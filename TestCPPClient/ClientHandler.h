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
	테스트 프로토콜 타입
	test0: 문자열
	test1: 소수점 4바이트
	test2: 소수점 8바이트
	test3: 정수 1바이트
	test4: 정수 2바이트
	test5: 정수 4바이트
	test6: 정수 8바이트
	*/
	bool rfReqTest(string test0, float test1, double test2, uint8_t test3, uint16_t test4, uint32_t test5, uint64_t test6);

	/*	recv func
	테스트 프로토콜에 대한 응답
	result: 성공여부, 0이면 성공
	msg: 에러 메시지
	*/
	bool rfAckTest(uint8_t result, string msg);
};

