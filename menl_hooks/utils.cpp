#include "pch.h"

void DebugWrite(LPCWSTR msg) {
    std::wstring newMsg = L"[MELRT] " + (std::wstring)msg;

    OutputDebugString(newMsg.c_str());
}