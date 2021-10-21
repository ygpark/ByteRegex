// dllmain.cpp : DLL 애플리케이션의 진입점을 정의합니다.
#include "pch.h"
//#include <iostream>
#include <vector>
#include "ByteRegex.h"


BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

extern "C" MY_DECLSPEC void* byteregex_init()
{
    return new ByteRegex();
}

extern "C" MY_DECLSPEC void* byteregex_init_by_pattern(char* pattern, int size)
{
        return new ByteRegex(pattern, size);
}

extern "C" MY_DECLSPEC void byteregex_free(void* handle)
{
    ByteRegex* regex = (ByteRegex*)handle;
    delete regex;
}

extern "C" MY_DECLSPEC void byteregex_compile(void* handle, char* pattern, int size)
{
    ByteRegex* regex = (ByteRegex*)handle;
    regex->Compile(pattern, size);
}

extern "C" MY_DECLSPEC void byteregex_matches(void* handle, char* buffer, int size)
{
    ByteRegex* regex = (ByteRegex*)handle;
    regex->Matches(buffer, size);
    //std::cout << "size: " << size << std::endl;
}

extern "C" MY_DECLSPEC int byteregex_get_matches(void* handle, int* indexArray, int& size)
{
    ByteRegex* regex = (ByteRegex*)handle;

    int *matches = regex->GetMatches();
    int matchesSize = regex->GetMatchesSize();
    if (size < matchesSize) {
        size = matchesSize;
        return -1;
    }

    memcpy(indexArray, matches, matchesSize);
    size = matchesSize;
    return 0;
}

extern "C" MY_DECLSPEC int byteregex_get_matches_count(void* handle)
{
    ByteRegex* regex = (ByteRegex*)handle;
    return regex->GetMatchesSize();
}
