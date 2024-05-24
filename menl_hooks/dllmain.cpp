// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"

#include <thread>

#pragma comment(lib, "libMinHook.x86.lib")
#include "MinHook.h"

#include "main.h"
#include "utils.h"

// Globals
std::thread g_thread;
HMODULE g_base;

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        if (!g_thread.joinable()) {
            DebugWrite(L"opening thread");
            g_thread = std::thread(MainThread);
            g_thread.detach();
        }
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        MH_Uninitialize();
        if (g_thread.joinable()) {
            g_thread.join();
        }
        break;
    }
    return TRUE;
}

void MainThread() {
    g_base = GetModuleHandleA(NULL);

    MH_STATUS status = MH_Initialize();
    if (status != MH_OK) {
        std::string sStatus = MH_StatusToString(status);
        std::wstring sTemp = L"MinHook Init failed, Status: " + std::wstring(sStatus.begin(), sStatus.end());

        DebugWrite(sTemp.c_str());
    }

    if (MH_CreateHook(reinterpret_cast<BYTE*>(reinterpret_cast<uintptr_t>(g_base) + 0xDC6A70), reinterpret_cast<BYTE*>(&StaticLevelLoadHook), nullptr) != MH_OK) {
        std::string sStatus = MH_StatusToString(status);
        std::wstring sTemp = L"MinHook CreateHook failed, Status: " + std::wstring(sStatus.begin(), sStatus.end());

        DebugWrite(sTemp.c_str());
    }

    MH_EnableHook(MH_ALL_HOOKS);
}



__declspec(naked) int __fastcall StaticLevelLoadGate(void* this_, void* edx_, void* levelInfo, int unk, void* unk2) {
    __asm {
        nop; nop; nop; nop; nop; nop; nop; // overwritten bytes
        nop; nop; nop; nop; nop; // jmp
    }
}

int __fastcall StaticLevelLoadHook(void* this_, void* edx_, void* levelInfo, int unk, void* unk2) {
    wchar_t* wc = *reinterpret_cast<wchar_t**>(reinterpret_cast<char*>(levelInfo) + 0x1C);
    std::wstring ws(wc);
    std::string name(ws.begin(), ws.end());

    //g_currentLevel = name;

    std::wstring msg = L"static load started, level: " + std::wstring(name.begin(), name.end());
    DebugWrite(msg.c_str());

    /*SetPausedState(true);

    if (name == "TdMainMenu")
        g_waitForSubLevel.clear();

    for (LevelStreamData& d : g_levelStreamData) {
        d.Reset();
    }*/

    int retVal = StaticLevelLoadGate(this_, edx_, levelInfo, unk, unk2);

    //SetPausedState(false);
    DebugWrite(L"static load finished");
    return retVal;
}