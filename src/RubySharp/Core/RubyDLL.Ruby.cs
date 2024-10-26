#if !MRUBY
namespace RubySharp {
	
	using System;
	using System.IO;
	using System.Runtime.InteropServices;
	

	public static partial class RubyDLL {
		
		/// <summary>
		/// DLL ファイルのパスです。
		/// </summary>
		public const string RubyDll = "ruby";
		
		
		/// <summary>
		/// 从ruby中调用C#方法委托
		/// RUBY_DATA_FUNC
		/// </summary>
		/// <param name="state"></param>
		/// <param name="instance"></param>
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL RubyCSFunction ( int argc, [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 0)] R_VAL[] argv, R_VAL self );
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL RubyCSFunction2 ( R_VAL self, R_VAL argv );
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL RubyCSFunction3 ( R_VAL self );
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL RubyAllocFunction ( R_VAL klass );

		/// <summary>
		/// data type release function pointer
		/// </summary>
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void RubyDFreeFunction ( IntPtr data );
		
		/// <summary>
		/// data type release function pointer
		/// </summary>
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void RubyDMarkFunction ( IntPtr data );
		
		
		//
		// ruby_*
		//
		/// <summary>
		/// 初始化ruby虚拟机
		/// </summary>
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_ruby_init ();
		public static d_ruby_init ruby_init { get; } = FuncLoader.LoadFunction< d_ruby_init > ( RubyLibrary, "ruby_init" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_ruby_init_loadpath ();
		public static d_ruby_init_loadpath ruby_init_loadpath { get; } = FuncLoader.LoadFunction< d_ruby_init_loadpath > ( RubyLibrary, "ruby_init_loadpath" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_ruby_sysinit ( ref int argc, string[] argv );
		public static d_ruby_sysinit ruby_sysinit { get; } = FuncLoader.LoadFunction< d_ruby_sysinit > ( RubyLibrary, "ruby_sysinit" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate int d_ruby_setup ();
		public static d_ruby_setup ruby_setup { get; } = FuncLoader.LoadFunction< d_ruby_setup > ( RubyLibrary, "ruby_setup" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_ruby_finalize ();
		public static d_ruby_finalize ruby_finalize { get; } = FuncLoader.LoadFunction< d_ruby_finalize > ( RubyLibrary, "ruby_finalize" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_rb_current_vm ();
		public static d_rb_current_vm rb_current_vm { get; } = FuncLoader.LoadFunction< d_rb_current_vm > ( RubyLibrary, "rb_current_vm" );
		
		//
		// rb_eval*
		// ruby 运行函数
		// 
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_load ( R_VAL fname, int wrap );
		public static d_rb_load rb_load { get; } = FuncLoader.LoadFunction< d_rb_load > ( RubyLibrary, "rb_load" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_rb_load_protect ( R_VAL fname, int wrap, out int state );
		public static d_rb_load_protect rb_load_protect { get; } = FuncLoader.LoadFunction< d_rb_load_protect > ( RubyLibrary, "rb_load_protect" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_eval_string ( string str );
		public static d_rb_eval_string rb_eval_string { get; } = FuncLoader.LoadFunction< d_rb_eval_string > ( RubyLibrary, "rb_eval_string" );

		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_eval_string_protect ( string cstr, out int state );
		public static d_rb_eval_string_protect rb_eval_string_protect { get; } = FuncLoader.LoadFunction< d_rb_eval_string_protect > ( RubyLibrary, "rb_eval_string_protect" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_eval_string_protect ( R_VAL cstr );
		public static d_eval_string_protect eval_string_protect { get; } = FuncLoader.LoadFunction< d_eval_string_protect > ( RubyLibrary, "eval_string_protect" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_require ( string fname );
		public static d_rb_require rb_require { get; } = FuncLoader.LoadFunction< d_rb_require > ( RubyLibrary, "rb_require" );
		
		
		//
		// 数値
		// (int が 32 bit でないと不具合が生じうる)
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_INT2FIX ( int i );
		public static d_INT2FIX INT2FIX { get; } = FuncLoader.LoadFunction< d_INT2FIX > ( RubyLibrary, "INT2FIX_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_INT2NUM ( int i );
		public static d_INT2NUM INT2NUM { get; } = FuncLoader.LoadFunction< d_INT2NUM > ( RubyLibrary, "INT2NUM_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_UINT2NUM ( uint i );
		public static d_UINT2NUM UINT2NUM { get; } = FuncLoader.LoadFunction< d_UINT2NUM > ( RubyLibrary, "UINT2NUM_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_LL2NUM ( long l );
		public static d_LL2NUM LL2NUM { get; } = FuncLoader.LoadFunction< d_LL2NUM > ( RubyLibrary, "LL2NUM_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_ULL2NUM ( ulong l );
		public static d_ULL2NUM ULL2NUM { get; } = FuncLoader.LoadFunction< d_ULL2NUM > ( RubyLibrary, "ULL2NUM_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_DBL2NUM ( double d );
		public static d_DBL2NUM DBL2NUM { get; } = FuncLoader.LoadFunction< d_DBL2NUM > ( RubyLibrary, "DBL2NUM_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate int d_FIX2INT ( R_VAL o );
		public static d_FIX2INT FIX2INT { get; } = FuncLoader.LoadFunction< d_FIX2INT > ( RubyLibrary, "FIX2INT_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate int d_NUM2INT ( R_VAL o );
		public static d_NUM2INT NUM2INT { get; } = FuncLoader.LoadFunction< d_NUM2INT > ( RubyLibrary, "NUM2INT_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate uint d_NUM2UINT ( R_VAL o );
		public static d_NUM2UINT NUM2UINT { get; } = FuncLoader.LoadFunction< d_NUM2UINT > ( RubyLibrary, "NUM2UINT_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate long d_NUM2LONG ( R_VAL o );
		public static d_NUM2LONG NUM2LONG { get; } = FuncLoader.LoadFunction< d_NUM2LONG > ( RubyLibrary, "NUM2LONG_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate long d_NUM2LL ( R_VAL o );
		public static d_NUM2LL NUM2LL { get; } = FuncLoader.LoadFunction< d_NUM2LL > ( RubyLibrary, "NUM2LL_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate ulong d_NUM2ULL ( R_VAL o );
		public static d_NUM2ULL NUM2ULL { get; } = FuncLoader.LoadFunction< d_NUM2ULL > ( RubyLibrary, "NUM2ULL_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate double d_NUM2DBL ( R_VAL o );
		public static d_NUM2DBL NUM2DBL { get; } = FuncLoader.LoadFunction< d_NUM2DBL > ( RubyLibrary, "NUM2DBL_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate char d_NUM2CHR ( R_VAL o );
		public static d_NUM2CHR NUM2CHR { get; } = FuncLoader.LoadFunction< d_NUM2CHR > ( RubyLibrary, "NUM2CHR_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate bool d_r_test ( R_VAL o );
		public static d_r_test r_test { get; } = FuncLoader.LoadFunction< d_r_test > ( RubyLibrary, "r_test" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate bool d_r_char_to_sym ( string str );
		public static d_r_char_to_sym r_char_to_sym { get; } = FuncLoader.LoadFunction< d_r_char_to_sym > ( RubyLibrary, "r_char_to_sym" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate rb_vtype d_rb_type ( R_VAL o );
		public static d_rb_type r_type { get; } = FuncLoader.LoadFunction< d_rb_type > ( RubyLibrary, "rb_type_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate bool d_rb_eql_p ( R_VAL o );
		public static d_rb_eql_p rb_integer_type_p { get; } = FuncLoader.LoadFunction< d_rb_eql_p > ( RubyLibrary, "rb_integer_type_p" );
		public static d_rb_eql_p rb_symbol_p { get; } = FuncLoader.LoadFunction< d_rb_eql_p > ( RubyLibrary, "RB_SYMBOL_P" );
		public static d_rb_eql_p mrb_float_p { get; } = FuncLoader.LoadFunction< d_rb_eql_p > ( RubyLibrary, "RB_FLOAT_TYPE_P" );
		
		
		//
		// 文字列
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_str_new_static ( byte[] cstr, long len );
		public static d_rb_str_new_static r_str_new_static { get; } = FuncLoader.LoadFunction< d_rb_str_new_static > ( RubyLibrary, "rb_str_new_static" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_utf8_str_new_static ( string str, long len );
		public static d_rb_utf8_str_new_static rb_utf8_str_new_static { get; } = FuncLoader.LoadFunction< d_rb_utf8_str_new_static > ( RubyLibrary, "rb_utf8_str_new_static" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_obj_as_string ( R_VAL obj );
		public static d_rb_obj_as_string rb_obj_as_string { get; } = FuncLoader.LoadFunction< d_rb_obj_as_string > ( RubyLibrary, "rb_obj_as_string" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate string d_RSTRING_PTR ( R_VAL str );
		public static d_RSTRING_PTR RSTRING_PTR { get; } = FuncLoader.LoadFunction< d_RSTRING_PTR > ( RubyLibrary, "RSTRING_PTR" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_rb_string_value_cstr ( ref R_VAL v_ptr );
		public static d_rb_string_value_cstr r_string_value_cstr { get; } = FuncLoader.LoadFunction< d_rb_string_value_cstr > ( RubyLibrary, "rb_string_value_cstr" );

		
		public static string StringValuePtr ( R_VAL v ) {
			int length = 0;
			IntPtr ptr = r_string_value_cstr ( ref v );
			unsafe {
				byte* p = ( byte* )ptr;
				while ( *p != 0 ) {
					length++;
					p++;
				}
			}

			byte[] bytes = new byte[length];
			Marshal.Copy ( ptr, bytes, 0, length );
			return Encoding.GetString ( bytes );
		}
		
		
		//
		// Symbol
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		private delegate R_VAL d_rb_intern ( string name );
		private static d_rb_intern rb_intern { get; } = FuncLoader.LoadFunction< d_rb_intern > ( RubyLibrary, "rb_intern" );

		
		//
		// 定数
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_rb_define_const ( IntPtr klass, string name, R_VAL val );
		public static d_rb_define_const r_define_const { get; } = FuncLoader.LoadFunction< d_rb_define_const > ( RubyLibrary, "rb_define_const" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_rb_define_global_const ( IntPtr mrb_state, string name, R_VAL val );
		public static d_rb_define_global_const rb_define_global_const { get; } = FuncLoader.LoadFunction< d_rb_define_global_const > ( RubyLibrary, "rb_define_global_const" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_const_get ( R_VAL klass, ulong sym );
		public static d_rb_const_get rb_const_get { get; } = FuncLoader.LoadFunction< d_rb_const_get > ( RubyLibrary, "rb_const_get" );
		
		
		//
		// module, class
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_define_class ( string name, R_VAL super );
		public static d_rb_define_class r_define_class { get; } = FuncLoader.LoadFunction< d_rb_define_class > ( RubyLibrary, "rb_define_class" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_define_class_under ( R_VAL outer, string name, R_VAL super );
		public static d_rb_define_class_under r_define_class_under { get; } = FuncLoader.LoadFunction< d_rb_define_class_under > ( RubyLibrary, "rb_define_class_under" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_define_module ( string name );
		public static d_rb_define_module r_define_module { get; } = FuncLoader.LoadFunction< d_rb_define_module > ( RubyLibrary, "rb_define_module" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_define_module_under ( R_VAL outer, string name );
		public static d_rb_define_module_under r_define_module_under { get; } = FuncLoader.LoadFunction< d_rb_define_module_under > ( RubyLibrary, "rb_define_module_under" );
		
		
		//
		// メソッド
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_rb_define_method ( R_VAL klass, string name, [MarshalAs ( UnmanagedType.FunctionPtr )] RubyCSFunction func, int argc );
		public static d_rb_define_method r_define_method { get; } = FuncLoader.LoadFunction< d_rb_define_method > ( RubyLibrary, "rb_define_method" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_rb_define_module_function ( R_VAL klass, string name, RubyCSFunction func, int argc );
		public static d_rb_define_module_function r_define_module_function { get; } = FuncLoader.LoadFunction< d_rb_define_module_function > ( RubyLibrary, "rb_define_module_function" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_rb_define_singleton_method ( R_VAL klass, string name, RubyCSFunction func, int argc );
		public static d_rb_define_singleton_method r_define_singleton_method { get; } = FuncLoader.LoadFunction< d_rb_define_singleton_method > ( RubyLibrary, "rb_define_singleton_method" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_rb_define_alloc_func ( R_VAL klass, string name, RubyAllocFunction func, int argc );
		public static d_rb_define_alloc_func rb_define_alloc_func { get; } = FuncLoader.LoadFunction< d_rb_define_alloc_func > ( RubyLibrary, "rb_define_alloc_func" );

		
		//
		// funcall
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_funcall ( R_VAL obj, R_VAL funcName, int argc );
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate R_VAL d_mrb_funcall_1 ( R_VAL obj, R_VAL funcName, int argc, R_VAL arg1 );

		public static d_mrb_funcall r_funcall { get; } = FuncLoader.LoadFunction< d_mrb_funcall > ( RubyLibrary, "rb_funcall" );

		public static d_mrb_funcall_1 r_funcall_1 { get; } = FuncLoader.LoadFunction< d_mrb_funcall_1 > ( RubyLibrary, "rb_funcall" );
		
		
		// 変数
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_iv_get ( R_VAL obj, string name );
		public static d_rb_iv_get r_iv_get { get; } = FuncLoader.LoadFunction< d_rb_iv_get > ( RubyLibrary, "rb_iv_get" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_mrb_iv_set ( R_VAL obj, string name, R_VAL v );
		public static d_mrb_iv_set r_iv_set { get; } = FuncLoader.LoadFunction< d_mrb_iv_set > ( RubyLibrary, "rb_iv_set" );

		// [UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		// public delegate R_VAL d_mrb_obj_iv_get ( IntPtr obj, mrb_sym sym );
		// public static d_mrb_obj_iv_get mrb_obj_iv_get { get; } = FuncLoader.LoadFunction< d_mrb_obj_iv_get > ( RubyLibrary, "mrb_obj_iv_get" );
		//
		// [UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		// public delegate void d_mrb_obj_iv_set ( IntPtr obj, mrb_sym sym, R_VAL v );
		// public static d_mrb_obj_iv_set mrb_obj_iv_set { get; } = FuncLoader.LoadFunction< d_mrb_obj_iv_set > ( RubyLibrary, "mrb_obj_iv_set" );
		
		
		//
		// 配列
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_ary_new ();
		public static d_rb_ary_new r_ary_new { get; } = FuncLoader.LoadFunction< d_rb_ary_new > ( RubyLibrary, "rb_ary_new" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_ary_push ( R_VAL array, R_VAL item );
		public static d_rb_ary_push r_ary_push { get; } = FuncLoader.LoadFunction< d_rb_ary_push > ( RubyLibrary, "rb_ary_push" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_ary_aref1 ( R_VAL ary, R_VAL arg );
		public static d_rb_ary_aref1 r_ary_ref { get; } = FuncLoader.LoadFunction< d_rb_ary_aref1 > ( RubyLibrary, "rb_ary_aref1" );
		
		
		//
		// Hash
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_hash_new ();
		public static d_rb_hash_new r_hash_new { get; } = FuncLoader.LoadFunction< d_rb_hash_new > ( RubyLibrary, "rb_hash_new" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_hash_aset ( R_VAL hash, R_VAL key, R_VAL val );
		public static d_rb_hash_aset r_hash_set { get; } = FuncLoader.LoadFunction< d_rb_hash_aset > ( RubyLibrary, "rb_hash_aset" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_hash_fetch ( R_VAL hash, R_VAL key );
		public static d_rb_hash_fetch r_hash_get { get; } = FuncLoader.LoadFunction< d_rb_hash_fetch > ( RubyLibrary, "rb_hash_fetch" );
		
		
		//
		// Data
		//
		
		
		
		//
		// 例外
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		private delegate void d_ruby_xmalloc ( long size );
		private static d_ruby_xmalloc r_malloc { get; } = FuncLoader.LoadFunction < d_ruby_xmalloc >( RubyLibrary, "ruby_xmalloc" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		private delegate void d_rb_raise ( IntPtr exc, byte[] fmt );
		private static d_rb_raise r_raise { get; } = FuncLoader.LoadFunction< d_rb_raise > ( RubyLibrary, "rb_raise" );
		
		
		//
		// そのほか
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_inspect ( R_VAL obj );
		public static d_rb_inspect r_inspect { get; } = FuncLoader.LoadFunction< d_rb_inspect > ( RubyLibrary, "rb_inspect" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_obj_inspect ( R_VAL obj );
		public static d_rb_obj_inspect rb_obj_inspect { get; } = FuncLoader.LoadFunction< d_rb_obj_inspect > ( RubyLibrary, "rb_obj_inspect" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_vm_top_self ();
		public static d_rb_vm_top_self rb_vm_top_self { get; } = FuncLoader.LoadFunction< d_rb_vm_top_self > ( RubyLibrary, "rb_vm_top_self" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_errinfo ();
		public static d_rb_errinfo rb_errinfo { get; } = FuncLoader.LoadFunction< d_rb_errinfo > ( RubyLibrary, "rb_errinfo" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_exc_clear ( IntPtr mrb_state );
		public static d_mrb_exc_clear mrb_exc_clear { get; } = FuncLoader.LoadFunction< d_mrb_exc_clear > ( RubyLibrary, "mrb_exc_clear" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_rb_get_backtrace ( R_VAL exc );
		public static d_rb_get_backtrace r_get_backtrace { get; } = FuncLoader.LoadFunction< d_rb_get_backtrace > ( RubyLibrary, "rb_get_backtrace" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate R_VAL d_exc_backtrace ( R_VAL exc );
		public static d_exc_backtrace r_exc_backtrace { get; } = FuncLoader.LoadFunction< d_exc_backtrace > ( RubyLibrary, "exc_backtrace" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate int d_rb_eql ( R_VAL exc );
		public static d_rb_eql rb_eql { get; } = FuncLoader.LoadFunction< d_rb_eql > ( RubyLibrary, "rb_eql" );
	}
	
	
	/// <summary>
	/// ruby中的值类型
	/// (详细参见ruby/ruby.h)
	/// ruby 3.0.0 2020-12-25
	/// </summary>
	public enum rb_vtype {
		RUBY_T_NONE   = 0x00,

		RUBY_T_OBJECT = 0x01,
		RUBY_T_CLASS  = 0x02,
		RUBY_T_MODULE = 0x03,
		RUBY_T_FLOAT  = 0x04,
		RUBY_T_STRING = 0x05,
		RUBY_T_REGEXP = 0x06,
		RUBY_T_ARRAY  = 0x07,
		RUBY_T_HASH   = 0x08,
		RUBY_T_STRUCT = 0x09,
		RUBY_T_BIGNUM = 0x0a,
		RUBY_T_FILE   = 0x0b,
		RUBY_T_DATA   = 0x0c,
		RUBY_T_MATCH  = 0x0d,
		RUBY_T_COMPLEX  = 0x0e,
		RUBY_T_RATIONAL = 0x0f,

		RUBY_T_NIL    = 0x11,
		RUBY_T_TRUE   = 0x12,
		RUBY_T_FALSE  = 0x13,
		RUBY_T_SYMBOL = 0x14,
		RUBY_T_FIXNUM = 0x15,
		RUBY_T_UNDEF  = 0x16,

		RUBY_T_IMEMO  = 0x1a, /*!< @see imemo_type */
		RUBY_T_NODE   = 0x1b,
		RUBY_T_ICLASS = 0x1c,
		RUBY_T_ZOMBIE = 0x1d,

		RUBY_T_MASK   = 0x1f
	}
	
	
	/// <summary>
	/// ruby方法参数配置 
	/// </summary>
	public struct rb_args {
		
		public int value;

		public rb_args ( int value ) {
			this.value = value;
		}
		
		public static rb_args NONE () {
			return new rb_args ( 0 );
		}

		public static rb_args ANY () {
			return new rb_args ( -1 );
		}

		public static rb_args REQ ( int n ) {
			return new rb_args ( n );
		}

		public static rb_args OPT ( int n ) {
			return new rb_args ( ( ( n ) & 0x1f ) << 13 );
		}

		public static rb_args ARGS ( int req, int opt ) {
			return rb_args.REQ ( req ) | rb_args.OPT ( opt );
		}

		public static rb_args BLOCK () {
			return new rb_args ( 1 );
		}

		public static rb_args operator | ( rb_args args1, rb_args args2 ) {
			return new rb_args ( args1.value | args2.value );
		}
		
		public static implicit operator int ( rb_args args ) {
			return args.value;
		}
	}


	[StructLayout ( LayoutKind.Explicit )]
	public struct R_VAL {

		[FieldOffset ( 0 )] 
		public UInt64 value;
		
		
		/*
		 * ruby_special_consts
		 * see special_consts.h
		 */
		public static readonly R_VAL FALSE = new R_VAL () { value = 0x00 };
		public static readonly R_VAL TRUE = new R_VAL () { value = 0x14 };
		public static readonly R_VAL NIL = new R_VAL () { value = 0x08 };
		public static readonly R_VAL DEFAULT = new R_VAL () { value = 0x34 };
		

		static public R_VAL Create ( int i ) {
			return RubyDLL.INT2FIX ( i );
		}

		static public R_VAL Create ( float f ) {
			return RubyDLL.DBL2NUM ( f );
		}

		static public R_VAL Create ( double dbl ) {
			return RubyDLL.DBL2NUM ( dbl );
		}

		static public R_VAL Create ( bool b ) {
			return b ? TRUE : FALSE;
		}

		static public R_VAL Create ( string str ) {
			return RubyDLL.rb_utf8_str_new_static ( str, str.Length );
		}

		static public R_VAL CreateNIL () {
			return NIL;
		}
		
		
		static public bool IsFixnum ( R_VAL value ) {
            return RubyDLL.rb_integer_type_p ( value );
        }
        
        static public bool IsSymbol ( R_VAL value ) {
            return RubyDLL.rb_symbol_p ( value );
        }
		
        static public bool IsNil ( R_VAL value ) {
            return RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_NIL;
        }
        
        static public bool IsFalse ( R_VAL value ) {
            return RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_FALSE;
        }
        
        static public bool IsTrue ( R_VAL value ) {
            return RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_TRUE;
        }
        
        static public bool IsFloat ( R_VAL value ) {
            return RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_FLOAT;
        }
        
        static public bool IsArray ( R_VAL value ) {
            return RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_ARRAY;
        }
        
        static public bool IsString ( R_VAL value ) {
            return RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_STRING;
        }
        
        static public bool IsHash ( R_VAL value ) {
            return RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_HASH;
        }
		
        static public bool IsObject ( R_VAL value ) {
            return RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_OBJECT;
        }
        
        static public bool IsClass ( R_VAL value ) {
            return RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_CLASS;
        }
        
        static public bool IsModule ( R_VAL value ) {
            return RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_MODULE;
        }
        
        static public bool IsIClass ( R_VAL value ) {
            return RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_ICLASS;
        }
		
        static public bool IsData ( R_VAL value ) {
            return RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_DATA;
        }
		
        static public bool IsIStruct ( R_VAL value ) {
            return RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_STRUCT;
        }
		
        static public bool IsBool ( R_VAL value ) {
			return RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_TRUE ||
				   RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_FALSE;
		}
		
		
		static public implicit operator int ( R_VAL value ) {
			if ( RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_FIXNUM ) {
				return RubyDLL.FIX2INT ( value );
			}

			if ( RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_FLOAT ) {
				return ( int )RubyDLL.NUM2DBL ( value );
			}

			return 0;
		}
		
		
		static public implicit operator float ( R_VAL value ) {
			if ( RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_FLOAT ) {
				return ( float )RubyDLL.NUM2DBL ( value );
			}

			if ( RubyDLL.r_type ( value ) == rb_vtype.RUBY_T_FIXNUM ) {
				return ( float )RubyDLL.NUM2INT ( value );
			}

			return 0f;
		}
		
		
		// static public implicit operator bool ( R_VAL value ) {
		// 	switch ( RubyDLL.r_type ( value ) ) {
		// 		case rb_vtype.RUBY_T_FALSE:
		// 		// case rb_vtype.RUBY_T_EXCEPTION:
		// 		case rb_vtype.RUBY_T_UNDEF:
		// 			return false;
		// 	}
		//
		// 	return true;
		// }
		
		
		public string ToString () {
			var str = RubyDLL.rb_obj_as_string ( this );
			int length = 0;
			IntPtr ptr = RubyDLL.r_string_value_cstr ( ref str );
			unsafe {
				byte * p = ( byte * )ptr;
				while ( *p != 0 ) {
					length++;
					p++;
				}
			}

			byte[] bytes = new byte[length];
			Marshal.Copy ( ptr, bytes, 0, length );
			return RubyDLL.Encoding.GetString ( bytes );
		}
	}

}
#endif