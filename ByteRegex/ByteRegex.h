#pragma once
#include <vector>


namespace ByteRegex
{
    enum class CommandCode {
        NONE = 1,
        EQUAL = 2,
        SINGLE = 3,
        SINGLE_ANY = 4,
        SINGLE_RANGE = 5,
    };

    struct Command {
        CommandCode code;
        unsigned char value;
        unsigned char end;
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
        ByteRegex(unsigned char* pattern, int size);
        ~ByteRegex();

        void Compile(unsigned char* pattern, int size);
        int Matches(unsigned char* buffer, int size);
        int* GetMatches();
        int GetMatchesSize();

        void Debug();

    private:
    };
}