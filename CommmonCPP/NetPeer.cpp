#include "NetPeer.h"

NetPeer::NetPeer(SOCKET socket)
{
	mSocket = socket;
}


NetPeer::~NetPeer()
{
}


static const std::string base64_chars =
"ABCDEFGHIJKLMNOPQRSTUVWXYZ"
"abcdefghijklmnopqrstuvwxyz"
"0123456789+/";

static inline bool is_base64(unsigned char c) {
	return (isalnum(c) || (c == '+') || (c == '/'));
}

std::string NetPeer::PadRight(std::string const& str, size_t s)
{
	if (str.size() < s)
		return str + std::string(s - str.size(), '0');
	else
		return str;
}

std::string NetPeer::PadLeft(std::string const& str, size_t s)
{
	if (str.size() < s)
		return std::string(s - str.size(), '0') + str;
	else
		return str;
}

std::string NetPeer::ByteArrayToBinary(unsigned char* byteBuf)
{
	string binStr = "";
	for (int i = 0; i < 256; ++i)
	{
		char tmpStr[16];
		_itoa(byteBuf[i], tmpStr, 2);
		string strByte = tmpStr;
		string strPad = PadLeft(strByte, 8);
		binStr.append(strPad);
	}

	return binStr;
}

unsigned char* NetPeer::BinaryToByteArray(std::string const& strBinary)
{
	unsigned char retStr[4096] = { '\0', };
	for (int i = 0; i < strBinary.length(); i += 8)
	{
		string test = strBinary.substr(i, 8).c_str();
		char *end;
		retStr[i/8] = strtol(test.c_str(), &end, 2);
	}

	return retStr;
}

std::string NetPeer::Base64Encoding(unsigned char const* bytes_to_encode, unsigned int in_len) {
	std::string ret;
	int i = 0;
	int j = 0;
	unsigned char char_array_3[3];
	unsigned char char_array_4[4];

	while (in_len--) {
		char_array_3[i++] = *(bytes_to_encode++);
		if (i == 3) {
			char_array_4[0] = (char_array_3[0] & 0xfc) >> 2;
			char_array_4[1] = ((char_array_3[0] & 0x03) << 4) + ((char_array_3[1] & 0xf0) >> 4);
			char_array_4[2] = ((char_array_3[1] & 0x0f) << 2) + ((char_array_3[2] & 0xc0) >> 6);
			char_array_4[3] = char_array_3[2] & 0x3f;

			for (i = 0; (i <4); i++)
				ret += base64_chars[char_array_4[i]];
			i = 0;
		}
	}

	if (i)
	{
		for (j = i; j < 3; j++)
			char_array_3[j] = '\0';

		char_array_4[0] = (char_array_3[0] & 0xfc) >> 2;
		char_array_4[1] = ((char_array_3[0] & 0x03) << 4) + ((char_array_3[1] & 0xf0) >> 4);
		char_array_4[2] = ((char_array_3[1] & 0x0f) << 2) + ((char_array_3[2] & 0xc0) >> 6);
		char_array_4[3] = char_array_3[2] & 0x3f;

		for (j = 0; (j < i + 1); j++)
			ret += base64_chars[char_array_4[j]];

		while ((i++ < 3))
			ret += '=';

	}

	return ret;

}

std::string NetPeer::Base64Decoding(std::string const& encoded_string) {
	int in_len = encoded_string.size();
	int i = 0;
	int j = 0;
	int in_ = 0;
	unsigned char char_array_4[4], char_array_3[3];
	std::string ret;

	while (in_len-- && (encoded_string[in_] != '=') && is_base64(encoded_string[in_])) {
		char_array_4[i++] = encoded_string[in_]; in_++;
		if (i == 4) {
			for (i = 0; i <4; i++)
				char_array_4[i] = base64_chars.find(char_array_4[i]);

			char_array_3[0] = (char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4);
			char_array_3[1] = ((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2);
			char_array_3[2] = ((char_array_4[2] & 0x3) << 6) + char_array_4[3];

			for (i = 0; (i < 3); i++)
				ret += char_array_3[i];
			i = 0;
		}
	}

	if (i) {
		for (j = i; j <4; j++)
			char_array_4[j] = 0;

		for (j = 0; j <4; j++)
			char_array_4[j] = base64_chars.find(char_array_4[j]);

		char_array_3[0] = (char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4);
		char_array_3[1] = ((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2);
		char_array_3[2] = ((char_array_4[2] & 0x3) << 6) + char_array_4[3];

		for (j = 0; (j < i - 1); j++) ret += char_array_3[j];
	}

	return ret;
}

bool NetPeer::SendData(NetBuffer* buffer)
{
	// 바이너리로 변경하자
	unsigned char* byteBuf = buffer->GetBuffer();
	string strBinary = ByteArrayToBinary(byteBuf);
	//printf("%s\n", (char*)strBinary.c_str());
	// 암호화 시키자
	string encryptedPacket = Base64Encoding((unsigned char*)strBinary.c_str(), strBinary.length());
	// 보내자
	_PACKET_STRUCT packet = { 0 };
	packet.size = 4096+sizeof(packet.size);
	memcpy(packet.buffer, (unsigned char*)encryptedPacket.c_str(), 4096);
	int result = send(mSocket, (char*)&packet, sizeof(packet), 0);
	if (result == SOCKET_ERROR) {
		printf("failed send data with error.%d", WSAGetLastError());
		closesocket(mSocket);
		WSACleanup();
		return false;
	}
	//printf("bytes sent:%d", result);

	return true;
}

unsigned char* NetPeer::RecvData(std::string const& packet)
{
	//printf("%s\n", (char*)packet.c_str());
	// 복호화 하자
	std::string decryptedPacket = Base64Decoding(packet);
	//printf("%s\n", (char*)decryptedPacket.c_str());
	// 바이너리 풀자
	unsigned char* buffer = BinaryToByteArray(decryptedPacket);

	return buffer;
}