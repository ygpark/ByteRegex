using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using ByteRegex.NET;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //                                                     ----------------
            byte[] dataHasPattern = new byte[] { 0x01, 0x11, 0x00, 0x01, 0x12, 0x12, 0x01, 0xF1, 0x30, 0x31, 0x00, 0x01, 0x10, 0x11, 0x55 };
            byte[] data100MB = new byte[10 * 1024 * 1024];//버퍼를 100MB로 잡고 x86으로 빌드하면 Access Violation 오류 발생.
            //byte[] data100MB = new byte[30];
            //string pattern = "\x01[\x10\x12-\xF0\xFF]{2}";
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

            for (int i = 0; i < 10; i++)
            {
                ByteRegex.NET.ByteRegexWrapper byteRegex1 = new ByteRegex.NET.ByteRegexWrapper(pattern);
                watch.Reset();
                watch.Start();
                int[] matchIndex = byteRegex1.Matches(data100MB);
                watch.Stop();
                Console.WriteLine($"C++(소요시간: {watch.Elapsed}) (매치: {matchIndex.Length})");
            }
            


            ////시작
            //ByteRegex0.ByteRegex binRegex = new ByteRegex0.ByteRegex(pattern);
            //watch.Reset();
            //watch.Start();
            ////binRegex.MatchesParallel(data100MB);
            //ByteRegex0.ByteMatches bm = binRegex.Matches(data100MB);
            //watch.Stop();
            //Console.WriteLine($"c#(소요시간: {watch.Elapsed}) (매치: {bm.Count})");


            

            //Console.WriteLine($"종료: {matches.Count}개 패턴 매치.");
            // [1] (소요시간: 00:00:00.0397982) 비교 안하고 for-if 순회만 하면
            // [2] (소요시간: 00:00:01.2921870) ByteRegex 사용한 시간
            // [3] (소요시간: 00:00:00.0051644) [2]와 동일 조건에서 Re2 시간... 258배 빠름
            // [4] (소요시간: 00:00:00.5758892) [2]에서 List<T>.Count 및 foreach->for 최적화 만으로 2배 이상 빨라짐.
            // [5] (소요시간: 00:00:00.3912497) [4]에서 Parallel.For를 이용한 병렬 처리로 약 1.4배 더 빨라짐.
            Console.ReadLine();
        }
    }
}
