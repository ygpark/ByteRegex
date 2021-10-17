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
        public List<Command> Nodes = null;


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
        private int commandsCount = 0;
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
            commandsCount = 0;

            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] == '[')//OR
                {
                    i++;

                    Command orCmd = new Command();
                    orCmd.code = CommandCode.OR;
                    if(orCmd.Nodes == null)
                    {
                        orCmd.Nodes = new List<Command>();
                    }

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
            commandsCount = commands.Count;
        }

        public void Debug()
        {
            foreach (var item in commands)
            {
                Console.WriteLine(item.ToString());
                if(item.Nodes != null)
                {
                    foreach (var nodeItem in item.Nodes)
                    {
                        Console.WriteLine(nodeItem.ToString());
                    }
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

            if (commandsCount > data.Length)
                return matches;

            for (int dataIdx = 0; dataIdx < data.Length; dataIdx++)
            {
                int remain = data.Length - dataIdx;
                if (remain < commandsCount)
                    break;

                int hitSum = 0;
                for (int cmdIdx = 0; cmdIdx < commandsCount; cmdIdx++)
                {
                    bool found = FindNode(data[dataIdx + cmdIdx], commands[cmdIdx]);
                    if (found)
                    {
                        hitSum++;
                    }
                    else
                    {
                        break;//하나라도 찾지 못하면 중단한다.
                    }
                }

                if (hitSum == commandsCount)
                {
                    matches.Add(new ByteMatch(dataIdx));
                    //TODO: 패턴이 매칭되면 그 구간을 건너뛰는 옵션을 구현해 보는건 어떨까?
                    //      CCTV용 패턴은 200짜리같은 긴 것도 있는데.. 한번 매칭된 구간은 건너뛰는게 맞는 것 같다.
                    //      예) dataIdx += commandsCount;
                }
            }

            return matches;
        }

        public ByteMatches MatchesParallel(byte[] data)
        {
            if (!_isCompiled)
            {
                throw new Exception("패턴이 없습니다.");
            }
            object listLock = new object();
            matches.Clear();

            if (commandsCount > data.Length)
                return matches;

            int len = data.Length;
            // <최적화> MaxDegreeOfParallelism = Environment.ProcessorCount / 2 옵션을 설정하면
            // 속도가 가장 빠르고 CPU 사용률도 약 60%정도 유지되었다. 이 옵션을 사용하지 않으면 CPU 사용율은 100%
            Parallel.For( 0, len, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount / 2}, dataIdx => {
                int remain = data.Length - dataIdx;
                if (remain >= commandsCount) //Parallel.For는 break;를 쓸 수 없어서 if문으로 감쌌다.
                {
                    int hitSum = 0;
                    for (int cmdIdx = 0; cmdIdx < commandsCount; cmdIdx++)
                    {
                        bool found = FindNode(data[dataIdx + cmdIdx], commands[cmdIdx]);
                        if (found)
                        {
                            hitSum++;
                        }
                        else
                        {
                            break;//하나라도 찾지 못하면 중단한다.
                        }
                    }

                    if (hitSum == commandsCount)
                    {
                        lock (listLock)
                        {
                            matches.Add(new ByteMatch(dataIdx));
                        }
                    }
                }
            });

            return matches;
        }

        private bool FindNode(byte value, Command command)
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
                    // <최적화 메모> 
                    //  - foreach보다 for가 미세하게 빠르다.
                    //  - List<T> 클래스의 Count를 가져오는데 엄청난 리소스가 낭비된다.
                    //  - List<T>가 null인지 아닌지 검사하는 것만으로도 1.24sec 에서 0.54sec로 빨라졌다..
                    if (command.Nodes != null)
                    {
                        for (int i = 0; i < command.Nodes.Count; i++)
                        {
                            if (FindNode(value, command.Nodes[i]))
                                return true;
                        }
                    }
                    
                    return false;

                default:
                    break;
            }
            return false;
        }
    }
}
