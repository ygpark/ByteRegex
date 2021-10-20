using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ByteRegex.NET
{
    public class ByteRegexWrapper
    {
        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int byteregex_init();

        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int byteregex_init_with_pattern(byte[] pattern, int size);

        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int byteregex_free();

        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int byteregex_compile(char[] pattern, int size);

        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int byteregex_matches(byte[] buffer, int length);
        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int byteregex_get_matches(int[] indexArray, out int length);

        public ByteRegexWrapper(string pattern)
        {
            byteregex_init();
            char[] arr = pattern.ToCharArray();
            byteregex_compile(arr, arr.Length);
        }

        ~ByteRegexWrapper()
        {
            byteregex_free();
        }

        public void Compile(string pattern)
        {
            char[] arr = pattern.ToCharArray();
            byteregex_compile(arr, arr.Length);
        }


        public int[] Matches(byte[] buffer)
        {
            int count = byteregex_matches(buffer, buffer.Length);
            int[] indexArray = new int[count];
            int err = byteregex_get_matches(indexArray, out count);
            if (err == -1)// need more buffer
            {
                indexArray = new int[count];
                err = byteregex_get_matches(indexArray, out _);
                if (err != 0)
                {
                    throw new Exception("알 수 없는 오류 발생");
                }
            }

            return indexArray;
        }
    }
}
