#include "pch.h"

#include "ByteRegexCLR.h"

namespace ByteRegexCLR
{

	inline ByteRegex::ByteRegex()
	{
		byteRegex = new ::ByteRegex::ByteRegex();
	}
}