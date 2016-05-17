#pragma once
#define WIN32_LEAN_AND_MEAN
#include <iostream>
#include <string>
#include <stdint.h>
#include <iomanip>
#include <bitset>
#include <Windows.h>
#include <WinSock2.h>
#pragma comment(lib, "ws2_32.lib")
using namespace std;

#pragma pack (1)
typedef struct _PACKET_STRUCT {
	int size;
	unsigned char buffer[4096];
}_PACKET_STRUCT;