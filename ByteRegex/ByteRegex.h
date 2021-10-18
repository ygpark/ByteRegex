#pragma once
#include <vector>

enum CommandCode {
    Single,
    OR,
    Range
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
    std::vector<Command> commands;
    int commandsCount = 0;
    std::vector<int> matches;

public:
    ByteRegex();
    ByteRegex(char* pattern);

    void Compile(char* pattern);
};

