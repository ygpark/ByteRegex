#pragma once

using namespace System;
using namespace System::Text;

#include "../ByteRegex/ByteRegex.h"

namespace ByteRegexCLR
{

	public ref class ByteRegex
	{
		::ByteRegex::ByteRegex *byteRegex;
		// TODO: 여기에 이 클래스에 대한 메서드를 추가합니다.

	public:
		ByteRegex();

		ByteRegex(String^ pattern)
		{
			array<unsigned char>^ bArray = Encoding::GetEncoding("Latin1")->GetBytes(pattern);
			pin_ptr<unsigned char> bArrayPtr = &bArray[0];
			byteRegex = new ::ByteRegex::ByteRegex(bArrayPtr, pattern->Length);
		}

		int Matches(array<Byte>^ buffer, int size)
		{
			pin_ptr<unsigned char> input = &buffer[0];
			return byteRegex->Matches(input, buffer->Length);
		}

	};
}