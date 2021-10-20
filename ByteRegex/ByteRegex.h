#pragma once
#include <vector>

enum class CommandCode {
    NONE,
    SINGLE,
    OR,
    RANGE
};

struct Command {
    CommandCode code;
    char value;
    char end;
    std::vector<Command> Nodes;
};


class ByteRegex
{
private:
    bool _isCompiled = false;
    std::vector<Command> _commands;
    std::vector<int> matches;
    int* matchesArray;
    int matchesArraySize;
    int matchesArrayCapa;

public:
    ByteRegex();
    ByteRegex(char* pattern, int size);
    ~ByteRegex();

    void Compile(char* pattern, int size);
    int Matches(char* buffer, int len);
    int* GetMatches();
    int GetMatchesSize();

    void Debug();

private:
};

