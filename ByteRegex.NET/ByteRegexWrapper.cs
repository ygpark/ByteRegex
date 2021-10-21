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
        #region PInvoke
        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr byteregex_init();

        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr byteregex_init_by_pattern(char[] pattern, int size);

        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int byteregex_free(IntPtr handle);

        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void byteregex_compile(IntPtr handle, char[] pattern, int size);

        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int byteregex_matches(IntPtr handle, byte[] buffer, int size);
        
        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int byteregex_get_matches(int[] indexArray, out int size);

        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int byteregex_get_matches_count(IntPtr handle);

        #endregion

        #region 멤버변수
        IntPtr _handle;
        #endregion

        public ByteRegexWrapper()
        {
            _handle = byteregex_init();
        }

        public ByteRegexWrapper(string pattern)
        {
            char[] buf = pattern.ToCharArray();
            _handle = byteregex_init_by_pattern(buf, buf.Length);
        }

        ~ByteRegexWrapper()
        {
            byteregex_free(_handle);
        }

        public void Compile(string pattern)
        {
            char[] arr = pattern.ToCharArray();
            byteregex_compile(_handle, arr, arr.Length);
        }


        public int[] Matches(byte[] buffer)
        {
            int count;
            int[] indexArray;

            byteregex_matches(_handle, buffer, buffer.Length);
            
            count = byteregex_get_matches_count(_handle);
            indexArray = new int[count];

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
