using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ByteRegex.NET
{
    public class ByteRegex
    {
        
        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Compile(string pattern);

        [DllImport("ByteRegex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Matches(byte[] buffer, int bufferSize, int[] indexArray, int indexSize);

        public ByteRegex(string pattern)
        {
            Compile(pattern);
        }
    }
}
