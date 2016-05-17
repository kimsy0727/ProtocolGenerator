#pragma once
#include "stdafx.h"

class NetBuffer
{
	unsigned char* mBuffer;
	unsigned int mAddOffset = 0;
	unsigned int mGetOffset = 0;
public:
	NetBuffer();
	NetBuffer(unsigned char* buffer);
	~NetBuffer();

	unsigned char* GetBuffer() { return mBuffer; }

	void GetString(string& sStr);
	void AddString(string sStr);

	void GetFloat(float& fFloat);
	void AddFloat(float fFloat);

	void GetDouble(double& dDouble);
	void AddDouble(double dDouble);

	void GetUInt8(uint8_t& nUint);
	void GetUInt16(uint16_t& nUint);
	void GetUInt32(uint32_t& nUint);
	void GetUInt64(uint64_t& nUint);

	void AddUInt8(uint8_t nUint);
	void AddUInt16(uint16_t nUint);
	void AddUInt32(uint32_t nUint);
	void AddUInt64(uint64_t nUint);
};

