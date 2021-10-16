using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightweightBinRegex
{
    internal enum CommandCode
    {
        Byte,
        Range
    }






    internal class Command
    {
        public CommandCode code;
        public byte start;
        public byte end;

        public override string ToString()
        {
            return String.Format("Code:{0}, start:0x{1:X2}, end:0x{2:X2}", code, (int)start, (int)end);
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
        private List<Command> commandes = new List<Command>();
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
            commandes.Clear();

            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] == '[')
                {
                    i++;
                    while (p[i] != ']')
                    {
                        if (p[i - 1] == '-')
                        {
                            commandes.RemoveAt(commandes.Count - 1);// '-' 지우기
                            Command lastCmd = commandes[commandes.Count - 1];// '-' 왼쪽 커맨드 꺼내기
                            commandes.RemoveAt(commandes.Count - 1);// '-' 왼쪽 커맨드 삭제
                            lastCmd.code = CommandCode.Range;
                            lastCmd.end = Convert.ToByte(p[i]);
                            commandes.Add(lastCmd);
                        }
                        else
                        {
                            Command cmd = new Command()
                            {
                                code = CommandCode.Byte,
                                start = Convert.ToByte(p[i])
                            };
                            commandes.Add(cmd);
                        }
                        i++;
                    }
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
                    last = commandes[commandes.Count - 1];
                    for (int j = 0; j < count - 1; j++)//이미 1개는 들어가 있으니까 count-1회 반복한다.
                    {
                        commandes.Add(last);
                    }
                }
                else
                {
                    commandes.Add(new Command()
                    {
                        code = CommandCode.Byte,
                        start = Convert.ToByte(p[i])
                    });
                }
            }

            _isCompiled = true;
        }



        public ByteMatches Matches(byte[] data)
        {

            if (!_isCompiled)
            {
                throw new Exception("패턴이 없습니다.");
            }

            matches.Clear();

            if (commandes.Count > data.Length)
                return matches;

            for (int i = 0; i < data.Length; i++)
            {
                // 0 1 2
                // count = 3
                int remain = data.Length - i;
                if (remain < commandes.Count)
                    break;

                int hitSum = 0;
                for (int j = 0; j < commandes.Count; j++)
                {
                    int hit = 0;
                    switch (commandes[j].code)
                    {
                        case CommandCode.Byte:
                            if (data[i + j] == commandes[j].start)
                            {
                                hitSum++;
                                hit++;
                            }
                            break;
                        case CommandCode.Range:
                            if (commandes[j].start <= data[i + j] && data[i + j] <= commandes[j].end)
                            {
                                hitSum++;
                                hit++;
                            }
                            break;
                        default:
                            throw new Exception("잘못된 컴파일.");
                    }
                    //2021-10-16 박영기
                    //성능을 위하여 하나라도 hit되지 않으면 현재 바이트에 대한 패턴 검사를 중단한다.
                    //이번 테스트에서는 6배 정도 속도가 향상되었다.
                    if (hit != 1)
                        break;
                }

                if (hitSum == commandes.Count)
                {
                    matches.Add(new ByteMatch(i));
                }
            }

            return matches;
        }
    }
}
