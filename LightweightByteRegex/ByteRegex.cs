using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightweightBinRegex
{
    internal enum CommandCode
    {
        Single,
        OR,
        Range
    }






    internal class Command
    {
        public CommandCode code;
        public byte value;
        public byte end;
        public List<Command> Nodes = new List<Command>();


        public override string ToString()
        {
            return String.Format("Code:{0}, value:0x{1:X2}, end:0x{2:X2}", code, (int)value, (int)end);
        }
    }






    public class ByteMatch
    {
        public ByteMatch(int index)
        {
            Index = index;
        }
        public int Index { get; set; }
    }






    public class ByteMatches : List<ByteMatch>
    {

    }






    public class ByteRegex
    {
        bool _isCompiled = false;
        private List<Command> commands = new List<Command>();
        private ByteMatches matches = new ByteMatches();



        public ByteRegex()
        {
            _isCompiled = false;
        }



        public ByteRegex(string pattern)
        {
            Compile(pattern);
        }



        public void Compile(string pattern)
        {
            char[] p = pattern.ToCharArray();
            commands.Clear();

            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] == '[')//OR
                {
                    i++;

                    Command orCmd = new Command();
                    orCmd.code = CommandCode.OR;

                    while (p[i] != ']')
                    {
                        if (p[i - 1] == '-')
                        {
                            orCmd.Nodes.RemoveAt(orCmd.Nodes.Count - 1);// '-' 지우기
                            Command lastCmd = orCmd.Nodes[orCmd.Nodes.Count - 1];// '-' 왼쪽 커맨드 꺼내기
                            orCmd.Nodes.RemoveAt(orCmd.Nodes.Count - 1);// '-' 왼쪽 커맨드 삭제
                            lastCmd.code = CommandCode.Range;
                            lastCmd.end = Convert.ToByte(p[i]);
                            orCmd.Nodes.Add(lastCmd);
                        }
                        else
                        {
                            Command cmd = new Command()
                            {
                                code = CommandCode.Single,
                                value = Convert.ToByte(p[i])
                            };
                            orCmd.Nodes.Add(cmd);
                        }
                        i++;
                    }
                    commands.Add(orCmd);
                    
                }
                else if (p[i] == '{')//Count
                {
                    i++;
                    var sCount = new StringBuilder();
                    int count;
                    Command last;

                    while (p[i] != '}')
                    {
                        sCount.Append(p[i]);
                        i++;
                    }

                    count = int.Parse(sCount.ToString());
                    last = commands[commands.Count - 1];
                    for (int j = 0; j < count - 1; j++)//이미 1개는 들어가 있으니까 count-1회 반복한다.
                    {
                        commands.Add(last);
                    }
                }
                else
                {
                    commands.Add(new Command()
                    {
                        code = CommandCode.Single,
                        value = Convert.ToByte(p[i])
                    });
                }
            }

            _isCompiled = true;
        }

        public void Debug()
        {
            foreach (var item in commands)
            {
                Console.WriteLine(item.ToString());
                foreach (var nodeItem in item.Nodes)
                {
                    Console.WriteLine(nodeItem.ToString());
                }
            } 
        }


        public ByteMatches Matches(byte[] data)
        {

            if (!_isCompiled)
            {
                throw new Exception("패턴이 없습니다.");
            }

            matches.Clear();

            if (commands.Count > data.Length)
                return matches;

            for (int dataIdx = 0; dataIdx < data.Length; dataIdx++)
            {
                // 0 1 2
                // count = 3
                int remain = data.Length - dataIdx;
                if (remain < commands.Count)
                    break;

                int hitSum = 0;
                for (int cmdIdx = 0; cmdIdx < commands.Count; cmdIdx++)
                {
                    bool found = findNode(data[dataIdx + cmdIdx], commands[cmdIdx]);
                    if (found)
                    {
                        hitSum++;
                    }
                    else
                    {
                        break;//하나라도 찾지 못하면 중단한다.
                    }
                }

                if (hitSum == commands.Count)
                {
                    matches.Add(new ByteMatch(dataIdx));
                }
            }

            return matches;
        }

        private bool findNode(byte value, Command command)
        {
            switch (command.code)
            {
                case CommandCode.Single:
                    if (value == command.value)
                    {
                        return true;
                    }
                    break;

                case CommandCode.Range:
                    if (command.value <= value && value <= command.end)
                    {
                        return true;
                    }
                    break;

                case CommandCode.OR:
                    foreach (var item in command.Nodes)
                    {
                        if (findNode(value, item))
                            return true;
                    }
                    return false;
                default:
                    break;
            }
            return false;
        }
    }
}
