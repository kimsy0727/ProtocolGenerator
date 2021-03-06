#include "../CommmonCPP/stdafx.h"
#include "../CommmonCPP/NetPeer.h"
#include "../CommmonCPP/NetPeer.cpp"
#include "../CommmonCPP/NetBuffer.h"
#include "../CommmonCPP/NetBuffer.cpp"
#include "ProtocolHandler.h"


bool ProtocolHandler::Process(string buffer){
	unsigned char* decryptedPacket = mpPeer->RecvData(buffer);
	NetBuffer recvBuffer{decryptedPacket};
	uint32_t nProtocolNum = 0;
	recvBuffer.GetUInt32(nProtocolNum);
	bool bProcess = (this->*mFuncList[nProtocolNum])(&recvBuffer);
	if(!bProcess)
	{
		 return false;
	}

	return true;
}
/*	parser func*/
bool ProtocolHandler::ppAckResult(NetBuffer* pBuff)
{
	uint8_t result;
	string msg;

	pBuff->GetUInt8(result);
	pBuff->GetString(msg);

	return rfAckResult(result, msg);
}

/*	parser func*/
bool ProtocolHandler::ppReqTest(NetBuffer* pBuff)
{
	string test0;
	float test1;
	double test2;
	uint8_t test3;
	uint16_t test4;
	uint32_t test5;
	uint64_t test6;

	pBuff->GetString(test0);
	pBuff->GetFloat(test1);
	pBuff->GetDouble(test2);
	pBuff->GetUInt8(test3);
	pBuff->GetUInt16(test4);
	pBuff->GetUInt32(test5);
	pBuff->GetUInt64(test6);

	return rfReqTest(test0, test1, test2, test3, test4, test5, test6);
}

/*	parser func*/
bool ProtocolHandler::ppAckTest(NetBuffer* pBuff)
{
	uint8_t result;
	string msg;

	pBuff->GetUInt8(result);
	pBuff->GetString(msg);

	return rfAckTest(result, msg);
}

/*	send func
	디폴트 응답 프로토콜 
	result: 성공여부, 0이면 성공
	msg: 에러 메시지
*/
bool ProtocolHandler::sfAckResult(uint8_t result, string msg)
{
	NetBuffer buffer;
	buffer.AddUInt32(PROTOCOLS::PPACKRESULT);
	buffer.AddUInt8(result);
	buffer.AddString(msg);

	return mpPeer->SendData(&buffer);
}

/*	send func
	테스트 프로토콜 타입 
	test0: 문자열
	test1: 소수점 4바이트
	test2: 소수점 8바이트
	test3: 정수 1바이트
	test4: 정수 2바이트
	test5: 정수 4바이트
	test6: 정수 8바이트
*/
bool ProtocolHandler::sfReqTest(string test0, float test1, double test2, uint8_t test3, uint16_t test4, uint32_t test5, uint64_t test6)
{
	NetBuffer buffer;
	buffer.AddUInt32(PROTOCOLS::PPREQTEST);
	buffer.AddString(test0);
	buffer.AddFloat(test1);
	buffer.AddDouble(test2);
	buffer.AddUInt8(test3);
	buffer.AddUInt16(test4);
	buffer.AddUInt32(test5);
	buffer.AddUInt64(test6);

	return mpPeer->SendData(&buffer);
}

/*	send func
	테스트 프로토콜에 대한 응답 
	result: 성공여부, 0이면 성공
	msg: 에러 메시지
*/
bool ProtocolHandler::sfAckTest(uint8_t result, string msg)
{
	NetBuffer buffer;
	buffer.AddUInt32(PROTOCOLS::PPACKTEST);
	buffer.AddUInt8(result);
	buffer.AddString(msg);

	return mpPeer->SendData(&buffer);
}


ProtocolHandler::ProtocolHandler(NetPeer* pPeer)
{
	mpPeer = pPeer;
	mFuncList[PROTOCOLS::PPACKRESULT] = &ProtocolHandler::ppAckResult;
	mFuncList[PROTOCOLS::PPREQTEST] = &ProtocolHandler::ppReqTest;
	mFuncList[PROTOCOLS::PPACKTEST] = &ProtocolHandler::ppAckTest;
}