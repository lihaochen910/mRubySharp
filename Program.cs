using System;

namespace CandyFramework.mRuby
{
    class Program
    {
        static void Main(string[] args)
        {
            MRUBY.mrb_state = MRUBY.mrb_open();

            mrb_value v1 = MRUBY.mrb_fixnum_value_ex(2333);
            mrb_value v2 = MRUBY.mrb_float_value_ex(MRUBY.mrb_state, 65.5f);
            mrb_value v3 = MRUBY.mrb_bool_value_ex(0);

            Console.WriteLine(v1.ToString());
            Console.WriteLine(v2.ToString());
            Console.WriteLine(v3.ToString());

            IntPtr klass = MRUBY.mrb_define_class("MyClass");

            mrb_value klass_v = mrb_value.CreateOBJ(klass);

            MRUBY.mrb_define_method(klass, "log", WriteLine, mrb_args.ANY());

            MRUBY.mrb_load_string(@"
                cls = MyClass.new
                cls.log 'call MyClass.log function in mruby.'
            ");

            MRUBY.mrb_close(MRUBY.mrb_state);
        }

        static mrb_value WriteLine(IntPtr state, mrb_value context)
        {
            mrb_value[] args = MRUBY.GetFunctionArgs();

            string str = args[0];

            Console.WriteLine(str);

            return context;
        }
    }
}
