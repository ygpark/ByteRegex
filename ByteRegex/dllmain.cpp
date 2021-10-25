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

extern "C" MY_DECLSPEC void* byteregex_init()
{
    return new ::ByteRegex::ByteRegex();
}

extern "C" MY_DECLSPEC void* byteregex_init_by_pattern(byte *pattern, int size)
{
    //std::cout << "PATTERN : ";
    //for (int i = 0; i < size; i++)
    //{
    //    //std::cout << (int)pattern[i] << "(" << pattern[i] << ")" << " ";
    //    std::cout << (int)pattern[i]  << " ";
    //}
    //std::cout << std::endl;
    return new ::ByteRegex::ByteRegex(pattern, size);
}

extern "C" MY_DECLSPEC void byteregex_free(void *handle)
{
    ::ByteRegex::ByteRegex* regex = (::ByteRegex::ByteRegex*)handle;
    delete regex;
}

extern "C" MY_DECLSPEC void byteregex_compile(void *handle, byte *pattern, int size)
{
    ::ByteRegex::ByteRegex* regex = (::ByteRegex::ByteRegex*)handle;
    regex->Compile(pattern, size);
    regex->Debug();
}

extern "C" MY_DECLSPEC int byteregex_matches(void *handle, byte *input, int size)
{
    //std::cout << "[debug] void byteregex_matches(void *handle, byte *input, int size) - begin " << std::endl;
    ::ByteRegex::ByteRegex* regex = (::ByteRegex::ByteRegex*)handle;
    //std::cout << "[debug] void byteregex_matches(void *handle, byte *input, int size) - end " << std::endl;
    return regex->Matches(input, size);
}

extern "C" MY_DECLSPEC int byteregex_get_matches(void *handle, int *output, int &size)
{
    //std::cout << "[debug] int byteregex_get_matches(void *handle, int *output, int &size) - begin" << std::endl;
    ::ByteRegex::ByteRegex* regex = (::ByteRegex::ByteRegex*)handle;
    int rstSize = regex->GetMatchesSize();
    int* rst = NULL;
    if (rstSize == 0)
    {
        size = 0;
        return 0;
    }
    else if (size < rstSize)
    {
        size = rstSize;
        return -1;
    }

    rst = regex->GetMatches();
    memcpy(output, rst, rstSize);
    //std::cout << "[debug] int byteregex_get_matches(void *handle, int *output, int &size) - end " << std::endl;
    return 0;
}

extern "C" MY_DECLSPEC int byteregex_get_matches_count(void *handle)
{
    ::ByteRegex::ByteRegex* regex = (::ByteRegex::ByteRegex*)handle;
    return regex->GetMatchesSize();
}
