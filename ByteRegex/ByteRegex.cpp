#include "pch.h"
#include "ByteRegex.h"
#include <string>
#include <iostream>
#include <array>

namespace ByteRegex
{
    ByteRegex::ByteRegex()
    {

    }


    ByteRegex::ByteRegex(unsigned char* pattern, int size)
    {
        Compile(pattern, size);
    }

    ByteRegex::~ByteRegex()
    {

    }

    void ByteRegex::Compile(unsigned char* pattern, int size)
    {
        _commands.clear();

        for (int i = 0; i < size; i++)
        {
            //std::cout << "---> " << (int)pattern[i] << std::endl;
            if (pattern[i] == '[')//OR
            {
                i++;

                Command single;
                single.code = CommandCode::SINGLE;

                while (pattern[i] != ']')
                {

                    if (pattern[i - 1] == '-')
                    {
                        single.Nodes.pop_back();// '-' 지우기
                        Command lastCmd = single.Nodes.back();// '-' 왼쪽 커맨드 꺼내기
                        single.Nodes.pop_back();// '-' 왼쪽 커맨드 삭제
                        lastCmd.code = CommandCode::SINGLE_RANGE;
                        lastCmd.end = pattern[i];
                        single.Nodes.push_back(lastCmd);
                    }
                    else
                    {
                        Command cmd;
                        cmd.code = CommandCode::EQUAL;
                        cmd.value = pattern[i];
                        cmd.end = 0;
                        single.Nodes.push_back(cmd);
                    }
                    i++;
                }

                //single_any가 있는지 검사
                bool hasAny = false;
                for (int nodeIdx = 0; nodeIdx < (int)single.Nodes.size(); nodeIdx++)
                {
                    Command node = single.Nodes[nodeIdx];
                    if (node.code == CommandCode::SINGLE_RANGE
                        && node.value == 0x00
                        && node.end == 0xFF)
                    {
                        hasAny = true;
                        break;
                    }
                }

                //push
                if (hasAny)
                {
                    Command cmdAny;
                    cmdAny.code = CommandCode::SINGLE_ANY;
                    _commands.push_back(cmdAny);
                }
                else
                {
                    _commands.push_back(single);
                }


            }
            else if (pattern[i] == '{')//Count
            {
                i++;
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
                cmd.code = CommandCode::EQUAL;
                cmd.value = pattern[i];
                _commands.push_back(cmd);
            }
        }
        _isCompiled = true;
        //this->Debug();
    }

    int ByteRegex::Matches(unsigned char* buffer, int size)
    {
        //std::cout << "[debug] int ByteRegex::Matches(byte* buffer, int size) - begin" << std::endl;

        int commandSize = (int)_commands.size();
        //bool skipStop;
        _matches.clear();

        if (!_isCompiled)
        {
            return -1;
        }

        if ((int)_commands.size() > size)
        {
            return 0;
        }

        for (int dataIdx = 0; dataIdx < size; dataIdx++)
        {
            int remain = size - dataIdx;
            if (remain < commandSize)
                break;

            int hit = 0;
            int hitSum = 0;
            /*int skip = 0;
            skipStop = false;*/
            for (int cmdIdx = 0; cmdIdx < commandSize; cmdIdx++)
            {
                hit = 0;

                /*if (cmdIdx != 0
                    && skipStop == false
                    && _commands[0].value != buffer[dataIdx + cmdIdx])
                {
                    skip++;
                    std::cout << skip << std::endl;
                }
                else if (cmdIdx != 0
                    && skipStop == false
                    && _commands[0].value == buffer[dataIdx + cmdIdx])
                {
                    skipStop = true;
                }*/

                if (_commands[cmdIdx].code == CommandCode::EQUAL
                    && buffer[dataIdx + cmdIdx] == _commands[cmdIdx].value)
                {
                    hit = 1;
                }
                else if (_commands[cmdIdx].code == CommandCode::SINGLE
                    && _commands[cmdIdx].value <= buffer[dataIdx + cmdIdx] && buffer[dataIdx + cmdIdx] <= _commands[cmdIdx].end)
                {
                    //SINGLE([])의 노드 탐색
                    int nodeSize = (int)_commands[cmdIdx].Nodes.size();
                    for (int nodeIdx = 0; nodeIdx < nodeSize; nodeIdx++)
                    {
                        Command node = _commands[cmdIdx].Nodes[nodeIdx];
                        if (node.code == CommandCode::EQUAL
                            && buffer[dataIdx + cmdIdx] == node.value)
                        {
                            hit = 1;
                            break;
                        }
                        else if (node.code == CommandCode::SINGLE_RANGE
                            && node.value <= buffer[dataIdx + cmdIdx] && buffer[dataIdx + cmdIdx] <= node.end)
                        {
                            hit = 1;
                            break;
                        }
                    }
                }
                else if (_commands[cmdIdx].code == CommandCode::SINGLE_ANY)
                {
                    hit = 1;
                }
                else
                {
                    //no match
                    break;
                }

                hitSum += hit;
            }

            ////skip
            //dataIdx += skip;

            if (hitSum == commandSize)
            {
                _matches.push_back(dataIdx);
            }
        }

        //std::cout << "[debug] int ByteRegex::Matches(byte* buffer, int size) - end" << std::endl;
        return (int)_matches.size();
    }

    int* ByteRegex::GetMatches()
    {
        //std::cout << "_matches.size(): " << _matches.size() << std::endl;
        return _matches.data();
    }

    int ByteRegex::GetMatchesSize()
    {
        return (int)_matches.size();
    }

    void ByteRegex::Debug()
    {
        std::vector<Command>::iterator iter;
        for (iter = _commands.begin(); iter != _commands.end(); iter++)
        {
            std::cout << "(code)" << (int)iter->code << " (value)" << (int)iter->value << " (end)" << (int)iter->end << std::endl;
        }
    }
}