using System;
using System.Runtime.InteropServices;

namespace CandyFramework.mRuby
{
    class Program
    {
        static mRubyState state;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            state = new mRubyState();

            mrb_value v1 = mrb_value.Create(2333);
            mrb_value v2 = mrb_value.Create(state, 65.5f);
            mrb_value v3 = mrb_value.FALSE;
            mrb_value v4 = mrb_value.TRUE;
            mrb_value v5 = mrb_value.NIL;

            Console.WriteLine(v1.ToString(state));
            Console.WriteLine(v2.ToString(state));
            Console.WriteLine(v3.ToString(state));
            Console.WriteLine(v4.ToString(state));
            Console.WriteLine(v5.ToString(state));

            state.DefineMethod("log", WriteLine, mrb_args.ANY());
            state.DefineMethod("show_backtrace", ShowBackTrace, mrb_args.NONE());

            mRubyClass klass = new mRubyClass(state, "CSharpClass");

            klass.DefineMethod("write", WriteLine, mrb_args.ANY());

			state.DoString("CSharpClass.new.write \"mruby #{RUBY_VERSION}\"");

			Console.ReadKey();
        }

        static mrb_value WriteLine(IntPtr state, mrb_value context)
        {
            mrb_value[] args = mRubyDLL.GetFunctionArgs(state);

            string str = args[0].ToString(state);

            Console.WriteLine(str);

            return context;
        }

        static mrb_value ShowBackTrace(IntPtr state, mrb_value context)
        {
            Console.WriteLine(Program.state.GetCurrentBackTrace());
            return context;
        }
    }
}
