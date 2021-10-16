using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightweightBinRegex;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] dataHasPattern = new byte[] { 0x30, 0x31, 0x00, 0x55, 0x10, 0x11, 0x00, 0x00, 0x30, 0x31, 0x00, 0x01, 0x10, 0x11, 0x55 };
            byte[] data100MB = new byte[100 * 1024 * 1024];
            string pattern = "\x31[\x00-\x33]{2}\x10\x11";
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
            Console.ReadLine();
        }
    }
}
