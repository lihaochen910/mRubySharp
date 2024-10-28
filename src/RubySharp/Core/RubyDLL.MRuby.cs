#if MRUBY
#define MRB_INT32
#define MRB_USE_FLOAT32
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

#if MRB_INT32
using mrb_int = System.Int32;
#elif MRB_INT64
using mrb_int = System.Int64;
#endif
#if MRB_USE_FLOAT32
using mrb_float = System.Single;
#else
using mrb_float = System.Double;
#endif
using mrb_sym = System.UInt32;
using mrb_bool = System.Boolean;


namespace RubySharp {
	
	public static unsafe partial class RubyDLL {
		
		/// <summary>
		/// DLL ファイルのパスです。
		/// </summary>
		private const string __DllName = "mruby";


		// static RubyDLL() {
		// 	Console.WriteLine ( "static init." );
		// }
		
		
		/// <summary>
		/// 从mruby中调用C#方法委托
		/// mrb_func_t
		/// </summary>
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL RubyCSFunction( IntPtr state, R_VAL self );

		/// <summary>
		/// data type release function pointer
		/// </summary>
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL RubyDFreeFunction( IntPtr state, IntPtr data );

		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL RubyAllocFunction( mrb_state* mrb, IntPtr ptr, ulong size, IntPtr ud );

		
		/// <summary>
		/// Custom data type description.
		/// </summary>
		[StructLayout( LayoutKind.Sequential )]
		public struct mrb_data_type {
			public string struct_name;
			public RubyDFreeFunction dfree;
		}
		
		
		//
        // ruby_*
        //
        /// <summary>
        /// 初始化mruby虚拟机
        /// </summary>
        /// <returns>pointer</returns>
		[DllImport(__DllName, EntryPoint = "mrb_open", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_state* mrb_open();
		
		
		[DllImport(__DllName, EntryPoint = "mrb_open_allocf", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_state* mrb_open_allocf( RubyAllocFunction f, IntPtr ud );
		
		
		[DllImport(__DllName, EntryPoint = "mrb_open_core", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_state* mrb_open_core( RubyAllocFunction f, IntPtr ud );
		
		
        /// <summary>
        /// 关闭并释放mruby虚拟机
        /// </summary>
        /// <param name="mrb_state"></param>
		[DllImport(__DllName, EntryPoint = "mrb_close", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void mrb_close( IntPtr mrb );

		//
		// rb_eval*
		// mruby 运行函数
		// 
		[DllImport(__DllName, EntryPoint = "mrb_load_string", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_load_string( IntPtr mrb, byte[] s );
		
		
		// [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		// public delegate R_VAL d_mrb_load_nstring( IntPtr mrb_state, byte[] script_string, long len );
		// public static d_mrb_load_nstring mrb_load_nstring { get; } = FuncLoader.LoadFunction< d_mrb_load_nstring >( RubyLibrary, "mrb_load_nstring" );

		
		[DllImport(__DllName, EntryPoint = "mrb_load_string_cxt", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_load_string_cxt( IntPtr mrb, byte[] s, IntPtr cxt );
		
		
		[DllImport(__DllName, EntryPoint = "mrb_load_file", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_load_file( IntPtr mrb, IntPtr f );
		
		
		[DllImport(__DllName, EntryPoint = "mrb_load_file_cxt", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_load_file_cxt( IntPtr mrb, IntPtr f, IntPtr c );
		
		
		/// <summary>
		/// 加载mrbc字节码
		/// </summary>
		/// <param name="mrb"></param>
		/// <param name="bin"></param>
		/// <returns></returns>
		[DllImport(__DllName, EntryPoint = "mrb_load_irep", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_load_irep( IntPtr mrb, byte[] bin );


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

		// [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		// public delegate R_VAL d_mrb_to_int( IntPtr mrb_state, R_VAL v );
		//
		// [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		// public delegate R_VAL d_mrb_to_str( IntPtr mrb_state, R_VAL v );
		//
		// [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		// public delegate R_VAL d_mrb_any_to_s( IntPtr mrb_state, R_VAL obj );
		//
		// public static d_mrb_to_int mrb_to_int { get; } = FuncLoader.LoadFunction< d_mrb_to_int >( RubyLibrary, "mrb_to_int" );
		//
		// public static d_mrb_to_str mrb_to_str { get; } = FuncLoader.LoadFunction< d_mrb_to_str >( RubyLibrary, "mrb_to_str" );
		//
		// public static d_mrb_any_to_s mrb_any_to_s { get; } = FuncLoader.LoadFunction< d_mrb_any_to_s >( RubyLibrary, "mrb_any_to_s" );

		
		[DllImport(__DllName, EntryPoint = "mrb_float_value_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_float_value( IntPtr mrb, mrb_float f );

		
		[DllImport(__DllName, EntryPoint = "mrb_fixnum_value_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_fixnum_value( IntPtr mrb, mrb_int i );

		
		[DllImport(__DllName, EntryPoint = "mrb_bool_value_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_bool_value( IntPtr mrb, mrb_bool b );

		
		[DllImport(__DllName, EntryPoint = "mrb_nil_value_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_nil_value();

		
		[DllImport(__DllName, EntryPoint = "mrb_undef_value_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_undef_value();

		
		[DllImport(__DllName, EntryPoint = "mrb_false_value_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_false_value();

		
		[DllImport(__DllName, EntryPoint = "mrb_true_value_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_true_value();

		
		[DllImport(__DllName, EntryPoint = "mrb_cptr_value_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_cptr_value( IntPtr mrb, IntPtr p );

		
		[DllImport(__DllName, EntryPoint = "mrb_obj_value_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_obj_value( IntPtr p );

		
		[DllImport(__DllName, EntryPoint = "mrb_ptr_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrb_ptr( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_cptr_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrb_cptr( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_float_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_float mrb_float( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_fixnum_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_int mrb_fixnum( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_symbol_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_sym mrb_symbol( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_type_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern rb_vtype r_type( R_VAL o );
		
		
		[DllImport(__DllName, EntryPoint = "mrb_immediate_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_immediate_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_fixnum_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_fixnum_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_symbol_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_symbol_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_undef_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_undef_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_nil_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_nil_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_false_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_false_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_true_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_true_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_float_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_float_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_array_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_array_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_string_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_string_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_hash_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_hash_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_cptr_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_cptr_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_exception_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_exception_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_free_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_free_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_object_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_object_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_class_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_class_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_module_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_module_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_iclass_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_iclass_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_sclass_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_sclass_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_proc_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_proc_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_range_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_range_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_env_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_env_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_data_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_data_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_fiber_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_fiber_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_istruct_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_istruct_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_break_p_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_break_p( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_bool_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_bool mrb_bool( R_VAL o );
		
		[DllImport(__DllName, EntryPoint = "mrb_test_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern bool r_test( R_VAL o );
		
		
		
		//
		// 文字列
		//
		[DllImport(__DllName, EntryPoint = "mrb_str_new", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_str_new( byte[] ptr, long len );


		[DllImport(__DllName, EntryPoint = "mrb_str_new_cstr", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_str_new_cstr( IntPtr mrb_state, byte[] ptr );


		[DllImport(__DllName, EntryPoint = "mrb_str_new_static", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_str_new_static( IntPtr mrb, byte[] ptr, long len );


		[DllImport(__DllName, EntryPoint = "mrb_obj_as_string", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_obj_as_string( IntPtr mrb, R_VAL obj );


		/// <summary>
		/// mruby value to C# string
		/// </summary>
		/// <param name="obj"></param>
		//public static string R_VAL_to_csstr(R_VAL obj)
		//{
		//    if (obj.mrb_type == mrb_vtype.MRB_TT_STRING)
		//        return Marshal.PtrToStringAuto(mrb_string_value_ptr(mrb_state, obj));
		//    else
		//    {
		//        return Marshal.PtrToStringAuto(mrb_string_value_ptr(mrb_state, mrb_obj_as_string(mrb_state, obj))); 
		//    }
		//}

		[DllImport(__DllName, EntryPoint = "mrb_string_value_cstr", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrb_string_value_cstr( IntPtr mrb, ref R_VAL v_ptr );

		public static string StringValuePtr( IntPtr mrb_state, R_VAL v ) {
			int length = 0;
			IntPtr ptr = mrb_string_value_cstr( mrb_state, ref v );
			unsafe {
				byte * p = ( byte * )ptr;
				while ( *p != 0 ) {
					length++;
					p++;
				}
			}

			byte[] bytes = new byte[ length ];
			Marshal.Copy( ptr, bytes, 0, length );
			return Encoding.GetString( bytes );
		}


		//
		// Symbol
		//
		[DllImport(__DllName, EntryPoint = "mrb_intern", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_sym mrb_intern( IntPtr mrb, byte[] name, UInt64 s );


		[DllImport(__DllName, EntryPoint = "mrb_intern_cstr", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_sym mrb_intern_cstr( IntPtr mrb, byte[] name );


		[DllImport(__DllName, EntryPoint = "mrb_intern_static", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_sym mrb_intern_static( IntPtr mrb, byte[] name, UInt64 s );


		[DllImport(__DllName, EntryPoint = "mrb_obj_id", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern Int64 mrb_obj_id( R_VAL obj );


		[DllImport(__DllName, EntryPoint = "mrb_obj_class", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrb_obj_class( IntPtr mrb, R_VAL obj );


		//
		// 定数
		//
		[DllImport(__DllName, EntryPoint = "mrb_define_const", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void r_define_const( IntPtr mrb, IntPtr klass, string name, R_VAL val );


		[DllImport(__DllName, EntryPoint = "mrb_define_global_const", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void mrb_define_global_const( IntPtr mrb, string name, R_VAL val );


		[DllImport(__DllName, EntryPoint = "mrb_const_get", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_const_get( IntPtr mrb, R_VAL obj, mrb_sym sym );


		//
		// module, class
		//
		[DllImport(__DllName, EntryPoint = "mrb_define_class", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr r_define_class( IntPtr mrb, string name, IntPtr super );


		[DllImport(__DllName, EntryPoint = "mrb_define_class_under", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr r_define_class_under( IntPtr mrb, IntPtr outer, string name, IntPtr super );


		// [DllImport ( MRubyDll, EntryPoint = "mrb_set_instance_tt" )]
		// public static extern void MRB_SET_INSTANCE_TT ( IntPtr c, rb_vtype tt );

		
		[DllImport(__DllName, EntryPoint = "mrb_class_get", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrb_class_get( IntPtr mrb, string name );


		[DllImport(__DllName, EntryPoint = "mrb_obj_new", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_obj_new( IntPtr mrb, IntPtr c, mrb_int argc, R_VAL[] argv );


		[DllImport(__DllName, EntryPoint = "mrb_define_module", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr r_define_module( IntPtr mrb, string name );


		[DllImport(__DllName, EntryPoint = "mrb_define_module_under", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr r_define_module_under( IntPtr mrb, IntPtr outer, string name, IntPtr super );


		[DllImport(__DllName, EntryPoint = "mrb_module_get", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrb_module_get( IntPtr mrb, string name );


		// [DllImport(__DllName, EntryPoint = "mrb_get_args", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		// public static extern void mrb_get_args( IntPtr mrb, string format, out IntPtr argv, out int argc, out R_VAL block );
		
		
		[DllImport(__DllName, EntryPoint = "mrb_get_argc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern mrb_int mrb_get_argc( IntPtr mrb );


		[DllImport(__DllName, EntryPoint = "mrb_get_argv", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrb_get_argv( IntPtr mrb );

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
		[DllImport(__DllName, EntryPoint = "mrb_define_method", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void r_define_method( IntPtr mrb, IntPtr klass, string name, RubyCSFunction func, rb_args aspec );


		[DllImport(__DllName, EntryPoint = "mrb_define_class_method", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void r_define_class_method( IntPtr mrb, IntPtr klass, string name, RubyCSFunction func, rb_args aspec );


		[DllImport(__DllName, EntryPoint = "mrb_define_module_function", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void r_define_module_function( IntPtr mrb, IntPtr klass, string name, RubyCSFunction func, rb_args aspec );


		[DllImport(__DllName, EntryPoint = "mrb_define_singleton_method", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void mrb_define_singleton_method( IntPtr mrb, IntPtr klass, string name, RubyCSFunction func, rb_args aspec );


		//public static void mrb_define_method(IntPtr klass, string name, MRubyCSFunction func, mrb_args aspec)
		//{
		//    MethodDelegates.Add(func);
		//    mrb_define_method(RubyDLL.mrb_state, klass, name, func, aspec);
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
		[DllImport(__DllName, EntryPoint = "mrb_funcall", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL r_funcall( IntPtr mrb, R_VAL obj, string funcName, int argc );

		[DllImport(__DllName, EntryPoint = "mrb_funcall", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL r_funcall_1( IntPtr mrb, R_VAL obj, string funcName, int argc, R_VAL arg1 );

		//[DllImport(MRubyDll)]
  //      public static extern R_VAL mrb_funcall(IntPtr mrb_state, R_VAL obj, string funcName, int argc, R_VAL arg1);
  //      [DllImport(MRubyDll)]
  //      public static extern R_VAL mrb_funcall(IntPtr mrb_state, R_VAL obj, string funcName, int argc, R_VAL arg1, R_VAL arg2);


		/// <summary>
		/// 从C#调用mruby方法时，使用这个方法获取参数
		/// </summary>
		/// <param name="state"></param>
		/// <param name="withBlock"></param>
		/// <returns></returns>
		public static R_VAL[] GetFunctionArgs( IntPtr state, bool withBlock = false ) {
			// R_VAL block;
			// RubyDLL.mrb_get_args( state, "*&", out argvPointer, out argc, out block );
			var argc = RubyDLL.mrb_get_argc( state );
			if ( argc < 1 ) {
				return Array.Empty< R_VAL >();
			}
			var argvPointer = RubyDLL.mrb_get_argv( state );

			int valueCount = argc;
			if ( withBlock ) {
				valueCount++;
			}
			
			R_VAL value = R_VAL.NIL;

			var values = new R_VAL[ valueCount ]; // Include Block
			var size = Marshal.SizeOf( typeof( R_VAL ) );
			for ( var i = 0; i < argc; i++ ) {
				value = ( R_VAL )Marshal.PtrToStructure( argvPointer + ( i * size ), typeof( R_VAL ) );
				values[ i ] = value;
			}

			if ( withBlock ) {
				values[ argc ] = value;
			}

			return values;
		}

		
		// 変数
		//
		[DllImport(__DllName, EntryPoint = "mrb_iv_get", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_iv_get( IntPtr mrb, R_VAL obj, mrb_sym sym );


		[DllImport(__DllName, EntryPoint = "mrb_iv_set", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void mrb_iv_set( IntPtr mrb, R_VAL obj, mrb_sym sym, R_VAL v );


		[DllImport(__DllName, EntryPoint = "mrb_obj_iv_get", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_obj_iv_get( IntPtr mrb, IntPtr obj, mrb_sym sym );


		[DllImport(__DllName, EntryPoint = "mrb_obj_iv_set", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void mrb_obj_iv_set( IntPtr mrb, IntPtr obj, mrb_sym sym, R_VAL v );


		//
		// 配列
		//
		[DllImport(__DllName, EntryPoint = "mrb_ary_new", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL r_ary_new();


		[DllImport(__DllName, EntryPoint = "mrb_ary_push", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void r_ary_push( IntPtr mrb, R_VAL array, R_VAL value );


		[DllImport(__DllName, EntryPoint = "mrb_ary_entry", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL r_ary_entry( R_VAL ary, mrb_int n );
		

		//
		// Hash
		//
		[DllImport(__DllName, EntryPoint = "mrb_hash_new", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_hash_new();


		[DllImport(__DllName, EntryPoint = "mrb_hash_set", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void mrb_hash_set( IntPtr mrb, R_VAL hash, R_VAL key, R_VAL val );


		[DllImport(__DllName, EntryPoint = "mrb_hash_get", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_hash_get( IntPtr mrb, R_VAL hash, R_VAL key );


		//
		// Context
		//
		[DllImport(__DllName, EntryPoint = "mrb_ccontext_new", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrbc_context_new( IntPtr mrb );


		[DllImport(__DllName, EntryPoint = "mrb_ccontext_free", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void mrbc_context_free( IntPtr mrb, IntPtr mrbc_context );


		[DllImport(__DllName, EntryPoint = "mrb_ccontext_filename", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void mrbc_filename( IntPtr mrb, IntPtr mrbc_context, string name );

		
		//
		// Data
		// 
		[DllImport(__DllName, EntryPoint = "mrb_data_object_alloc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrb_data_object_alloc( IntPtr mrb, IntPtr klass, IntPtr datap, IntPtr type );


		[DllImport(__DllName, EntryPoint = "mrb_data_init_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrb_data_init( R_VAL v, IntPtr ptr, IntPtr data_type );


		[DllImport(__DllName, EntryPoint = "mrb_data_get_ptr", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrb_data_get_ptr( IntPtr mrb, R_VAL obj, IntPtr type );


		[DllImport(__DllName, EntryPoint = "mrb_set_instance_tt", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrb_set_instance_tt( IntPtr klass, rb_vtype tt );


		[DllImport(__DllName, EntryPoint = "mrb_data_wrap_struct", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrb_data_wrap_struct( IntPtr mrb, IntPtr klass, IntPtr data_type, IntPtr ptr );


		[DllImport(__DllName, EntryPoint = "mrb_data_wrap_struct_obj", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_data_wrap_struct_obj( IntPtr mrb, IntPtr klass, IntPtr data_type, IntPtr ptr );
		
		[DllImport(__DllName, EntryPoint = "mrb_set_data_type", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrb_set_data_type( IntPtr mrb, R_VAL v, IntPtr data_type_ptr );
		
		
		[DllImport(__DllName, EntryPoint = "mrb_set_data_ptr", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern IntPtr mrb_set_data_ptr( IntPtr mrb, R_VAL v, IntPtr data_ptr );

		
		//
		// 例外
		//
		[DllImport(__DllName, EntryPoint = "mrb_malloc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		private static extern IntPtr mrb_malloc( IntPtr mrb, long len );

		
		[DllImport(__DllName, EntryPoint = "mrb_raise", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		private static extern void mrb_raise( IntPtr mrb, IntPtr obj, byte[] msg );


		//
		// そのほか
		//
		//private const int FL_FREEZE = (1 << 10);
		[DllImport(__DllName, EntryPoint = "mrb_inspect", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_inspect( IntPtr mrb, R_VAL obj );

		
		[DllImport(__DllName, EntryPoint = "mrb_top_self", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_top_self( IntPtr mrb );

		
		[DllImport(__DllName, EntryPoint = "mrb_has_exc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern bool mrb_has_exc( IntPtr mrb );
		
		
		[DllImport(__DllName, EntryPoint = "mrb_exc_clear", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void mrb_exc_clear( IntPtr mrb );
		
		
		[DllImport(__DllName, EntryPoint = "mrb_get_exc_value", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_get_exc_value( IntPtr mrb );

		
		[DllImport(__DllName, EntryPoint = "mrb_exc_detail", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL mrb_exc_detail( IntPtr mrb );

		
		[DllImport(__DllName, EntryPoint = "mrb_get_backtrace", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL r_get_backtrace( IntPtr mrb );

		
		[DllImport(__DllName, EntryPoint = "mrb_exc_backtrace", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern R_VAL r_exc_backtrace( IntPtr mrb, R_VAL exc );

		
		//
		// GC
		//
		[DllImport(__DllName, EntryPoint = "mrb_gc_arena_save_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern int mrb_gc_arena_save( IntPtr mrb );
		
		
		[DllImport(__DllName, EntryPoint = "mrb_gc_arena_restore_ex", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void mrb_gc_arena_restore( IntPtr mrb, int idx );
		
		
		[DllImport(__DllName, EntryPoint = "mrb_garbage_collect", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void mrb_garbage_collect( IntPtr mrb );
		
		
		[DllImport(__DllName, EntryPoint = "mrb_full_gc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void mrb_full_gc( IntPtr mrb );
		
		
		[DllImport(__DllName, EntryPoint = "mrb_incremental_gc", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void mrb_incremental_gc( IntPtr mrb );

		
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
		//public static List<Delegate> MethodDelegates { get; private set; } = new List<Delegate>();
	}

	
	/// <summary>
	/// mruby中的值类型
	/// (详细参见include\mruby\value.h)
	/// 保证兼容性, 使用CRuby枚举名定义
	/// 注意: mruby版本变动后可能需要更新此处枚举值
	/// mruby 3.0.0 2021-03-05
	/// </summary>
	public enum rb_vtype {
		RUBY_T_FALSE = 0, /*   0 */
		RUBY_T_TRUE,      /*   1 */
		RUBY_T_SYMBOL,    /*   2 */
		RUBY_T_UNDEF,     /*   3 */
		RUBY_T_FREE,      /*   4 */
		RUBY_T_FLOAT,     /*   5 */
		RUBY_T_INTEGER,   /*   6 */
		RUBY_T_CPTR,      /*   7 */
		RUBY_T_OBJECT,    /*   8 */
		RUBY_T_CLASS,     /*   9 */
		RUBY_T_MODULE,    /*  10 */
		RUBY_T_ICLASS,    /*  11 */
		RUBY_T_SCLASS,    /*  12 */
		RUBY_T_PROC,      /*  13 */
		RUBY_T_ARRAY,     /*  14 */
		RUBY_T_HASH,      /*  15 */
		RUBY_T_STRING,    /*  16 */
		RUBY_T_RANGE,     /*  17 */
		RUBY_T_EXCEPTION, /*  18 */
		RUBY_T_ENV,       /*  19 */
		RUBY_T_DATA,      /*  20 */
		RUBY_T_FIBER,     /*  21 */
		RUBY_T_STRUCT,	  /*  22 */
		RUBY_T_ISTRUCT,   /*  23 */
		RUBY_T_BREAK,     /*  24 */
		RUBY_T_COMPLEX,   /*  25 */
		RUBY_T_RATIONAL,  /*  26 */
		RUBY_T_BIGINT,    /*  27 */
		RUBY_T_BACKTRACE, /*  28 */
		RUBY_T_MAXDEFINE  /*  29 */
	}
	
	
	/// <summary>
	/// mruby方法参数配置 
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct rb_args {

		// public static rb_args NONE { get; } = new rb_args( 0 );

		public uint Value;

		public rb_args( uint value ) {
			Value = value;
		}
		
		public static rb_args NONE() {
			return new rb_args( 0 );
		}

		public static rb_args ANY() {
			return new rb_args( 1 << 12 );
		}

		public static rb_args REQ( uint n ) {
			return new rb_args( ( ( n ) & 0x1f ) << 18 );
		}

		public static rb_args OPT( uint n ) {
			return new rb_args( ( ( n ) & 0x1f ) << 13 );
		}

		public static rb_args ARGS( uint req, uint opt ) {
			return REQ( req ) | OPT( opt );
		}

		public static rb_args BLOCK() {
			return new rb_args( 1 );
		}

		public static rb_args operator |( rb_args args1, rb_args args2 ) {
			return new rb_args( args1.Value | args2.Value );
		}
	}
	
	
	/// <summary>
	/// data type release function pointer
	/// </summary>
	/// <param name="state">mrb_state *mrb</param>
	/// <param name="data">void*</param>
	/// <returns></returns>
	[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
	public delegate void MRubyDFreeFunction( IntPtr state, IntPtr data );

	
	/**
	 *  mrb_data_type Wrapper
	 * 
	 *  Create Helper to convert MrbValue to C# value types
	 */
	[StructLayout( LayoutKind.Sequential )]
	public struct mrb_data_type {
		
		[MarshalAs( UnmanagedType.BStr )] 
		public string struct_name;

		[MarshalAs( UnmanagedType.FunctionPtr )]
		public MRubyDFreeFunction dfree;
		
	}
	
	
	/// <summary>
	/// 施工中
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct mrb_state {
        
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
	
	
	/**
	 *  MrbValue Wrapper
	 * 
	 *  Create Helper to convert MrbValue to C# value types
	 *  we use MRB_NO_BOXING (unboxed mrb_value)
	 */
    [StructLayout( LayoutKind.Sequential )]
    public struct R_VAL {
        
        [StructLayout( LayoutKind.Explicit )]
        public struct Value {
            [FieldOffset( 0 )] public mrb_float f;
            [FieldOffset( 0 )] public IntPtr p;
            [FieldOffset( 0 )] public mrb_int i;
            [FieldOffset( 0 )] public mrb_sym sym;
        }

        public Value value;
        public rb_vtype tt;

        // public double   mrb_float  => value.f;
        // public Int64    mrb_fixnum => value.i;
        // public uint     mrb_symbol => value.sym;
        // public rb_vtype RbType   => tt;

        public static readonly R_VAL DEFAULT = new () { tt = rb_vtype.RUBY_T_UNDEF };
        public static readonly R_VAL TRUE = RubyDLL.mrb_true_value();
        public static readonly R_VAL FALSE = RubyDLL.mrb_false_value();
        public static readonly R_VAL NIL = RubyDLL.mrb_nil_value();

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static R_VAL Create( IntPtr mrb_state, int i ) {
            return RubyDLL.mrb_fixnum_value( mrb_state, i );
        }

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static R_VAL Create( IntPtr mrb_state, float f ) {
            return RubyDLL.mrb_float_value( mrb_state, f );
        }

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static R_VAL Create( IntPtr mrb_state, double dbl ) {
            return RubyDLL.mrb_float_value( mrb_state, ( float )dbl );
        }

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static R_VAL Create( bool b ) {
            return b ? RubyDLL.mrb_true_value() : RubyDLL.mrb_false_value();
        }

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static R_VAL Create( IntPtr mrb_state, string str ) {
            byte[] cbytes = RubyDLL.ToCBytes( str );
            return RubyDLL.mrb_str_new_static( mrb_state, cbytes, cbytes.Length );
        }

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static R_VAL CreateNIL() {
            return RubyDLL.mrb_nil_value();
        }

        public static R_VAL CreateUNDEF() {
            return RubyDLL.mrb_undef_value();
        }

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static R_VAL CreateOBJ( IntPtr p ) {
            return RubyDLL.mrb_obj_value( p );
        }

        public static R_VAL CreateOBJ( object obj ) {
			if ( obj == null ) {
				return RubyDLL.mrb_nil_value();
			}

            return R_VAL.CreateOBJ( GCHandle.ToIntPtr( GCHandle.Alloc( obj ) ) );
        }


        //public static implicit operator string(R_VAL value)
        //{
        //    int length = 0;
        //    IntPtr ptr = RubyDLL.mrb_string_value_cstr(RubyDLL.mrb_state, ref value);
        //    unsafe
        //    {
        //        byte* p = (byte*)ptr;
        //        while (*p != 0)
        //        {
        //            length++;
        //            p++;
        //        }
        //    }
        //    byte[] bytes = new byte[length];
        //    Marshal.Copy(ptr, bytes, 0, length);
        //    return RubyDLL.Encoding.GetString(bytes);

        //    //IntPtr stringPtr = IntPtr.Zero;

        //    //if (value.tt == mrb_vtype.MRB_TT_STRING)
        //    //{
        //    //    stringPtr = MRUBY.mrb_string_value_ptr(MRUBY.mrb_state, value);
        //    //}
        //    //else
        //    //{
        //    //    stringPtr = MRUBY.mrb_string_value_ptr(MRUBY.mrb_state, MRUBY.mrb_obj_as_string(MRUBY.mrb_state, value));
        //    //}
        //    //return Marshal.PtrToStringAuto(stringPtr);
        //}

        public static implicit operator int( R_VAL value ) {
            if ( value.tt == rb_vtype.RUBY_T_INTEGER ) {
                return value.value.i;
            }

            if ( value.tt == rb_vtype.RUBY_T_FLOAT ) {
                return ( int )value.value.f;
            }

            return 0;
        }
        
        public static implicit operator float( R_VAL value ) {
            if ( value.tt == rb_vtype.RUBY_T_FLOAT ) {
                return ( float )value.value.f;
            }

            if ( value.tt == rb_vtype.RUBY_T_INTEGER ) {
                return ( float )value.value.i;
            }

            return 0f;
        }

        public static implicit operator double( R_VAL value ) {
            if ( value.tt == rb_vtype.RUBY_T_FLOAT ) {
                return value.value.f;
            }

            if ( value.tt == rb_vtype.RUBY_T_INTEGER ) {
                return value.value.i;
            }

            return 0f;
        }

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static implicit operator bool( R_VAL value ) {
            switch ( value.tt ) {
                case rb_vtype.RUBY_T_FALSE:
                case rb_vtype.RUBY_T_EXCEPTION:
                case rb_vtype.RUBY_T_UNDEF:
                    return false;
            }

            return true;
        }

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static implicit operator IntPtr( R_VAL value ) {
            return value.value.p;
        }
        
        public static bool IsImmediate( R_VAL value ) {
            return RubyDLL.mrb_immediate_p( value );
        }
        
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool IsFixnum( R_VAL value ) {
            return RubyDLL.mrb_fixnum_p( value );
        }
        
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool IsSymbol( R_VAL value ) {
            return RubyDLL.mrb_symbol_p( value );
        }
        
        public static bool IsUndef( R_VAL value ) {
            return RubyDLL.mrb_undef_p( value );
        }
        
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool IsNil( R_VAL value ) {
            return RubyDLL.mrb_nil_p( value );
        }
        
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool IsFalse( R_VAL value ) {
            return RubyDLL.mrb_false_p( value );
        }
        
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool IsTrue( R_VAL value ) {
            return RubyDLL.mrb_true_p( value );
        }
        
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool IsFloat( R_VAL value ) {
            return RubyDLL.mrb_float_p( value );
        }
        
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool IsArray( R_VAL value ) {
            return RubyDLL.mrb_array_p( value );
        }
        
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool IsString( R_VAL value ) {
            return RubyDLL.mrb_string_p( value );
        }
        
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool IsHash( R_VAL value ) {
            return RubyDLL.mrb_hash_p( value );
        }
        
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool IsCPtr( R_VAL value ) {
            return RubyDLL.mrb_cptr_p( value );
        }
        
        public static bool IsException( R_VAL value ) {
            return RubyDLL.mrb_exception_p( value );
        }
        
        public static bool IsFree( R_VAL value ) {
            return RubyDLL.mrb_free_p( value );
        }
        
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool IsObject( R_VAL value ) {
            return RubyDLL.mrb_object_p( value );
        }
        
        public static bool IsClass( R_VAL value ) {
            return RubyDLL.mrb_class_p( value );
        }
        
        public static bool IsModule( R_VAL value ) {
            return RubyDLL.mrb_module_p( value );
        }
        
        public static bool IsIClass( R_VAL value ) {
            return RubyDLL.mrb_iclass_p( value );
        }
        
        public static bool IsSClass( R_VAL value ) {
            return RubyDLL.mrb_sclass_p( value );
        }
        
        public static bool IsProc( R_VAL value ) {
            return RubyDLL.mrb_proc_p( value );
        }
        
        public static bool IsRange( R_VAL value ) {
            return RubyDLL.mrb_range_p( value );
        }
		
        public static bool IsEnv( R_VAL value ) {
            return RubyDLL.mrb_env_p( value );
        }
        
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool IsData( R_VAL value ) {
            return RubyDLL.mrb_data_p( value );
        }
        
        public static bool IsFiber( R_VAL value ) {
            return RubyDLL.mrb_fiber_p( value );
        }
        
        public static bool IsIStruct( R_VAL value ) {
            return RubyDLL.mrb_istruct_p ( value );
        }
        
        public static bool IsBreak( R_VAL value ) {
            return RubyDLL.mrb_break_p( value );
        }
        
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool IsBool( R_VAL value ) {
            return RubyDLL.mrb_true_p( value ) || RubyDLL.mrb_false_p( value );
        }
        
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool Test( R_VAL value ) {
            if ( RubyDLL.mrb_true_p( value ) ) {
                return true;
            }
            if ( RubyDLL.mrb_false_p( value ) ) {
                return false;
            }
            return RubyDLL.r_test( value );
        }

        //public override string ToString()
        //{
        //    //IntPtr stringPtr = IntPtr.Zero;

        //    //if (tt == mrb_vtype.MRB_TT_STRING)
        //    //{
        //    //    stringPtr = MRUBY.mrb_string_value_ptr(MRUBY.mrb_state, this);
        //    //}
        //    //else
        //    //{
        //    //    stringPtr = MRUBY.mrb_string_value_ptr(MRUBY.mrb_state, MRUBY.mrb_obj_as_string(MRUBY.mrb_state, this));
        //    //}
        //    //return Marshal.PtrToStringAuto(stringPtr);

        //    var str = RubyDLL.mrb_obj_as_string(RubyDLL.mrb_state, this);
        //    int length = 0;
        //    IntPtr ptr = RubyDLL.mrb_string_value_cstr(RubyDLL.mrb_state, ref str);
        //    unsafe
        //    {
        //        byte* p = (byte*)ptr;
        //        while (*p != 0)
        //        {
        //            length++;
        //            p++;
        //        }
        //    }
        //    byte[] bytes = new byte[length];
        //    Marshal.Copy(ptr, bytes, 0, length);
        //    return RubyDLL.Encoding.GetString(bytes);
        //}

        public string ToString( IntPtr mrb_state ) {
            var str = RubyDLL.mrb_obj_as_string( mrb_state, this );
            int length = 0;
            IntPtr ptr = RubyDLL.mrb_string_value_cstr( mrb_state, ref str );
            unsafe {
                byte* p = ( byte* )ptr;
                while ( *p != 0 ) {
                    length++;
                    p++;
                }
            }

			byte[] bytes = new byte[ length ];
            Marshal.Copy( ptr, bytes, 0, length );
            return RubyDLL.Encoding.GetString( bytes );
        }
		
    }
	
}
#endif