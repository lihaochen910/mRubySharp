using System;
using System.Runtime.InteropServices;

namespace CandyFramework.mRuby
{
    /// <summary>
    /// mruby方法参数配置 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct mrb_args
    {
        public mrb_args(UInt32 value)
        {
            this.value = value;
        }

        public UInt32 value;

        public static mrb_args NONE()
        {
            return new mrb_args(0);
        }

        public static mrb_args ANY()
        {
            return new mrb_args(1 << 12);
        }

        public static mrb_args REQ(uint n)
        {
            return new mrb_args(((n) & 0x1f) << 18);
        }

        public static mrb_args OPT(uint n)
        {
            return new mrb_args(((n) & 0x1f) << 13);
        }

        public static mrb_args ARGS(uint req, uint opt)
        {
            return mrb_args.REQ(req) | mrb_args.OPT(opt);
        }

        public static mrb_args BLOCK()
        {
            return new mrb_args(1);
        }

        public static mrb_args operator |(mrb_args args1, mrb_args args2)
        {
            return new mrb_args(args1.value | args2.value);

        }
    }
}
