#include "stdafx.h"
#include "NetBuffer.h"

NetBuffer::NetBuffer()
{
	mBuffer = new unsigned char[256];
	memset(mBuffer, '\0', 256);
}

NetBuffer::NetBuffer(unsigned char* buffer)
{
	mBuffer = new unsigned char[256];
	memcpy(mBuffer, buffer, 255);
}

NetBuffer::~NetBuffer()
{
	delete mBuffer;
}

void NetBuffer::GetString(string& sStr) {
	size_t strSize = mBuffer[mGetOffset++];
	char* tmpStr = new char[strSize+1];
	memset(tmpStr, '\0', strSize + 1);
	strncpy(tmpStr, (char*)&mBuffer[mGetOffset], strSize);
	sStr = tmpStr;
	mGetOffset += strSize;

	delete tmpStr;
	return;
}
void NetBuffer::AddString(string sStr) {
	mBuffer[mAddOffset++] = (char)sStr.size();
	sStr.copy((char*)&mBuffer[mAddOffset], sStr.size());
	mAddOffset += sStr.size();
	return;
}

void NetBuffer::GetFloat(float& fFloat) {
	memcpy(&fFloat, &mBuffer[mGetOffset], sizeof(float));
	mGetOffset += sizeof(float);
	return;
}
void NetBuffer::AddFloat(float fFloat) {
	memcpy(&mBuffer[mAddOffset], &fFloat, sizeof(float));
	mAddOffset += sizeof(float);
	return;
}

void NetBuffer::GetDouble(double& dDouble) {
	memcpy(&dDouble, &mBuffer[mGetOffset], sizeof(double));
	mGetOffset += sizeof(double);
	return;
}
void NetBuffer::AddDouble(double dDouble) {
	memcpy(&mBuffer[mAddOffset], &dDouble, sizeof(double));
	mAddOffset += sizeof(double);
	return;
}

void NetBuffer::GetUInt8(uint8_t& nUint) {
	memcpy(&nUint, &mBuffer[mGetOffset], sizeof(uint8_t));
	mGetOffset += sizeof(uint8_t);
	return;
}
void NetBuffer::GetUInt16(uint16_t& nUint) {
	memcpy(&nUint, &mBuffer[mGetOffset], sizeof(uint16_t));
	mGetOffset += sizeof(uint16_t);
	return;
}
void NetBuffer::GetUInt32(uint32_t& nUint) {
	memcpy(&nUint, &mBuffer[mGetOffset], sizeof(uint32_t));
	mGetOffset += sizeof(uint32_t);
	return;
}
void NetBuffer::GetUInt64(uint64_t& nUint) {
	memcpy(&nUint, &mBuffer[mGetOffset], sizeof(uint64_t));
	mGetOffset += sizeof(uint64_t);
	return;
}

void NetBuffer::AddUInt8(uint8_t nUint) {
	memcpy(&mBuffer[mAddOffset], &nUint, sizeof(uint8_t));
	mAddOffset += sizeof(uint8_t);
	return;
}
void NetBuffer::AddUInt16(uint16_t nUint) {
	memcpy(&mBuffer[mAddOffset], &nUint, sizeof(uint16_t));
	mAddOffset += sizeof(uint16_t);
	return;
}
void NetBuffer::AddUInt32(uint32_t nUint) {
	memcpy(&mBuffer[mAddOffset], &nUint, sizeof(uint32_t));
	mAddOffset += sizeof(uint32_t);
	return;
}
void NetBuffer::AddUInt64(uint64_t nUint) {
	memcpy(&mBuffer[mAddOffset], &nUint, sizeof(uint64_t));
	mAddOffset += sizeof(uint64_t);
	return;
}