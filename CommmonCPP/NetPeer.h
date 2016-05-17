#pragma once
#include "NetBuffer.h"
class NetPeer
{
	SOCKET mSocket;
public:
	NetPeer(SOCKET socket);
	~NetPeer();

	bool SendData(NetBuffer* buffer);
	unsigned char* RecvData(std::string const& packet);

private:
	std::string Base64Encoding(unsigned char const* bytes_to_encode, unsigned int in_len);
	std::string Base64Decoding(std::string const& encoded_string);

	std::string PadRight(std::string const& str, size_t s);
	std::string PadLeft(std::string const& str, size_t s);

	std::string ByteArrayToBinary(unsigned char* byteBuf);
	unsigned char* BinaryToByteArray(std::string const& strBinary);

};

