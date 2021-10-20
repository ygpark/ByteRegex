// dllmain.cpp : DLL 애플리케이션의 진입점을 정의합니다.
#include "pch.h"
#include <iostream>
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

ByteRegex* byteRegex = NULL;

extern "C" MY_DECLSPEC void byteregex_init()
{
    if (byteRegex == NULL)
    {
        byteRegex = new ByteRegex();
    }
    else 
    {
        byteregex_free();
        byteregex_init();
    }
}

extern "C" MY_DECLSPEC void byteregex_init_with_pattern(char *pattern, int size)
{
    if (byteRegex == NULL)
    {
        byteRegex = new ByteRegex(pattern, size);
    }
    else
    {
        byteregex_free();
        byteregex_init();
    }
}

extern "C" MY_DECLSPEC void byteregex_free()
{
    if (byteRegex != NULL)
    {
        delete byteRegex;
        byteRegex = NULL;
    }
}

extern "C" MY_DECLSPEC bool byteregex_compile(char* pattern, unsigned int size)
{
    if (byteRegex == NULL) 
    {
        return false;
    }

    byteRegex->Compile(pattern, size);
    //byteRegex->Debug();
    return true;
}

extern "C" MY_DECLSPEC int byteregex_matches(char* buffer, int length)
{
    return byteRegex->Matches(buffer, length);
}

extern "C" MY_DECLSPEC int byteregex_get_matches(int* indexArray, int& length)
{
    int *matches = byteRegex->GetMatches();
    int matchesSize = byteRegex->GetMatchesSize();
    if (length < matchesSize) {
        length = matchesSize;
        return -1;
    }

    memcpy(indexArray, matches, matchesSize);
    length = matchesSize;
    return 0;
}

extern "C" MY_DECLSPEC int byteregex_matches_get_matches_count()
{
    return byteRegex->GetMatchesSize();
}
