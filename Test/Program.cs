using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LightweightBinRegex;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //                                                     ----------------
            byte[] dataHasPattern = new byte[] { 0x01, 0x11, 0x00, 0x01, 0x12, 0x12, 0x01, 0xF1, 0x30, 0x31, 0x00, 0x01, 0x10, 0x11, 0x55 };
            byte[] data100MB = new byte[100 * 1024 * 1024];
            string pattern = "\x01\x11";
            Stopwatch watch = new Stopwatch();

            //검사할 데이터 준비
            int writeCount = 0;
            for (int i = 0; i < data100MB.Length; i += 512)
            {
                Array.Copy(dataHasPattern, 0, data100MB, i, dataHasPattern.Length);
                writeCount++;
            }
            Console.WriteLine("준비: 100MB 메모리에 {0}개 패턴 쓰기 완료.", writeCount);

            //시작
            watch.Start();
            ByteRegex binRegex = new ByteRegex(pattern);
            var matches = binRegex.Matches(data100MB);
            watch.Stop();
            Console.WriteLine($"종료: {matches.Count}개 패턴 매치.");
            Console.WriteLine($"(소요시간: {watch.Elapsed})");
            // [1] (소요시간: 00:00:00.0397982) 비교 안하고 for-if 순회만 하면
            // [2] (소요시간: 00:00:01.2921870) ByteRegex 사용한 시간.........동일 조건 Re2는 
            // [3] (소요시간: 00:00:00.0051644) [2]와 동일 조건에서 Re2 시간... 258배 빠름
            Console.ReadLine();
        }
    }
}
