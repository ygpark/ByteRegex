#pragma once
#include <vector>

enum class CommandCode {
    NONE = 1,
    EQUAL = 2,
    SINGLE = 3,
    SINGLE_ANY = 4,
    SINGLE_RANGE = 5,
};

struct Command {
    CommandCode code;
    byte value;
    byte end;
    std::vector<Command> Nodes;
};

class ByteRegex
{
private:
    bool _isCompiled = false;
    std::vector<Command> _commands;
    std::vector<int> _matches;

public:
    ByteRegex();
    ByteRegex(byte* pattern, int size);
    ~ByteRegex();

    void Compile(byte* pattern, int size);
    int Matches(byte* buffer, int size);
    int* GetMatches();
    int GetMatchesSize();

    void Debug();

private:
};

