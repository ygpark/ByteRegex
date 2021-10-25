#pragma once

#define WIN32_LEAN_AND_MEAN             // 거의 사용되지 않는 내용을 Windows 헤더에서 제외합니다.
// Windows 헤더 파일
#include <windows.h>

#define MY_DECLSPEC __declspec(dllexport)

#define byte unsigned char

//extern "C" MY_DECLSPEC void byteregex_init();
extern "C" MY_DECLSPEC void* byteregex_init();
extern "C" MY_DECLSPEC void* byteregex_init_by_pattern(byte *pattern, int size);
extern "C" MY_DECLSPEC void byteregex_free(void *handle);
extern "C" MY_DECLSPEC void byteregex_compile(void *handle, byte *pattern, int size);
extern "C" MY_DECLSPEC int byteregex_matches(void *handle, byte *input, int size);
extern "C" MY_DECLSPEC int byteregex_get_matches(void *handle, int *output, int &size);
extern "C" MY_DECLSPEC int byteregex_get_matches_count(void *handle);