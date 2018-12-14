using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using mrb_float = System.Double;
using mrb_int = System.Int32;
using mrb_sym = System.UInt32;
using mrb_bool = System.Byte;

namespace CandyFramework.mRuby
{
    /// <summary>
    /// mRubyのポーティング。
    /// </summary>
    /// <remarks>
    /// 行き当たりばったりで機能を追加しています。
    /// </remarks>
    public static class mRubyDLL
    {
        /// <summary>
        /// DLL ファイルのパスです。
        /// </summary>
        public const string MRubyDll = "mruby";

        /// <summary>
        /// mruby VM
        /// </summary>
        //public static IntPtr mrb_state { get; set; }

        /// <summary>
        /// Ruby スクリプトを解釈するときのエンコーディングを取得または設定します。
        /// </summary>
        public static System.Text.Encoding Encoding { get; set; } = System.Text.Encoding.UTF8;


        /// <summary>
        /// 从mruby中调用C#方法委托
        /// </summary>
        /// <param name="state"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate mrb_value MRubyCSFunction(IntPtr state, mrb_value instance);


        /// <summary>
        /// C 用に文字列を byte の配列に変換します。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToCBytes(string str)
        {
            return Encoding.GetBytes(str + '\0');
        }


        //
        // ruby_*
        //
        /// <summary>
        /// 初始化mruby虚拟机
        /// </summary>
        /// <returns>pointer</returns>
        [DllImport(MRubyDll)]
        public static extern IntPtr mrb_open();

        /// <summary>
        /// 关闭并释放mruby虚拟机
        /// </summary>
        /// <param name="mrb_state"></param>
        [DllImport(MRubyDll)]
        public static extern void mrb_close(IntPtr mrb_state);

        //
        // rb_eval*
        // mruby 运行函数
        // 
        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_load_string(IntPtr mrb_state, byte[] script_string);
        //public static mrb_value mrb_load_string(string script_string)
        //{
        //    return mrb_load_string(mrb_state, ToCBytes(script_string));
        //}

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_load_string_cxt(IntPtr mrb_state, byte[] script_string, IntPtr mrbc_context);

        /// <summary>
        /// 加载mrbc字节码
        /// </summary>
        /// <param name="mrb_state"></param>
        /// <param name="byte_code"></param>
        /// <returns></returns>
        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_load_irep(IntPtr mrb_state, byte[] byte_code);

        [DllImport(MRubyDll)]
        private static extern mrb_value mrb_load_nstring(IntPtr mrb_state, byte[] script_string, long len);


        //
        // 数値
        // (int が 32 bit でないと不具合が生じうる)
        //
        //public const int MRB_FIXNUM_FLAG = 0x01;
        //public const int FIXNUM_MAX = int.MaxValue >> 1;
        //public const int FIXNUM_MIN = int.MinValue >> 1;
        //public static bool FIXNUM_P(mrb_int val)
        //{
        //    return (val & MRB_FIXNUM_FLAG) != 0;
        //}

        [DllImport(MRubyDll)]
        private static extern mrb_value mrb_to_int(IntPtr mrb_state, mrb_value v);
        [DllImport(MRubyDll)]
        private static extern mrb_value mrb_to_str(IntPtr mrb_state, mrb_value v);
        [DllImport(MRubyDll)]
        private static extern mrb_value mrb_any_to_s(IntPtr mrb_state, mrb_value obj);

        [DllImport(MRubyDll)]
        private static extern string mrb_obj_classname(IntPtr mrb_state, mrb_value obj);


        //[DllImport(MRubyDll)]
        //private static extern VALUE rb_int2inum(Int32 i);
        //public static VALUE INT2NUM(Int32 i)
        //{
        //    if (FIXNUM_MIN <= i && i <= FIXNUM_MAX)
        //        return (i << 1) | MRB_FIXNUM_FLAG;
        //    return rb_int2inum(i);
        //}
        //[DllImport(MRubyDll)]
        //private static extern VALUE rb_ll2inum(Int64 i);
        //public static VALUE LL2NUM(Int64 i)
        //{
        //    if (FIXNUM_MIN <= i && i <= FIXNUM_MAX)
        //        return ((int)i << 1) | MRB_FIXNUM_FLAG;
        //    return rb_ll2inum(i);
        //}
        //[DllImport(MRubyDll)]
        //private static extern int rb_num2int(VALUE val);
        //public static int NUM2INT(VALUE val)
        //{
        //    if (FIXNUM_P(val))
        //        return val >> 1;
        //    return rb_num2int(val);
        //}
        //[DllImport(MRubyDll, EntryPoint = "rb_num2dbl")]
        //public static extern double NUM2DBL(VALUE val);

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_float_value_ex(IntPtr mrb_state, mrb_float f);

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_fixnum_value_ex(mrb_int i);

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_bool_value_ex(mrb_bool b);

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_nil_value_ex();

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_undef_value_ex();

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_false_value_ex();

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_true_value_ex();

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_cptr_value_ex(IntPtr mrb_state, IntPtr p);

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_obj_value_ex(IntPtr p);


        public static mrb_value mrb_bool_value(bool b)
        {
            return mrb_bool_value_ex(b ? (mrb_bool)1 : (mrb_bool)0);
        }

        //
        // 文字列
        //
        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_str_new(byte[] ptr, long len);

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_str_new_cstr(IntPtr mrb_state, byte[] ptr);

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_str_new_static(IntPtr mrb_state, byte[] ptr, long len);

        /// <summary>
        /// Mrb Object To String
        /// </summary>
        /// <param name="mrb_state"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_obj_as_string(IntPtr mrb_state, mrb_value obj);

        /// <summary>
        /// Mrb String Pointer
        /// </summary>
        /// <param name="mrb_state"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        [DllImport(MRubyDll)]
        public static extern IntPtr mrb_string_value_ptr(IntPtr mrb_state, mrb_value str);
        /// <summary>
        /// mruby value to C# string
        /// </summary>
        /// <param name="obj"></param>
        //public static string mrb_value_to_csstr(mrb_value obj)
        //{
        //    if (obj.mrb_type == mrb_vtype.MRB_TT_STRING)
        //        return Marshal.PtrToStringAuto(mrb_string_value_ptr(mrb_state, obj));
        //    else
        //    {
        //        return Marshal.PtrToStringAuto(mrb_string_value_ptr(mrb_state, mrb_obj_as_string(mrb_state, obj))); 
        //    }
        //}


        [DllImport(MRubyDll)]
        public static extern IntPtr mrb_string_value_cstr(IntPtr mrb_state, ref mrb_value v_ptr);
        public static string StringValuePtr(IntPtr mrb_state, mrb_value v)
        {
            int length = 0;
            IntPtr ptr = mrb_string_value_cstr(mrb_state, ref v);
            unsafe
            {
                byte* p = (byte*)ptr;
                while (*p != 0)
                {
                    length++;
                    p++;
                }
            }
            byte[] bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, length);
            return Encoding.GetString(bytes);
        }


        //
        // Symbol
        //
        [DllImport(MRubyDll)]
        private static extern mrb_sym mrb_intern(IntPtr mrb_state, byte[] name, UInt64 s);
        [DllImport(MRubyDll)]
        private static extern mrb_sym mrb_intern_cstr(IntPtr mrb_state, byte[] name);
        [DllImport(MRubyDll)]
        private static extern mrb_sym mrb_intern_static(IntPtr mrb_state, byte[] name, UInt64 s);
        //public static mrb_sym mrb_intern(string name)
        //{
        //    return mrb_intern_cstr(mRubyDLL.mrb_state, ToCBytes(name));
        //}


        [DllImport(MRubyDll)]
        private static extern Int64 mrb_obj_id(mrb_value obj);

        [DllImport(MRubyDll)]
        private static extern IntPtr mrb_obj_class(IntPtr mrb_state, mrb_value obj);

        //
        // 定数
        //
        [DllImport(MRubyDll)]
        public static extern void mrb_define_const(IntPtr mrb_state, IntPtr klass, string name, mrb_value val);
        [DllImport(MRubyDll)]
        public static extern void mrb_define_global_const(IntPtr mrb_state, string name, mrb_value val);
        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_const_get(IntPtr mrb_state, mrb_value obj, mrb_sym sym);
        //public static mrb_value mrb_const_get(mrb_value obj, string name)
        //{
        //    return mrb_const_get(mRubyDLL.mrb_state, obj, mRubyDLL.mrb_intern(name));
        //}


        //
        // module, class
        //
        [DllImport(MRubyDll)]
        public static extern IntPtr mrb_define_class(IntPtr mrb_state, string name, IntPtr super);

        [DllImport(MRubyDll)]
        private static extern IntPtr mrb_class_get(IntPtr mrb_state, string name);
        //public static IntPtr mrb_class_get(string name)
        //{
        //    return mrb_class_get(mrb_state, name);
        //}

        

        [DllImport(MRubyDll)]
        public static extern IntPtr mrb_define_module(IntPtr mrb_state, string name);

        [DllImport(MRubyDll)]
        public static extern IntPtr mrb_module_get(IntPtr mrb_state, string name);


        

        [DllImport(MRubyDll)]
        public static extern void mrb_get_args(IntPtr state, string format, out IntPtr argv, out int argc, out mrb_value block);


        //
        // コールバック
        // 
        //public delegate mrb_int CallbackArg0(mrb_int self);
        //public delegate mrb_int CallbackArg1(mrb_int self, mrb_int arg1);
        //public delegate mrb_int CallbackArg2(mrb_int self, mrb_int arg1, mrb_int arg2);
        //public delegate mrb_int CallbackArg3(mrb_int self, mrb_int arg1, mrb_int arg2, mrb_int arg3);
        //public delegate mrb_int CallbackArg4(mrb_int self, mrb_int arg1, mrb_int arg2, mrb_int arg3, mrb_int arg4);
        //public delegate mrb_int CallbackArg5(mrb_int self, mrb_int arg1, mrb_int arg2, mrb_int arg3, mrb_int arg4, mrb_int arg5);
        //public delegate mrb_int CallbackArg15(mrb_int self, mrb_int arg1, mrb_int arg2, mrb_int arg3, mrb_int arg4, mrb_int arg5, mrb_int arg6, mrb_int arg7, mrb_int arg8, mrb_int arg9, mrb_int arg10, mrb_int arg11, mrb_int arg12, mrb_int arg13, mrb_int arg14, mrb_int arg15);

        //
        // メソッド
        //
        [DllImport(MRubyDll)]
        public static extern void mrb_define_method(IntPtr state, IntPtr klass, string name, MRubyCSFunction func, mrb_args aspec);
        [DllImport(MRubyDll)]
        public static extern void mrb_define_class_method(IntPtr state, IntPtr klass, string name, MRubyCSFunction func, mrb_args aspec);
        [DllImport(MRubyDll)]
        public static extern void mrb_define_module_function(IntPtr state, IntPtr klass, string name, MRubyCSFunction func, mrb_args aspec);
        [DllImport(MRubyDll)]
        public static extern void mrb_define_singleton_method(IntPtr state, IntPtr klass, string name, MRubyCSFunction func, mrb_args aspec);

        //public static void mrb_define_method(IntPtr klass, string name, MRubyCSFunction func, mrb_args aspec)
        //{
        //    MethodDelegates.Add(func);
        //    mrb_define_method(mRubyDLL.mrb_state, klass, name, func, aspec);
        //}


        //
        // private メソッド
        //
        //[DllImport(MRubyDll)]
        //private static extern void rb_define_private_method(VALUE klass, string name, CallbackArg0 func, int argc);
        //public static void rb_define_private_method(VALUE klass, string name, CallbackArg0 func)
        //{
        //    MethodDelegates.Add(func);
        //    rb_define_private_method(klass, name, func, 0);
        //}
        //[DllImport(MRubyDll)]
        //private static extern void rb_define_private_method(VALUE klass, string name, CallbackArg1 func, int argc);
        //public static void rb_define_private_method(VALUE klass, string name, CallbackArg1 func)
        //{
        //    MethodDelegates.Add(func);
        //    rb_define_private_method(klass, name, func, 1);
        //}
        //[DllImport(MRubyDll)]
        //private static extern void rb_define_private_method(VALUE klass, string name, CallbackArg2 func, int argc);
        //public static void rb_define_private_method(VALUE klass, string name, CallbackArg2 func)
        //{
        //    MethodDelegates.Add(func);
        //    rb_define_private_method(klass, name, func, 2);
        //}
        //[DllImport(MRubyDll)]
        //private static extern void rb_define_private_method(VALUE klass, string name, CallbackArg3 func, int argc);
        //public static void rb_define_private_method(VALUE klass, string name, CallbackArg3 func)
        //{
        //    MethodDelegates.Add(func);
        //    rb_define_private_method(klass, name, func, 3);
        //}
        //[DllImport(MRubyDll)]
        //private static extern void rb_define_private_method(VALUE klass, string name, CallbackArg4 func, int argc);
        //public static void rb_define_private_method(VALUE klass, string name, CallbackArg4 func)
        //{
        //    MethodDelegates.Add(func);
        //    rb_define_private_method(klass, name, func, 4);
        //}
        //[DllImport(MRubyDll)]
        //private static extern void rb_define_private_method(VALUE klass, string name, CallbackArg15 func, int argc);
        //public static void rb_define_private_method(VALUE klass, string name, CallbackArg15 func)
        //{
        //    MethodDelegates.Add(func);
        //    rb_define_private_method(klass, name, func, 15);
        //}


        //
        // funcall
        //
        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_funcall(IntPtr mrb_state, mrb_value obj, string funcName, int argc);
        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_funcall(IntPtr mrb_state, mrb_value obj, string funcName, int argc, mrb_value arg1);
        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_funcall(IntPtr mrb_state, mrb_value obj, string funcName, int argc, mrb_value arg1, mrb_value arg2);


        /// <summary>
        /// 从C#调用mruby方法时，使用这个方法获取参数
        /// </summary>
        /// <param name="state"></param>
        /// <param name="withBlock"></param>
        /// <returns></returns>
        public static mrb_value[] GetFunctionArgs(IntPtr mrb_state, bool withBlock = false)
        {
            mrb_value[] values;
            IntPtr argvPointer;
            mrb_value value = default(mrb_value);
            int i, argc, size;
            mrb_value block;

            mRubyDLL.mrb_get_args(mrb_state, "*&", out argvPointer, out argc, out block);

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



        //
        // 変数
        //
        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_iv_get(IntPtr mrb_state, mrb_value obj, mrb_sym sym);
        [DllImport(MRubyDll)]
        public static extern void mrb_iv_set(IntPtr mrb_state, mrb_value obj, mrb_sym sym, mrb_value v);
        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_obj_iv_get(IntPtr mrb_state, IntPtr obj, mrb_sym sym);
        [DllImport(MRubyDll)]
        public static extern void mrb_obj_iv_set(IntPtr mrb_state, IntPtr obj, mrb_sym sym, mrb_value v);

        //
        // 配列
        //
        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_ary_new();

        [DllImport(MRubyDll)]
        public static extern void mrb_ary_push(IntPtr mrb_state, mrb_value array, mrb_value value);

        //
        // Hash
        //
        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_hash_new();
        [DllImport(MRubyDll)]
        public static extern void mrb_hash_set(IntPtr mrb_state, mrb_value hash, mrb_value key, mrb_value val);
        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_hash_get(IntPtr mrb_state, mrb_value hash, mrb_value key);


        //
        // Context
        //
        [DllImport(MRubyDll)]
        public static extern IntPtr mrbc_context_new(IntPtr mrb_state);
        [DllImport(MRubyDll)]
        public static extern void mrbc_context_free(IntPtr mrb_state, IntPtr mrbc_context);


        //
        // 例外
        //
        [DllImport(MRubyDll)]
        private static extern void mrb_raise(IntPtr mrb_state, IntPtr obj, byte[] msg);


        //
        // そのほか
        //
        //private const int FL_FREEZE = (1 << 10);

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_inspect(IntPtr mrb_state, mrb_value obj);

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_get_backtrace(IntPtr mrb_state);

        [DllImport(MRubyDll)]
        public static extern mrb_value mrb_exc_backtrace(IntPtr mrb_state, mrb_value exc);

        //public static unsafe void rb_check_frozen(VALUE obj)
        //{
        //    if ((((RBasic*)obj)->flags & FL_FREEZE) != 0)
        //        rb_error_frozen(obj);
        //}
        //[DllImport(MRubyDll)]
        //public static extern void rb_error_frozen(VALUE obj);
        //[DllImport(MRubyDll)]
        //public static extern VALUE rb_obj_init_copy(VALUE obj, VALUE orig);
        //[DllImport(MRubyDll)]
        //public static extern VALUE rb_define_alias(VALUE module, string newName, string oldName);

        //
        // 便利な関数
        //
        //public static VALUE Intern(string str)
        //{
        //    VALUE symbol;
        //    if (SymbolCache.TryGetValue(str, out symbol))
        //        return symbol;
        //    else
        //        return SymbolCache[str] = RUBY.Send(RUBY.rb_str_new2(str), "intern");
        //}
        //private static Dictionary<string, VALUE> SymbolCache = new Dictionary<string, int>();
        //public static VALUE HashAref(VALUE hash, string key)
        //{
        //    return RUBY.rb_hash_aref(hash, RUBY.Intern(key));
        //}
        //public static bool ToBool(VALUE obj)
        //{
        //    return obj != MRB_Qnil && obj != MRB_Qfalse;
        //}
        //public static VALUE Send(VALUE recv, string methodName)
        //{
        //    return rb_funcall(recv, mrb_intern(methodName));
        //}
        //public static VALUE Send(VALUE recv, string methodName, VALUE arg1)
        //{
        //    return rb_funcall(recv, mrb_intern(methodName), arg1);
        //}
        //public static VALUE Send(VALUE recv, string methodName, VALUE arg1, VALUE arg2)
        //{
        //    return rb_funcall(recv, mrb_intern(methodName), arg1, arg2);
        //}
        //public static VALUE Send(VALUE recv, string methodName, VALUE arg1, VALUE arg2, VALUE arg3)
        //{
        //    return rb_funcall(recv, mrb_intern(methodName), arg1, arg2, arg3);
        //}
        //public static VALUE Send(VALUE recv, string methodName, VALUE arg1, VALUE arg2, VALUE arg3, VALUE arg4)
        //{
        //    return rb_funcall(recv, mrb_intern(methodName), arg1, arg2, arg3, arg4);
        //}
        //public static VALUE Send(VALUE recv, string methodName, VALUE arg1, VALUE arg2, VALUE arg3, VALUE arg4, VALUE arg5)
        //{
        //    return rb_funcall(recv, mrb_intern(methodName), arg1, arg2, arg3, arg4, arg5);
        //}

        /// <summary>
        /// GC に回収されないための、デリゲートの参照です。
        /// </summary>
        public static List<Delegate> MethodDelegates { get; private set; } = new List<Delegate>();
    }
}
