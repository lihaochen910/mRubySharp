using System;
using System.Runtime.InteropServices;

namespace CandyFramework.mRuby
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RBasic
    {
        public mrb_vtype tt;
        public uint color;
        public uint flags;
        public IntPtr c;
        public IntPtr gcnext;
    }
}
