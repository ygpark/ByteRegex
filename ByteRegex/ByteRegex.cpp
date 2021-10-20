#include "pch.h"
#include "ByteRegex.h"
#include <string>
#include <iostream>



ByteRegex::ByteRegex()
{
    //matchesArrayCapa = 10 * 1024 * 1024;
    matchesArrayCapa = 64;
    matchesArraySize = 0;
    matchesArray = new int[matchesArrayCapa];
}

ByteRegex::ByteRegex(char* pattern, int size)
{
    //matchesArrayCapa = 10 * 1024 * 1024;
    matchesArrayCapa = 64;
    matchesArraySize = 0;
    matchesArray = new int[matchesArrayCapa];
	Compile(pattern, size);
}

ByteRegex::~ByteRegex()
{
    delete[] matchesArray;
}

void ByteRegex::Compile(char* pattern, int size)
{
	_commands.clear();
    
    for (size_t i = 0; i < size; i++)
    {
        if (pattern[i] == '[')//OR
        {
            i++;

            //Command *orCmd = new Command();
            Command orCmd;
            orCmd.code = CommandCode::OR;

            while (pattern[i] != ']')
            {
                if (pattern[i - 1] == '-')
                {
                    orCmd.Nodes.pop_back();// '-' 지우기
                    Command lastCmd = orCmd.Nodes.back();// '-' 왼쪽 커맨드 꺼내기
                    orCmd.Nodes.pop_back();// '-' 왼쪽 커맨드 삭제
                    lastCmd.code = CommandCode::RANGE;
                    lastCmd.end = pattern[i];
                    orCmd.Nodes.push_back(lastCmd);
                }
                else
                {
                    Command cmd;
                    cmd.code = CommandCode::SINGLE;
                    cmd.value = pattern[i];
                    orCmd.Nodes.push_back(cmd);
                }
                i++;
            }
            _commands.push_back(orCmd);

        }
        else if (pattern[i] == '{')//Count
        {
            i++;
            //var sCount = new StringBuilder();
            std::string sCount;
            int count;
            Command last;

            while (pattern[i] != '}')
            {
                sCount.push_back(pattern[i]);
                i++;
            }

            count = stoi(sCount);
            last = _commands.back();
            for (int j = 0; j < count - 1; j++)//이미 1개는 들어가 있으니까 count-1회 반복한다.
            {
                _commands.push_back(last);
            }
        }
        else
        {
            Command cmd;
            cmd.code = CommandCode::SINGLE;
            cmd.value = pattern[i];
            _commands.push_back(cmd);
        }
    }
    _isCompiled = true;
    //commandsCount = commands.Count;
}

int ByteRegex::Matches(char* buffer, int bufferSize)
{
    int commandSize = (int)_commands.size();
    matchesArraySize = 0;
    

    if (!_isCompiled)
    {
        return -1;
    }

    if (_commands.size() > bufferSize)
    {
        return 0;
    }
    
    //std::cout << "for1: " << bufferSize << std::endl;
    for (int dataIdx = 0; dataIdx < bufferSize; dataIdx++)
    {
        int remain = bufferSize - dataIdx;
        if (remain < commandSize)
            break;

        int hit = 0;
        int hitSum = 0;
        for (int cmdIdx = 0; cmdIdx < commandSize; cmdIdx++)
        {
            hit = 0;
            if (_commands[cmdIdx].code == CommandCode::SINGLE && buffer[dataIdx + cmdIdx] == _commands[cmdIdx].value)
            {
                hit = 1;
            }
            else if (_commands[cmdIdx].code == CommandCode::RANGE && _commands[cmdIdx].value <= buffer[dataIdx + cmdIdx] && buffer[dataIdx + cmdIdx] <= _commands[cmdIdx].end)
            {
                hit = 1;
            }
            else if (_commands[cmdIdx].code == CommandCode::OR && _commands[cmdIdx].value <= buffer[dataIdx + cmdIdx] && buffer[dataIdx + cmdIdx] <= _commands[cmdIdx].end)
            {
            }

            hitSum += hit;
        }

        if (hitSum == commandSize)
        {
            matchesArray[matchesArraySize++] = dataIdx;
            //배열 모자르면 2배 늘리기
            if (matchesArrayCapa <= matchesArraySize)
            {
                matchesArrayCapa *= 2;
                int* tmpArray = new int[matchesArrayCapa];
                memcpy(tmpArray, matchesArray, matchesArraySize);
                delete[] matchesArray;
                matchesArray = tmpArray;
            }
        }
    }

    return matchesArraySize;
}

int* ByteRegex::GetMatches()
{
    return matchesArray;
}

int ByteRegex::GetMatchesSize()
{
    return matchesArraySize;
}

void ByteRegex::Debug()
{
    std::vector<Command>::iterator iter;
    for (iter = _commands.begin(); iter != _commands.end(); iter++)
    {
        std::cout << "Code: " << (int)iter->code << ", value: " << (int)iter->value << std::endl;
    }
}