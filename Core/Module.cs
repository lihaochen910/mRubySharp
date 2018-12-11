using System;
using System.Runtime.InteropServices;

namespace CandyFramework.mRuby
{
    public class Module
    {
        private IntPtr state;
        private IntPtr klass;

        /// <summary>
        /// Module Name
        /// </summary>
        public string name { get; private set; }

        public Module(IntPtr state, string name)
        {
            this.name = name;

            this.state = state;
            this.klass = MRUBY.mrb_define_module(state, name);
        }

        public void DefineMethod(string name, MRUBY.Func receiver, mrb_args aspec)
        {
            MRUBY.mrb_define_module_function(state, klass, name, receiver, aspec);
        }

        public static mrb_value[] GetArgs(IntPtr state, bool withBlock = false)
        {
            mrb_value[] values;
            IntPtr argvPointer;
            mrb_value value = default(mrb_value);
            int i, argc, size;
            mrb_value block;

            MRUBY.mrb_get_args(state, "*&", out argvPointer, out argc, out block);

            int valueCount = argc;
            if (withBlock) { valueCount++; }

            values = new mrb_value[valueCount]; // Include Block
            size = Marshal.SizeOf(typeof(mrb_value));
            for (i = 0; i < argc; i++)
            {
                value = (mrb_value)Marshal.PtrToStructure(argvPointer + (i * size), typeof(mrb_value));
                values[i] = value;
            }

            if (withBlock)
            {
                values[argc] = value;
            }

            return values;
        }
    }
}
