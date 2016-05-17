#pragma once
#include "ProtocolHandler.h"
class ServerHandler :
	public ProtocolHandler
{
public:
	ServerHandler(NetPeer* peer);
	~ServerHandler();

	bool rfAckResult(uint8_t result, string msg);
	bool rfReqTest(string test0, float test1, double test2, uint8_t test3, uint16_t test4, uint32_t test5, uint64_t test6);
	bool rfAckTest(uint8_t result, string msg);
};

