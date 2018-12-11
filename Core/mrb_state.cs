using System;
using System.Runtime.InteropServices;

namespace CandyFramework.mRuby
{
    /// <summary>
    /// 施工中
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct mrb_state
    {
        public IntPtr jmp;

        public uint flags;
        public IntPtr allocf;
        public IntPtr allocf_ud;

        public IntPtr c;
        public IntPtr root_c;
        public IntPtr globals;

        public IntPtr exc;

        public IntPtr top_self;
        public IntPtr object_class;
        public IntPtr class_class;
        public IntPtr module_class;
        public IntPtr proc_class;
        public IntPtr string_class;
        public IntPtr array_class;
        public IntPtr hash_class;
        public IntPtr range_class;

        public IntPtr float_class;
        public IntPtr fixnum_class;
        public IntPtr true_class;
        public IntPtr false_class;
        public IntPtr nil_class;
        public IntPtr symbol_class;
        public IntPtr kernel_module;

        public IntPtr mems;

        // TODO:
    }
}
