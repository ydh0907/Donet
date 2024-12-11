#pragma once
#include "config.h"

static class PacketUtility {
public:
	template <typename T>
	static int Read(char* buffer, int offset, int size, T& value) {
		if (size - offset >= sizeof(T)) {
			::memcpy(&value, buffer + offset, sizeof(T));
			return sizeof(T);
		}
		return -1;
	}
	template <typename T>
	static int Write(char* buffer, int offset, int size, T& value) {
		if (size - offset >= sizeof(T)) {
			::memcpy(buffer + offset, &value, sizeof(T));
			return sizeof(T);
		}
		return -1;
	}
	template <typename T>
	static int Write(char* buffer, int offset, int size, T&& value) {
		if (size - offset >= sizeof(T)) {
			::memcpy(buffer + offset, &value, sizeof(T));
			return sizeof(T);
		}
		return -1;
	}
};