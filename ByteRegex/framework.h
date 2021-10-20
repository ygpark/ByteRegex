#pragma once

#define WIN32_LEAN_AND_MEAN             // 거의 사용되지 않는 내용을 Windows 헤더에서 제외합니다.
// Windows 헤더 파일
#include <windows.h>

#define MY_DECLSPEC __declspec(dllexport)
extern "C" MY_DECLSPEC void byteregex_init();
extern "C" MY_DECLSPEC void byteregex_init_with_pattern(char* pattern, int size);
extern "C" MY_DECLSPEC void byteregex_free();
extern "C" MY_DECLSPEC bool byteregex_compile(char* pattern, unsigned int size);
extern "C" MY_DECLSPEC int byteregex_matches(char* buffer, int length);
extern "C" MY_DECLSPEC int byteregex_get_matches(int* indexArray, int& length);
extern "C" MY_DECLSPEC int byteregex_matches_get_matches_count();