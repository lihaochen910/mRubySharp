#if MRUBY
namespace RubySharp {
	
	using System;
	using System.Runtime.InteropServices;
	using System.Runtime.CompilerServices;

	using mrb_float = System.Double;
	using mrb_int = System.Int32;
	using mrb_sym = System.UInt32;
	using mrb_bool = System.Boolean;


	public static partial class RubyDLL {
		
		/// <summary>
		/// DLL ファイルのパスです。
		/// </summary>
		public const string MRubyDll = "mruby";


		static RubyDLL () {
			// Console.WriteLine ( "static init." );
		}
		
		
		/// <summary>
		/// 从mruby中调用C#方法委托
		/// mrb_func_t
		/// </summary>
		/// <param name="state"></param>
		/// <param name="instance"></param>
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL RubyCSFunction( IntPtr state, R_VAL self );

		/// <summary>
		/// data type release function pointer
		/// </summary>
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL RubyDFreeFunction( IntPtr state, IntPtr data );


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
        [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
        public delegate IntPtr d_mrb_open ();
        public static d_mrb_open mrb_open { get; } = FuncLoader.LoadFunction< d_mrb_open >( RubyLibrary, "mrb_open" );


        /// <summary>
        /// 关闭并释放mruby虚拟机
        /// </summary>
        /// <param name="mrb_state"></param>
        [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
        public delegate void d_mrb_close( IntPtr mrb_state );
        public static d_mrb_close mrb_close { get; } = FuncLoader.LoadFunction< d_mrb_close >( RubyLibrary, "mrb_close" );


		//
		// rb_eval*
		// mruby 运行函数
		// 
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_load_string( IntPtr mrb_state, byte[] script_string );
		public static d_mrb_load_string mrb_load_string { get; } = FuncLoader.LoadFunction< d_mrb_load_string >( RubyLibrary, "mrb_load_string" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_load_string_cxt( IntPtr mrb_state, byte[] script_string, IntPtr mrbc_context );
		public static d_mrb_load_string_cxt mrb_load_string_cxt { get; } = FuncLoader.LoadFunction< d_mrb_load_string_cxt >( RubyLibrary, "mrb_load_string_cxt" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_load_file( IntPtr mrb_state, IntPtr file );
		public static d_mrb_load_file mrb_load_file { get; } = FuncLoader.LoadFunction< d_mrb_load_file >( RubyLibrary, "mrb_load_file" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_load_file_cxt( IntPtr mrb_state, IntPtr file, IntPtr mrbc_context );
		public static d_mrb_load_file_cxt mrb_load_file_cxt { get; } = FuncLoader.LoadFunction< d_mrb_load_file_cxt >( RubyLibrary, "mrb_load_file_cxt" );
		
		
		/// <summary>
		/// 加载mrbc字节码
		/// </summary>
		/// <param name="mrb_state"></param>
		/// <param name="byte_code"></param>
		/// <returns></returns>
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_load_irep( IntPtr mrb_state, byte[] byte_code );
		public static d_mrb_load_irep mrb_load_irep { get; } = FuncLoader.LoadFunction< d_mrb_load_irep >( RubyLibrary, "mrb_load_irep" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_load_nstring( IntPtr mrb_state, byte[] script_string, long len );
		public static d_mrb_load_nstring mrb_load_nstring { get; } = FuncLoader.LoadFunction< d_mrb_load_nstring >( RubyLibrary, "mrb_load_nstring" );


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

		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_to_int( IntPtr mrb_state, R_VAL v );

		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_to_str( IntPtr mrb_state, R_VAL v );

		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_any_to_s( IntPtr mrb_state, R_VAL obj );

		public static d_mrb_to_int mrb_to_int { get; } = FuncLoader.LoadFunction< d_mrb_to_int >( RubyLibrary, "mrb_to_int" );

		public static d_mrb_to_str mrb_to_str { get; } = FuncLoader.LoadFunction< d_mrb_to_str >( RubyLibrary, "mrb_to_str" );

		public static d_mrb_any_to_s mrb_any_to_s { get; } = FuncLoader.LoadFunction< d_mrb_any_to_s >( RubyLibrary, "mrb_any_to_s" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_float_value( IntPtr mrb_state, mrb_float f );
		public static d_mrb_float_value mrb_float_value { get; } = FuncLoader.LoadFunction< d_mrb_float_value >( RubyLibrary, "mrb_float_value_ex" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_fixnum_value( IntPtr mrb_state, mrb_int i );
		public static d_mrb_fixnum_value mrb_fixnum_value { get; } = FuncLoader.LoadFunction< d_mrb_fixnum_value >( RubyLibrary, "mrb_fixnum_value_ex" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_bool_value( mrb_bool b );
		public static d_mrb_bool_value mrb_bool_value { get; } = FuncLoader.LoadFunction< d_mrb_bool_value >( RubyLibrary, "mrb_bool_value_ex" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_nil_value();
		public static d_mrb_nil_value mrb_nil_value { get; } = FuncLoader.LoadFunction< d_mrb_nil_value >( RubyLibrary, "mrb_nil_value_ex" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_undef_value();
		public static d_mrb_undef_value mrb_undef_value { get; } = FuncLoader.LoadFunction< d_mrb_undef_value >( RubyLibrary, "mrb_undef_value_ex" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_false_value();
		public static d_mrb_false_value mrb_false_value { get; } = FuncLoader.LoadFunction< d_mrb_false_value >( RubyLibrary, "mrb_false_value_ex" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_true_value();
		public static d_mrb_true_value mrb_true_value { get; } = FuncLoader.LoadFunction< d_mrb_true_value >( RubyLibrary, "mrb_true_value_ex" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_cptr_value( IntPtr mrb_state, IntPtr p );
		public static d_mrb_cptr_value mrb_cptr_value { get; } = FuncLoader.LoadFunction< d_mrb_cptr_value >( RubyLibrary, "mrb_cptr_value_ex" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_obj_value( IntPtr p );
		public static d_mrb_obj_value mrb_obj_value { get; } = FuncLoader.LoadFunction< d_mrb_obj_value >( RubyLibrary, "mrb_obj_value_ex" );

		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_ptr( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_cptr( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_float d_mrb_float( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_int d_mrb_fixnum( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_sym d_mrb_symbol( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate rb_vtype d_mrb_type( R_VAL o );
		public static d_mrb_ptr mrb_ptr { get; } = FuncLoader.LoadFunction< d_mrb_ptr >( RubyLibrary, "mrb_ptr_ex" );
		public static d_mrb_cptr mrb_cptr { get; } = FuncLoader.LoadFunction< d_mrb_cptr >( RubyLibrary, "mrb_cptr_ex" );
		public static d_mrb_float mrb_float { get; } = FuncLoader.LoadFunction< d_mrb_float >( RubyLibrary, "mrb_float_ex" );
		public static d_mrb_fixnum mrb_fixnum { get; } = FuncLoader.LoadFunction< d_mrb_fixnum >( RubyLibrary, "mrb_fixnum_ex" );
		public static d_mrb_symbol mrb_symbol { get; } = FuncLoader.LoadFunction< d_mrb_symbol >( RubyLibrary, "mrb_symbol_ex" );
		public static d_mrb_type r_type { get; } = FuncLoader.LoadFunction< d_mrb_type >( RubyLibrary, "mrb_type_ex" );
		
		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_immediate_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_fixnum_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_symbol_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_undef_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_nil_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_false_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_true_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_float_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_array_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_string_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_hash_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_cptr_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_exception_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_free_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_object_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_class_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_module_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_iclass_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_sclass_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_proc_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_range_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_file_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_env_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_data_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_fiber_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_istruct_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_break_p( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_bool( R_VAL o );
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate bool d_mrb_test( R_VAL o );
		
		public static d_mrb_immediate_p mrb_immediate_p { get; } = FuncLoader.LoadFunction< d_mrb_immediate_p >( RubyLibrary, "mrb_immediate_p_ex" );
		public static d_mrb_fixnum_p mrb_fixnum_p { get; } = FuncLoader.LoadFunction< d_mrb_fixnum_p >( RubyLibrary, "mrb_fixnum_p_ex" );
		public static d_mrb_symbol_p mrb_symbol_p { get; } = FuncLoader.LoadFunction< d_mrb_symbol_p >( RubyLibrary, "mrb_symbol_p_ex" );
		public static d_mrb_undef_p mrb_undef_p { get; } = FuncLoader.LoadFunction< d_mrb_undef_p >( RubyLibrary, "mrb_undef_p_ex" );
		public static d_mrb_nil_p mrb_nil_p { get; } = FuncLoader.LoadFunction< d_mrb_nil_p >( RubyLibrary, "mrb_nil_p_ex" );
		public static d_mrb_false_p mrb_false_p { get; } = FuncLoader.LoadFunction< d_mrb_false_p >( RubyLibrary, "mrb_false_p_ex" );
		public static d_mrb_true_p mrb_true_p { get; } = FuncLoader.LoadFunction< d_mrb_true_p >( RubyLibrary, "mrb_true_p_ex" );
		public static d_mrb_float_p mrb_float_p { get; } = FuncLoader.LoadFunction< d_mrb_float_p >( RubyLibrary, "mrb_float_p_ex" );
		public static d_mrb_array_p mrb_array_p { get; } = FuncLoader.LoadFunction< d_mrb_array_p >( RubyLibrary, "mrb_array_p_ex" );
		public static d_mrb_string_p mrb_string_p { get; } = FuncLoader.LoadFunction< d_mrb_string_p >( RubyLibrary, "mrb_string_p_ex" );
		public static d_mrb_hash_p mrb_hash_p { get; } = FuncLoader.LoadFunction< d_mrb_hash_p >( RubyLibrary, "mrb_hash_p_ex" );
		public static d_mrb_cptr_p mrb_cptr_p { get; } = FuncLoader.LoadFunction< d_mrb_cptr_p >( RubyLibrary, "mrb_cptr_p_ex" );
		public static d_mrb_exception_p mrb_exception_p { get; } = FuncLoader.LoadFunction< d_mrb_exception_p >( RubyLibrary, "mrb_exception_p_ex" );
		public static d_mrb_free_p mrb_free_p { get; } = FuncLoader.LoadFunction< d_mrb_free_p >( RubyLibrary, "mrb_free_p_ex" );
		public static d_mrb_object_p mrb_object_p { get; } = FuncLoader.LoadFunction< d_mrb_object_p >( RubyLibrary, "mrb_object_p_ex" );
		public static d_mrb_class_p mrb_class_p { get; } = FuncLoader.LoadFunction< d_mrb_class_p >( RubyLibrary, "mrb_class_p_ex" );
		public static d_mrb_module_p mrb_module_p { get; } = FuncLoader.LoadFunction< d_mrb_module_p >( RubyLibrary, "mrb_module_p_ex" );
		public static d_mrb_iclass_p mrb_iclass_p { get; } = FuncLoader.LoadFunction< d_mrb_iclass_p >( RubyLibrary, "mrb_iclass_p_ex" );
		public static d_mrb_sclass_p mrb_sclass_p { get; } = FuncLoader.LoadFunction< d_mrb_sclass_p >( RubyLibrary, "mrb_sclass_p_ex" );
		public static d_mrb_proc_p mrb_proc_p { get; } = FuncLoader.LoadFunction< d_mrb_proc_p >( RubyLibrary, "mrb_proc_p_ex" );
		public static d_mrb_range_p mrb_range_p { get; } = FuncLoader.LoadFunction< d_mrb_range_p >( RubyLibrary, "mrb_range_p_ex" );
		// public static d_mrb_file_p mrb_file_p { get; } = FuncLoader.LoadFunction< d_mrb_file_p >( RubyLibrary, "mrb_file_p_ex" );
		public static d_mrb_env_p mrb_env_p { get; } = FuncLoader.LoadFunction< d_mrb_env_p >( RubyLibrary, "mrb_env_p_ex" );
		public static d_mrb_data_p mrb_data_p { get; } = FuncLoader.LoadFunction< d_mrb_data_p >( RubyLibrary, "mrb_data_p_ex" );
		public static d_mrb_fiber_p mrb_fiber_p { get; } = FuncLoader.LoadFunction< d_mrb_fiber_p >( RubyLibrary, "mrb_fiber_p_ex" );
		public static d_mrb_istruct_p mrb_istruct_p { get; } = FuncLoader.LoadFunction< d_mrb_istruct_p >( RubyLibrary, "mrb_istruct_p_ex" );
		public static d_mrb_break_p mrb_break_p { get; } = FuncLoader.LoadFunction< d_mrb_break_p >( RubyLibrary, "mrb_break_p_ex" );
		public static d_mrb_bool mrb_bool { get; } = FuncLoader.LoadFunction< d_mrb_bool >( RubyLibrary, "mrb_bool_ex" );
		public static d_mrb_test r_test { get; } = FuncLoader.LoadFunction< d_mrb_test >( RubyLibrary, "mrb_test_ex" );
		
		
		
		//
		// 文字列
		//
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_str_new( byte[] ptr, long len );
		public static d_mrb_str_new r_str_new { get; } = FuncLoader.LoadFunction< d_mrb_str_new >( RubyLibrary, "mrb_str_new" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_str_new_cstr( IntPtr mrb_state, byte[] ptr );
		public static d_mrb_str_new_cstr mrb_str_new_cstr { get; } = FuncLoader.LoadFunction< d_mrb_str_new_cstr >( RubyLibrary, "mrb_str_new_cstr" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_str_new_static( IntPtr mrb_state, byte[] ptr, long len );
		public static d_mrb_str_new_static mrb_str_new_static { get; } = FuncLoader.LoadFunction< d_mrb_str_new_static >( RubyLibrary, "mrb_str_new_static" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_obj_as_string( IntPtr mrb_state, R_VAL obj );
		public static d_mrb_obj_as_string mrb_obj_as_string { get; } = FuncLoader.LoadFunction< d_mrb_obj_as_string >( RubyLibrary, "mrb_obj_as_string" );


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

		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_string_value_cstr( IntPtr mrb_state, ref R_VAL v_ptr );
		public static d_mrb_string_value_cstr mrb_string_value_cstr { get; } = FuncLoader.LoadFunction< d_mrb_string_value_cstr >( RubyLibrary, "mrb_string_value_cstr" );

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
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		private delegate mrb_sym d_mrb_intern( IntPtr mrb_state, byte[] name, UInt64 s );
		private static d_mrb_intern mrb_intern { get; } = FuncLoader.LoadFunction< d_mrb_intern >( RubyLibrary, "mrb_intern" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		private delegate mrb_sym d_mrb_intern_cstr( IntPtr mrb_state, byte[] name );
		private static d_mrb_intern_cstr mrb_intern_cstr { get; } = FuncLoader.LoadFunction< d_mrb_intern_cstr >( RubyLibrary, "mrb_intern_cstr" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		private delegate mrb_sym d_mrb_intern_static( IntPtr mrb_state, byte[] name, UInt64 s );
		private static d_mrb_intern_static mrb_intern_static { get; } = FuncLoader.LoadFunction< d_mrb_intern_static >( RubyLibrary, "mrb_intern_static" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		private delegate Int64 d_mrb_obj_id( R_VAL obj );
		private static d_mrb_obj_id mrb_obj_id { get; } = FuncLoader.LoadFunction< d_mrb_obj_id >( RubyLibrary, "mrb_obj_id" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		private delegate IntPtr d_mrb_obj_class( IntPtr mrb_state, R_VAL obj );
		private static d_mrb_obj_class mrb_obj_class { get; } = FuncLoader.LoadFunction< d_mrb_obj_class >( RubyLibrary, "mrb_obj_class" );


		//
		// 定数
		//
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_define_const( IntPtr mrb_state, IntPtr klass, string name, R_VAL val );
		public static d_mrb_define_const r_define_const { get; } = FuncLoader.LoadFunction< d_mrb_define_const >( RubyLibrary, "mrb_define_const" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_define_global_const( IntPtr mrb_state, string name, R_VAL val );
		public static d_mrb_define_global_const mrb_define_global_const { get; } = FuncLoader.LoadFunction< d_mrb_define_global_const >( RubyLibrary, "mrb_define_global_const" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_const_get( IntPtr mrb_state, R_VAL obj, mrb_sym sym );
		public static d_mrb_const_get mrb_const_get { get; } = FuncLoader.LoadFunction< d_mrb_const_get >( RubyLibrary, "mrb_const_get" );


		//
		// module, class
		//
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_define_class( IntPtr mrb_state, string name, IntPtr super );
		public static d_mrb_define_class r_define_class { get; } = FuncLoader.LoadFunction< d_mrb_define_class >( RubyLibrary, "mrb_define_class" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_define_class_under( IntPtr mrb_state, IntPtr outer, string name, IntPtr super );
		public static d_mrb_define_class_under r_define_class_under { get; } = FuncLoader.LoadFunction< d_mrb_define_class_under >( RubyLibrary, "mrb_define_class_under" );


		// [DllImport ( MRubyDll, EntryPoint = "mrb_set_instance_tt" )]
		// public static extern void MRB_SET_INSTANCE_TT ( IntPtr c, rb_vtype tt );

		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_class_get( IntPtr mrb_state, string name );
		public static d_mrb_class_get mrb_class_get { get; } = FuncLoader.LoadFunction< d_mrb_class_get >( RubyLibrary, "mrb_class_get" );

		//[DllImport(MRubyDll)]
		//public static extern R_VAL mrb_class_new_instance(IntPtr mrb_state, mrb_int argc, R_VAL[] argv, IntPtr c);
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_obj_new( IntPtr mrb_state, IntPtr c, mrb_int argc, R_VAL[] argv );
		public static d_mrb_obj_new mrb_obj_new { get; } = FuncLoader.LoadFunction< d_mrb_obj_new >( RubyLibrary, "mrb_obj_new" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_define_module( IntPtr mrb_state, string name );
		public static d_mrb_define_module r_define_module { get; } = FuncLoader.LoadFunction< d_mrb_define_module >( RubyLibrary, "mrb_define_module" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_define_module_under( IntPtr mrb_state, IntPtr outer, string name, IntPtr super );
		public static d_mrb_define_module_under r_define_module_under { get; } = FuncLoader.LoadFunction< d_mrb_define_module_under >( RubyLibrary, "mrb_define_module_under" );

		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_module_get( IntPtr mrb_state, string name );
		public static d_mrb_module_get mrb_module_get { get; } = FuncLoader.LoadFunction< d_mrb_module_get >( RubyLibrary, "mrb_module_get" );




		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_get_args( IntPtr state, string format, out IntPtr argv, out int argc, out R_VAL block );
		public static d_mrb_get_args mrb_get_args { get; } = FuncLoader.LoadFunction< d_mrb_get_args >( RubyLibrary, "mrb_get_args" );



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
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_define_method( IntPtr state, IntPtr klass, string name, RubyCSFunction func, rb_args aspec );
		public static d_mrb_define_method r_define_method { get; } = FuncLoader.LoadFunction< d_mrb_define_method >( RubyLibrary, "mrb_define_method" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_define_class_method( IntPtr state, IntPtr klass, string name, RubyCSFunction func, rb_args aspec );
		public static d_mrb_define_class_method r_define_class_method { get; } = FuncLoader.LoadFunction< d_mrb_define_class_method >( RubyLibrary, "mrb_define_class_method" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_define_module_function( IntPtr state, IntPtr klass, string name, RubyCSFunction func, rb_args aspec );
		public static d_mrb_define_module_function r_define_module_function { get; } = FuncLoader.LoadFunction< d_mrb_define_module_function >( RubyLibrary, "mrb_define_module_function" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_define_singleton_method( IntPtr state, IntPtr klass, string name, RubyCSFunction func, rb_args aspec );
		public static d_mrb_define_singleton_method r_define_singleton_method { get; } = FuncLoader.LoadFunction< d_mrb_define_singleton_method >( RubyLibrary, "mrb_define_singleton_method" );


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
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_funcall( IntPtr mrb_state, R_VAL obj, string funcName, int argc );
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate R_VAL d_mrb_funcall_1( IntPtr mrb_state, R_VAL obj, string funcName, int argc, R_VAL arg1 );

		public static d_mrb_funcall r_funcall { get; } = FuncLoader.LoadFunction< d_mrb_funcall >( RubyLibrary, "mrb_funcall" );

		public static d_mrb_funcall_1 r_funcall_1 { get; } = FuncLoader.LoadFunction< d_mrb_funcall_1 >( RubyLibrary, "mrb_funcall" );

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
		public static R_VAL[] GetFunctionArgs ( IntPtr mrb_state, bool withBlock = false ) {
			R_VAL[] values;
			IntPtr argvPointer;
			R_VAL value = R_VAL.NIL;
			int i, argc, size;
			R_VAL block;

			RubyDLL.mrb_get_args ( mrb_state, "*&", out argvPointer, out argc, out block );

			int valueCount = argc;
			if ( withBlock ) {
				valueCount++;
			}

			values = new R_VAL[ valueCount ]; // Include Block
			size = Marshal.SizeOf( typeof( R_VAL ) );
			for ( i = 0; i < argc; i++ ) {
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
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_iv_get( IntPtr mrb_state, R_VAL obj, mrb_sym sym );
		public static d_mrb_iv_get r_iv_get { get; } = FuncLoader.LoadFunction< d_mrb_iv_get >( RubyLibrary, "mrb_iv_get" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_iv_set( IntPtr mrb_state, R_VAL obj, mrb_sym sym, R_VAL v );
		public static d_mrb_iv_set r_iv_set { get; } = FuncLoader.LoadFunction< d_mrb_iv_set >( RubyLibrary, "mrb_iv_set" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_obj_iv_get( IntPtr mrb_state, IntPtr obj, mrb_sym sym );
		public static d_mrb_obj_iv_get mrb_obj_iv_get { get; } = FuncLoader.LoadFunction< d_mrb_obj_iv_get >( RubyLibrary, "mrb_obj_iv_get" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_obj_iv_set( IntPtr mrb_state, IntPtr obj, mrb_sym sym, R_VAL v );
		public static d_mrb_obj_iv_set mrb_obj_iv_set { get; } = FuncLoader.LoadFunction< d_mrb_obj_iv_set >( RubyLibrary, "mrb_obj_iv_set" );


		//
		// 配列
		//
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_ary_new();
		public static d_mrb_ary_new r_ary_new { get; } = FuncLoader.LoadFunction< d_mrb_ary_new >( RubyLibrary, "mrb_ary_new" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_ary_push( IntPtr mrb_state, R_VAL array, R_VAL value );
		public static d_mrb_ary_push r_ary_push { get; } = FuncLoader.LoadFunction< d_mrb_ary_push >( RubyLibrary, "mrb_ary_push" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_ary_ref( IntPtr mrb_state, R_VAL ary, mrb_int n );
		public static d_mrb_ary_ref r_ary_ref { get; } = FuncLoader.LoadFunction< d_mrb_ary_ref >( RubyLibrary, "mrb_ary_ref" );
		

		//
		// Hash
		//
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_hash_new();
		public static d_mrb_hash_new r_hash_new { get; } = FuncLoader.LoadFunction< d_mrb_hash_new >( RubyLibrary, "mrb_hash_new" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_hash_set( IntPtr mrb_state, R_VAL hash, R_VAL key, R_VAL val );
		public static d_mrb_hash_set r_hash_set { get; } = FuncLoader.LoadFunction< d_mrb_hash_set >( RubyLibrary, "mrb_hash_set" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_hash_get( IntPtr mrb_state, R_VAL hash, R_VAL key );
		public static d_mrb_hash_get r_hash_get { get; } = FuncLoader.LoadFunction< d_mrb_hash_get >( RubyLibrary, "mrb_hash_get" );


		//
		// Context
		//
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrbc_context_new( IntPtr mrb_state );
		public static d_mrbc_context_new mrbc_context_new { get; } = FuncLoader.LoadFunction< d_mrbc_context_new >( RubyLibrary, "mrbc_context_new" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrbc_context_free( IntPtr mrb_state, IntPtr mrbc_context );
		public static d_mrbc_context_free mrbc_context_free { get; } = FuncLoader.LoadFunction< d_mrbc_context_free >( RubyLibrary, "mrbc_context_free" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrbc_filename( IntPtr mrb_state, IntPtr mrbc_context, string name );
		public static d_mrbc_filename mrbc_filename { get; } = FuncLoader.LoadFunction< d_mrbc_filename >( RubyLibrary, "mrbc_filename" );

		
		//
		// Data
		// 
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_data_object_alloc( IntPtr mrb_state, IntPtr klass, IntPtr datap, IntPtr type );
		public static d_mrb_data_object_alloc mrb_data_object_alloc { get; } = FuncLoader.LoadFunction< d_mrb_data_object_alloc >( RubyLibrary, "mrb_data_object_alloc" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_data_init( R_VAL v, IntPtr ptr, IntPtr data_type );
		public static d_mrb_data_init mrb_data_init { get; } = FuncLoader.LoadFunction< d_mrb_data_init >( RubyLibrary, "mrb_data_init_ex" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_data_get_ptr( IntPtr mrb_state, R_VAL obj, IntPtr type );
		public static d_mrb_data_get_ptr mrb_data_get_ptr { get; } = FuncLoader.LoadFunction< d_mrb_data_get_ptr >( RubyLibrary, "mrb_data_get_ptr" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_set_instance_tt( IntPtr klass, rb_vtype tt );
		public static d_mrb_set_instance_tt mrb_set_instance_tt { get; } = FuncLoader.LoadFunction< d_mrb_set_instance_tt >( RubyLibrary, "mrb_set_instance_tt" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_data_wrap_struct( IntPtr mrb_state, IntPtr klass, IntPtr data_type, IntPtr ptr );
		public static d_mrb_data_wrap_struct mrb_data_wrap_struct { get; } = FuncLoader.LoadFunction< d_mrb_data_wrap_struct >( RubyLibrary, "mrb_data_wrap_struct" );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_data_wrap_struct_obj( IntPtr mrb_state, IntPtr klass, IntPtr data_type, IntPtr ptr );
		public static d_mrb_data_wrap_struct_obj mrb_data_wrap_struct_obj { get; } = FuncLoader.LoadFunction< d_mrb_data_wrap_struct_obj >( RubyLibrary, "mrb_data_wrap_struct_obj" );
		
		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_set_data_type( IntPtr mrb_state, R_VAL v, IntPtr data_type_ptr );
		public static d_mrb_set_data_type mrb_set_data_type { get; } = FuncLoader.LoadFunction< d_mrb_set_data_type >( RubyLibrary, "mrb_set_data_type" );
		
		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_set_data_ptr ( IntPtr mrb_state, R_VAL v, IntPtr data_ptr );
		public static d_mrb_set_data_ptr mrb_set_data_ptr { get; } = FuncLoader.LoadFunction< d_mrb_set_data_ptr >( RubyLibrary, "mrb_set_data_ptr" );

		
		//
		// 例外
		//
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		private delegate IntPtr d_mrb_malloc( IntPtr mrb_state, long len );
		private static d_mrb_malloc mrb_malloc { get; } = FuncLoader.LoadFunction < d_mrb_malloc >( RubyLibrary, "mrb_malloc" );

		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		private delegate void d_mrb_raise ( IntPtr mrb_state, IntPtr obj, byte[] msg );
		private static d_mrb_raise mrb_raise { get; } = FuncLoader.LoadFunction< d_mrb_raise >( RubyLibrary, "mrb_raise" );


		//
		// そのほか
		//
		//private const int FL_FREEZE = (1 << 10);
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_inspect( IntPtr mrb_state, R_VAL obj );
		public static d_mrb_inspect mrb_inspect { get; } = FuncLoader.LoadFunction< d_mrb_inspect >( RubyLibrary, "mrb_inspect" );

		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_top_self( IntPtr mrb_state );
		public static d_mrb_top_self mrb_top_self { get; } = FuncLoader.LoadFunction< d_mrb_top_self >( RubyLibrary, "mrb_top_self" );

		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate bool d_mrb_has_exc( IntPtr mrb_state );
		public static d_mrb_has_exc mrb_has_exc { get; } = FuncLoader.LoadFunction< d_mrb_has_exc >( RubyLibrary, "mrb_has_exc" );
		
		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_exc_clear( IntPtr mrb_state );
		public static d_mrb_exc_clear mrb_exc_clear { get; } = FuncLoader.LoadFunction< d_mrb_exc_clear >( RubyLibrary, "mrb_exc_clear" );
		
		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_get_exc_value( IntPtr mrb_state );
		public static d_mrb_get_exc_value mrb_get_exc_value { get; } = FuncLoader.LoadFunction< d_mrb_get_exc_value >( RubyLibrary, "mrb_get_exc_value" );

		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_exc_detail( IntPtr mrb_state );
		public static d_mrb_exc_detail mrb_exc_detail { get; } = FuncLoader.LoadFunction< d_mrb_exc_detail >( RubyLibrary, "mrb_exc_detail" );

		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_get_backtrace( IntPtr mrb_state );
		public static d_mrb_get_backtrace r_get_backtrace { get; } = FuncLoader.LoadFunction< d_mrb_get_backtrace >( RubyLibrary, "mrb_get_backtrace" );

		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_exc_backtrace( IntPtr mrb_state, R_VAL exc );
		public static d_mrb_exc_backtrace r_exc_backtrace { get; } = FuncLoader.LoadFunction< d_mrb_exc_backtrace >( RubyLibrary, "mrb_exc_backtrace" );

		
		//
		// GC
		//
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate int d_mrb_gc_arena_save( IntPtr mrb_state );
		public static d_mrb_gc_arena_save mrb_gc_arena_save { get; } = FuncLoader.LoadFunction< d_mrb_gc_arena_save >( RubyLibrary, "mrb_gc_arena_save_ex" );
		
		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_gc_arena_restore( IntPtr mrb_state, int idx );
		public static d_mrb_gc_arena_restore mrb_gc_arena_restore { get; } = FuncLoader.LoadFunction< d_mrb_gc_arena_restore >( RubyLibrary, "mrb_gc_arena_restore_ex" );

		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_garbage_collect( IntPtr mrb_state );
		public static d_mrb_garbage_collect mrb_garbage_collect { get; } = FuncLoader.LoadFunction< d_mrb_garbage_collect >( RubyLibrary, "mrb_garbage_collect" );
		
		
		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void d_mrb_full_gc( IntPtr mrb_state );
		public static d_mrb_full_gc mrb_full_gc { get; } = FuncLoader.LoadFunction< d_mrb_full_gc >( RubyLibrary, "mrb_full_gc" );

		
		
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
		RUBY_T_FLOAT,     /*   2 */
		RUBY_T_INTEGER,   /*   3 */
		RUBY_T_SYMBOL,    /*   4 */
		RUBY_T_UNDEF,     /*   5 */
		RUBY_T_CPTR,      /*   6 */
		RUBY_T_FREE,      /*   7 */
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
		RUBY_T_ISTRUCT,   /*  22 */
		RUBY_T_BREAK,     /*  23 */
		RUBY_T_MAXDEFINE  /*  24 */
	}
	
	
	/// <summary>
	/// mruby方法参数配置 
	/// </summary>
	[StructLayout ( LayoutKind.Sequential )]
	public struct rb_args {
		
		public UInt32 Value;

		public rb_args( UInt32 value ) {
			this.Value = value;
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
			return rb_args.REQ( req ) | rb_args.OPT( opt );
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
	[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
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
            return RubyDLL.mrb_float_value( mrb_state, dbl );
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