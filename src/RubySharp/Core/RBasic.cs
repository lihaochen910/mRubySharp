using System;
using System.Runtime.InteropServices;


namespace RubySharp {
    
    [StructLayout( LayoutKind.Sequential )]
    public struct RBasic {
        public rb_vtype tt;
        public uint color;
        public uint flags;
        public IntPtr c;
        public IntPtr gcnext;
    }
    
}
