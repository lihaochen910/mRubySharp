using System;
using System.IO;
using System.Runtime.InteropServices;

using mrb_float = System.Double;
using mrb_int = System.Int32;
using mrb_sym = System.UInt32;
using mrb_bool = System.Byte;


namespace CandyFramework.mRuby {
	
    /// <summary>
    /// mRubyのポーティング。
    /// </summary>
    /// <remarks>
    /// 行き当たりばったりで機能を追加しています。
    /// </remarks>
    public static class mRubyDLL {
	    
        /// <summary>
        /// DLL ファイルのパスです。
        /// </summary>
        public const string MRubyDll = "mruby";

        /// <summary>
        /// 使用平台特定API加载动态库
        /// </summary>
        public static IntPtr MRubyLibrary = GetNativeLibrary ();

		private static IntPtr GetNativeLibrary () {
			
			var ret = IntPtr.Zero;

			// Load bundled library
			var assemblyLocation = Path.GetDirectoryName ( typeof ( mRubyDLL ).Assembly.Location );
			if ( CurrentPlatform.OS == OS.Windows && Environment.Is64BitProcess )
				ret = FuncLoader.LoadLibrary ( Path.Combine ( assemblyLocation, "x64/mruby.dll" ) );
			else if ( CurrentPlatform.OS == OS.Windows && !Environment.Is64BitProcess )
				ret = FuncLoader.LoadLibrary ( Path.Combine ( assemblyLocation, "x86/mruby.dll" ) );
			else if ( CurrentPlatform.OS == OS.Linux && Environment.Is64BitProcess )
				ret = FuncLoader.LoadLibrary ( Path.Combine ( assemblyLocation, "x64/mruby.so.0" ) );
			else if ( CurrentPlatform.OS == OS.Linux && !Environment.Is64BitProcess )
				ret = FuncLoader.LoadLibrary ( Path.Combine ( assemblyLocation, "x86/mruby.so.0" ) );
			else if ( CurrentPlatform.OS == OS.MacOSX )
				ret = FuncLoader.LoadLibrary ( Path.Combine ( assemblyLocation, "mruby.dylib" ) );

			// Load system library
			if ( ret == IntPtr.Zero ) {
				if ( CurrentPlatform.OS == OS.Windows )
					ret = FuncLoader.LoadLibrary ( "mruby.dll" );
				else if ( CurrentPlatform.OS == OS.Linux )
					ret = FuncLoader.LoadLibrary ( "mruby.so.0" );
				else
					ret = FuncLoader.LoadLibrary ( "mruby.dylib" );
			}

			// Welp, all failed, PANIC!!!
			if ( ret == IntPtr.Zero ) {
				throw new Exception ( "Failed to load mruby dynamic library." );
			}

			return ret;
		}

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
		/// mrb_func_t
		/// </summary>
		/// <param name="state"></param>
		/// <param name="instance"></param>
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value MRubyCSFunction ( IntPtr state, mrb_value self );

		/// <summary>
		/// data type release function pointer
		/// </summary>
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value MRubyDFreeFunction ( IntPtr state, IntPtr data );


		/// <summary>
		/// Custom data type description.
		/// </summary>
		[StructLayout ( LayoutKind.Sequential )]
		public struct mrb_data_type {
			public string struct_name;
			public MRubyDFreeFunction dfree;
		}


		/// <summary>
		/// C 用に文字列を byte の配列に変換します。
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static byte[] ToCBytes ( string str ) {
			return Encoding.GetBytes ( str + '\0' );
		}


		//
        // ruby_*
        //
        /// <summary>
        /// 初始化mruby虚拟机
        /// </summary>
        /// <returns>pointer</returns>
        [UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
        public delegate IntPtr d_mrb_open ();

        public static d_mrb_open mrb_open { get; } = FuncLoader.LoadFunction< d_mrb_open > ( MRubyLibrary, "mrb_open" );

        /// <summary>
        /// 关闭并释放mruby虚拟机
        /// </summary>
        /// <param name="mrb_state"></param>
        [UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
        public delegate void d_mrb_close ( IntPtr mrb_state );

        public static d_mrb_close mrb_close { get; } = FuncLoader.LoadFunction< d_mrb_close > ( MRubyLibrary, "mrb_close" );

		//
		// rb_eval*
		// mruby 运行函数
		// 
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_load_string ( IntPtr mrb_state, byte[] script_string );

		public static d_mrb_load_string mrb_load_string { get; } = FuncLoader.LoadFunction< d_mrb_load_string > ( MRubyLibrary, "mrb_load_string" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_load_string_cxt ( IntPtr mrb_state, byte[] script_string, IntPtr mrbc_context );

		public static d_mrb_load_string_cxt mrb_load_string_cxt { get; } = FuncLoader.LoadFunction< d_mrb_load_string_cxt > ( MRubyLibrary, "mrb_load_string_cxt" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_load_file ( IntPtr mrb_state, IntPtr file );

		public static d_mrb_load_file mrb_load_file { get; } = FuncLoader.LoadFunction< d_mrb_load_file > ( MRubyLibrary, "mrb_load_file" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_load_file_cxt ( IntPtr mrb_state, IntPtr file, IntPtr mrbc_context );

		public static d_mrb_load_file_cxt mrb_load_file_cxt { get; } = FuncLoader.LoadFunction< d_mrb_load_file_cxt > ( MRubyLibrary, "mrb_load_file_cxt" );
		
		/// <summary>
		/// 加载mrbc字节码
		/// </summary>
		/// <param name="mrb_state"></param>
		/// <param name="byte_code"></param>
		/// <returns></returns>
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_load_irep ( IntPtr mrb_state, byte[] byte_code );

		public static d_mrb_load_irep mrb_load_irep { get; } = FuncLoader.LoadFunction< d_mrb_load_irep > ( MRubyLibrary, "mrb_load_irep" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_load_nstring ( IntPtr mrb_state, byte[] script_string, long len );

		public static d_mrb_load_nstring mrb_load_nstring { get; } = FuncLoader.LoadFunction< d_mrb_load_nstring > ( MRubyLibrary, "mrb_load_nstring" );


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

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_to_int ( IntPtr mrb_state, mrb_value v );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_to_str ( IntPtr mrb_state, mrb_value v );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_any_to_s ( IntPtr mrb_state, mrb_value obj );

		public static d_mrb_to_int mrb_to_int { get; } = FuncLoader.LoadFunction< d_mrb_to_int > ( MRubyLibrary, "mrb_to_int" );

		public static d_mrb_to_str mrb_to_str { get; } = FuncLoader.LoadFunction< d_mrb_to_str > ( MRubyLibrary, "mrb_to_str" );

		public static d_mrb_any_to_s mrb_any_to_s { get; } = FuncLoader.LoadFunction< d_mrb_any_to_s > ( MRubyLibrary, "mrb_any_to_s" );


		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_float_value ( IntPtr mrb_state, mrb_float f );

		public static d_mrb_float_value mrb_float_value { get; } = FuncLoader.LoadFunction< d_mrb_float_value > ( MRubyLibrary, "mrb_float_value_ex" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_fixnum_value ( mrb_int i );

		public static d_mrb_fixnum_value mrb_fixnum_value { get; } = FuncLoader.LoadFunction< d_mrb_fixnum_value > ( MRubyLibrary, "mrb_fixnum_value_ex" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_bool_value ( mrb_bool b );

		public static d_mrb_bool_value mrb_bool_value { get; } = FuncLoader.LoadFunction< d_mrb_bool_value > ( MRubyLibrary, "mrb_bool_value_ex" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_nil_value ();

		public static d_mrb_nil_value mrb_nil_value { get; } = FuncLoader.LoadFunction< d_mrb_nil_value > ( MRubyLibrary, "mrb_nil_value_ex" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_undef_value ();

		public static d_mrb_undef_value mrb_undef_value { get; } = FuncLoader.LoadFunction< d_mrb_undef_value > ( MRubyLibrary, "mrb_undef_value_ex" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_false_value ();

		public static d_mrb_false_value mrb_false_value { get; } = FuncLoader.LoadFunction< d_mrb_false_value > ( MRubyLibrary, "mrb_false_value_ex" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_true_value ();

		public static d_mrb_true_value mrb_true_value { get; } = FuncLoader.LoadFunction< d_mrb_true_value > ( MRubyLibrary, "mrb_true_value_ex" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_cptr_value ( IntPtr mrb_state, IntPtr p );

		public static d_mrb_cptr_value mrb_cptr_value { get; } = FuncLoader.LoadFunction< d_mrb_cptr_value > ( MRubyLibrary, "mrb_cptr_value_ex" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_obj_value ( IntPtr p );

		public static d_mrb_obj_value mrb_obj_value { get; } = FuncLoader.LoadFunction< d_mrb_obj_value > ( MRubyLibrary, "mrb_obj_value_ex" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_ptr ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_cptr ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_float d_mrb_float ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_int d_mrb_fixnum ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_sym d_mrb_symbol ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_vtype d_mrb_type ( mrb_value o );
		public static d_mrb_ptr mrb_ptr { get; } = FuncLoader.LoadFunction< d_mrb_ptr > ( MRubyLibrary, "mrb_ptr_ex" );
		public static d_mrb_cptr mrb_cptr { get; } = FuncLoader.LoadFunction< d_mrb_cptr > ( MRubyLibrary, "mrb_cptr_ex" );
		public static d_mrb_float mrb_float { get; } = FuncLoader.LoadFunction< d_mrb_float > ( MRubyLibrary, "mrb_float_ex" );
		public static d_mrb_fixnum mrb_fixnum { get; } = FuncLoader.LoadFunction< d_mrb_fixnum > ( MRubyLibrary, "mrb_fixnum_ex" );
		public static d_mrb_symbol mrb_symbol { get; } = FuncLoader.LoadFunction< d_mrb_symbol > ( MRubyLibrary, "mrb_symbol_ex" );
		public static d_mrb_type mrb_type { get; } = FuncLoader.LoadFunction< d_mrb_type > ( MRubyLibrary, "mrb_type_ex" );
		
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_immediate_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_fixnum_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_symbol_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_undef_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_nil_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_false_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_true_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_float_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_array_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_string_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_hash_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_cptr_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_exception_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_free_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_object_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_class_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_module_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_iclass_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_sclass_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_proc_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_range_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_file_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_env_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_data_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_fiber_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_istruct_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_break_p ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_bool ( mrb_value o );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_test ( mrb_value o );
		
		public static d_mrb_immediate_p mrb_immediate_p { get; } = FuncLoader.LoadFunction< d_mrb_immediate_p > ( MRubyLibrary, "mrb_immediate_p_ex" );
		public static d_mrb_fixnum_p mrb_fixnum_p { get; } = FuncLoader.LoadFunction< d_mrb_fixnum_p > ( MRubyLibrary, "mrb_fixnum_p_ex" );
		public static d_mrb_symbol_p mrb_symbol_p { get; } = FuncLoader.LoadFunction< d_mrb_symbol_p > ( MRubyLibrary, "mrb_symbol_p_ex" );
		public static d_mrb_undef_p mrb_undef_p { get; } = FuncLoader.LoadFunction< d_mrb_undef_p > ( MRubyLibrary, "mrb_undef_p_ex" );
		public static d_mrb_nil_p mrb_nil_p { get; } = FuncLoader.LoadFunction< d_mrb_nil_p > ( MRubyLibrary, "mrb_nil_p_ex" );
		public static d_mrb_false_p mrb_false_p { get; } = FuncLoader.LoadFunction< d_mrb_false_p > ( MRubyLibrary, "mrb_false_p_ex" );
		public static d_mrb_true_p mrb_true_p { get; } = FuncLoader.LoadFunction< d_mrb_true_p > ( MRubyLibrary, "mrb_true_p_ex" );
		public static d_mrb_float_p mrb_float_p { get; } = FuncLoader.LoadFunction< d_mrb_float_p > ( MRubyLibrary, "mrb_float_p_ex" );
		public static d_mrb_array_p mrb_array_p { get; } = FuncLoader.LoadFunction< d_mrb_array_p > ( MRubyLibrary, "mrb_array_p_ex" );
		public static d_mrb_string_p mrb_string_p { get; } = FuncLoader.LoadFunction< d_mrb_string_p > ( MRubyLibrary, "mrb_string_p_ex" );
		public static d_mrb_hash_p mrb_hash_p { get; } = FuncLoader.LoadFunction< d_mrb_hash_p > ( MRubyLibrary, "mrb_hash_p_ex" );
		public static d_mrb_cptr_p mrb_cptr_p { get; } = FuncLoader.LoadFunction< d_mrb_cptr_p > ( MRubyLibrary, "mrb_cptr_p_ex" );
		public static d_mrb_exception_p mrb_exception_p { get; } = FuncLoader.LoadFunction< d_mrb_exception_p > ( MRubyLibrary, "mrb_exception_p_ex" );
		public static d_mrb_free_p mrb_free_p { get; } = FuncLoader.LoadFunction< d_mrb_free_p > ( MRubyLibrary, "mrb_free_p_ex" );
		public static d_mrb_object_p mrb_object_p { get; } = FuncLoader.LoadFunction< d_mrb_object_p > ( MRubyLibrary, "mrb_object_p_ex" );
		public static d_mrb_class_p mrb_class_p { get; } = FuncLoader.LoadFunction< d_mrb_class_p > ( MRubyLibrary, "mrb_class_p_ex" );
		public static d_mrb_module_p mrb_module_p { get; } = FuncLoader.LoadFunction< d_mrb_module_p > ( MRubyLibrary, "mrb_module_p_ex" );
		public static d_mrb_iclass_p mrb_iclass_p { get; } = FuncLoader.LoadFunction< d_mrb_iclass_p > ( MRubyLibrary, "mrb_iclass_p_ex" );
		public static d_mrb_sclass_p mrb_sclass_p { get; } = FuncLoader.LoadFunction< d_mrb_sclass_p > ( MRubyLibrary, "mrb_sclass_p_ex" );
		public static d_mrb_proc_p mrb_proc_p { get; } = FuncLoader.LoadFunction< d_mrb_proc_p > ( MRubyLibrary, "mrb_proc_p_ex" );
		public static d_mrb_range_p mrb_range_p { get; } = FuncLoader.LoadFunction< d_mrb_range_p > ( MRubyLibrary, "mrb_range_p_ex" );
		public static d_mrb_file_p mrb_file_p { get; } = FuncLoader.LoadFunction< d_mrb_file_p > ( MRubyLibrary, "mrb_file_p_ex" );
		public static d_mrb_env_p mrb_env_p { get; } = FuncLoader.LoadFunction< d_mrb_env_p > ( MRubyLibrary, "mrb_env_p_ex" );
		public static d_mrb_data_p mrb_data_p { get; } = FuncLoader.LoadFunction< d_mrb_data_p > ( MRubyLibrary, "mrb_data_p_ex" );
		public static d_mrb_fiber_p mrb_fiber_p { get; } = FuncLoader.LoadFunction< d_mrb_fiber_p > ( MRubyLibrary, "mrb_fiber_p_ex" );
		public static d_mrb_istruct_p mrb_istruct_p { get; } = FuncLoader.LoadFunction< d_mrb_istruct_p > ( MRubyLibrary, "mrb_istruct_p_ex" );
		public static d_mrb_break_p mrb_break_p { get; } = FuncLoader.LoadFunction< d_mrb_break_p > ( MRubyLibrary, "mrb_break_p_ex" );
		public static d_mrb_bool mrb_bool { get; } = FuncLoader.LoadFunction< d_mrb_bool > ( MRubyLibrary, "mrb_bool_ex" );
		public static d_mrb_test mrb_test { get; } = FuncLoader.LoadFunction< d_mrb_test > ( MRubyLibrary, "mrb_test_ex" );
		
		
		
		//
		// 文字列
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_str_new ( byte[] ptr, long len );

		public static d_mrb_str_new mrb_str_new { get; } = FuncLoader.LoadFunction< d_mrb_str_new > ( MRubyLibrary, "mrb_str_new" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_str_new_cstr ( IntPtr mrb_state, byte[] ptr );

		public static d_mrb_str_new_cstr mrb_str_new_cstr { get; } = FuncLoader.LoadFunction< d_mrb_str_new_cstr > ( MRubyLibrary, "mrb_str_new_cstr" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_str_new_static ( IntPtr mrb_state, byte[] ptr, long len );

		public static d_mrb_str_new_static mrb_str_new_static { get; } = FuncLoader.LoadFunction< d_mrb_str_new_static > ( MRubyLibrary, "mrb_str_new_static" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_obj_as_string ( IntPtr mrb_state, mrb_value obj );

		public static d_mrb_obj_as_string mrb_obj_as_string { get; } = FuncLoader.LoadFunction< d_mrb_obj_as_string > ( MRubyLibrary, "mrb_obj_as_string" );

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

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_string_value_cstr ( IntPtr mrb_state, ref mrb_value v_ptr );

		public static d_mrb_string_value_cstr mrb_string_value_cstr { get; } = FuncLoader.LoadFunction< d_mrb_string_value_cstr > ( MRubyLibrary, "mrb_string_value_cstr" );

		public static string StringValuePtr ( IntPtr mrb_state, mrb_value v ) {
			int    length = 0;
			IntPtr ptr    = mrb_string_value_cstr ( mrb_state, ref v );
			unsafe {
				byte * p = ( byte * )ptr;
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
		private delegate mrb_sym d_mrb_intern ( IntPtr mrb_state, byte[] name, UInt64 s );

		private static d_mrb_intern mrb_intern { get; } = FuncLoader.LoadFunction< d_mrb_intern > ( MRubyLibrary, "mrb_intern" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		private delegate mrb_sym d_mrb_intern_cstr ( IntPtr mrb_state, byte[] name );

		private static d_mrb_intern_cstr mrb_intern_cstr { get; } = FuncLoader.LoadFunction< d_mrb_intern_cstr > ( MRubyLibrary, "mrb_intern_cstr" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		private delegate mrb_sym d_mrb_intern_static ( IntPtr mrb_state, byte[] name, UInt64 s );

		private static d_mrb_intern_static mrb_intern_static { get; } = FuncLoader.LoadFunction< d_mrb_intern_static > ( MRubyLibrary, "mrb_intern_static" );



		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		private delegate Int64 d_mrb_obj_id ( mrb_value obj );

		private static d_mrb_obj_id mrb_obj_id { get; } = FuncLoader.LoadFunction< d_mrb_obj_id > ( MRubyLibrary, "mrb_obj_id" );


		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		private delegate IntPtr d_mrb_obj_class ( IntPtr mrb_state, mrb_value obj );

		private static d_mrb_obj_class mrb_obj_class { get; } = FuncLoader.LoadFunction< d_mrb_obj_class > ( MRubyLibrary, "mrb_obj_class" );


		//
		// 定数
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_define_const ( IntPtr mrb_state, IntPtr klass, string name, mrb_value val );

		public static d_mrb_define_const mrb_define_const { get; } = FuncLoader.LoadFunction< d_mrb_define_const > ( MRubyLibrary, "mrb_define_const" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_define_global_const ( IntPtr mrb_state, string name, mrb_value val );

		public static d_mrb_define_global_const mrb_define_global_const { get; } = FuncLoader.LoadFunction< d_mrb_define_global_const > ( MRubyLibrary, "mrb_define_global_const" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_const_get ( IntPtr mrb_state, mrb_value obj, mrb_sym sym );

		public static d_mrb_const_get mrb_const_get { get; } = FuncLoader.LoadFunction< d_mrb_const_get > ( MRubyLibrary, "mrb_const_get" );


		//
		// module, class
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_define_class ( IntPtr mrb_state, string name, IntPtr super );

		public static d_mrb_define_class mrb_define_class { get; } = FuncLoader.LoadFunction< d_mrb_define_class > ( MRubyLibrary, "mrb_define_class" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_define_class_under ( IntPtr mrb_state, IntPtr outer, string name, IntPtr super );

		public static d_mrb_define_class_under mrb_define_class_under { get; } = FuncLoader.LoadFunction< d_mrb_define_class_under > ( MRubyLibrary, "mrb_define_class_under" );


		[DllImport ( MRubyDll, EntryPoint = "mrb_set_instance_tt" )]
		public static extern void MRB_SET_INSTANCE_TT ( IntPtr c, mrb_vtype tt );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_class_get ( IntPtr mrb_state, string name );

		public static d_mrb_class_get mrb_class_get { get; } = FuncLoader.LoadFunction< d_mrb_class_get > ( MRubyLibrary, "mrb_class_get" );

		//[DllImport(MRubyDll)]
		//public static extern mrb_value mrb_class_new_instance(IntPtr mrb_state, mrb_int argc, mrb_value[] argv, IntPtr c);
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_obj_new ( IntPtr mrb_state, IntPtr c, mrb_int argc, mrb_value[] argv );

		public static d_mrb_obj_new mrb_obj_new { get; } = FuncLoader.LoadFunction< d_mrb_obj_new > ( MRubyLibrary, "mrb_obj_new" );


		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_define_module ( IntPtr mrb_state, string name );

		public static d_mrb_define_module mrb_define_module { get; } = FuncLoader.LoadFunction< d_mrb_define_module > ( MRubyLibrary, "mrb_define_module" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_define_module_under ( IntPtr mrb_state, IntPtr outer, string name, IntPtr super );

		public static d_mrb_define_module_under mrb_define_module_under { get; } = FuncLoader.LoadFunction< d_mrb_define_module_under > ( MRubyLibrary, "mrb_define_module_under" );

		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_module_get ( IntPtr mrb_state, string name );

		public static d_mrb_module_get mrb_module_get { get; } = FuncLoader.LoadFunction< d_mrb_module_get > ( MRubyLibrary, "mrb_module_get" );




		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_get_args ( IntPtr state, string format, out IntPtr argv, out int argc, out mrb_value block );

		public static d_mrb_get_args mrb_get_args { get; } = FuncLoader.LoadFunction< d_mrb_get_args > ( MRubyLibrary, "mrb_get_args" );



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
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_define_method ( IntPtr state, IntPtr klass, string name, MRubyCSFunction func, mrb_args aspec );

		public static d_mrb_define_method mrb_define_method { get; } = FuncLoader.LoadFunction< d_mrb_define_method > ( MRubyLibrary, "mrb_define_method" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_define_class_method ( IntPtr state, IntPtr klass, string name, MRubyCSFunction func, mrb_args aspec );

		public static d_mrb_define_class_method mrb_define_class_method { get; } = FuncLoader.LoadFunction< d_mrb_define_class_method > ( MRubyLibrary, "mrb_define_class_method" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_define_module_function ( IntPtr state, IntPtr klass, string name, MRubyCSFunction func, mrb_args aspec );

		public static d_mrb_define_module_function mrb_define_module_function { get; } = FuncLoader.LoadFunction< d_mrb_define_module_function > ( MRubyLibrary, "mrb_define_module_function" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_define_singleton_method ( IntPtr state, IntPtr klass, string name, MRubyCSFunction func, mrb_args aspec );

		public static d_mrb_define_singleton_method mrb_define_singleton_method { get; } = FuncLoader.LoadFunction< d_mrb_define_singleton_method > ( MRubyLibrary, "mrb_define_singleton_method" );


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
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_funcall ( IntPtr mrb_state, mrb_value obj, string funcName, int argc );
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate mrb_value d_mrb_funcall_1 ( IntPtr mrb_state, mrb_value obj, string funcName, int argc, mrb_value arg1 );

		public static d_mrb_funcall mrb_funcall { get; } = FuncLoader.LoadFunction< d_mrb_funcall > ( MRubyLibrary, "mrb_funcall" );

		public static d_mrb_funcall_1 mrb_funcall_1 { get; } = FuncLoader.LoadFunction< d_mrb_funcall_1 > ( MRubyLibrary, "mrb_funcall" );

		//[DllImport(MRubyDll)]
  //      public static extern mrb_value mrb_funcall(IntPtr mrb_state, mrb_value obj, string funcName, int argc, mrb_value arg1);
  //      [DllImport(MRubyDll)]
  //      public static extern mrb_value mrb_funcall(IntPtr mrb_state, mrb_value obj, string funcName, int argc, mrb_value arg1, mrb_value arg2);


		/// <summary>
		/// 从C#调用mruby方法时，使用这个方法获取参数
		/// </summary>
		/// <param name="state"></param>
		/// <param name="withBlock"></param>
		/// <returns></returns>
		public static mrb_value[] GetFunctionArgs ( IntPtr mrb_state, bool withBlock = false ) {
			mrb_value[] values;
			IntPtr      argvPointer;
			mrb_value   value = default ( mrb_value );
			int         i, argc, size;
			mrb_value   block;

			mRubyDLL.mrb_get_args ( mrb_state, "*&", out argvPointer, out argc, out block );

			int valueCount = argc;
			if ( withBlock ) {
			  valueCount++;
			}

			values = new mrb_value[valueCount]; // Include Block
			size   = Marshal.SizeOf ( typeof ( mrb_value ) );
			for ( i = 0; i < argc; i++ ) {
			  value       = ( mrb_value )Marshal.PtrToStructure ( argvPointer + ( i * size ), typeof ( mrb_value ) );
			  values[ i ] = value;
			}

			if ( withBlock ) {
			  values[ argc ] = value;
			}

			return values;
		}


	  
		// 変数
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_iv_get ( IntPtr mrb_state, mrb_value obj, mrb_sym sym );

		public static d_mrb_iv_get mrb_iv_get { get; } = FuncLoader.LoadFunction< d_mrb_iv_get > ( MRubyLibrary, "mrb_iv_get" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_iv_set ( IntPtr mrb_state, mrb_value obj, mrb_sym sym, mrb_value v );

		public static d_mrb_iv_set mrb_iv_set { get; } = FuncLoader.LoadFunction< d_mrb_iv_set > ( MRubyLibrary, "mrb_iv_set" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_obj_iv_get ( IntPtr mrb_state, IntPtr obj, mrb_sym sym );

		public static d_mrb_obj_iv_get mrb_obj_iv_get { get; } = FuncLoader.LoadFunction< d_mrb_obj_iv_get > ( MRubyLibrary, "mrb_obj_iv_get" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_obj_iv_set ( IntPtr mrb_state, IntPtr obj, mrb_sym sym, mrb_value v );

		public static d_mrb_obj_iv_set mrb_obj_iv_set { get; } = FuncLoader.LoadFunction< d_mrb_obj_iv_set > ( MRubyLibrary, "mrb_obj_iv_set" );


		//
		// 配列
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_ary_new ();
		public static d_mrb_ary_new mrb_ary_new { get; } = FuncLoader.LoadFunction< d_mrb_ary_new > ( MRubyLibrary, "mrb_ary_new" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_ary_push ( IntPtr mrb_state, mrb_value array, mrb_value value );
		public static d_mrb_ary_push mrb_ary_push { get; } = FuncLoader.LoadFunction< d_mrb_ary_push > ( MRubyLibrary, "mrb_ary_push" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_ary_ref ( IntPtr mrb_state, mrb_value ary, mrb_int n );
		public static d_mrb_ary_ref mrb_ary_ref { get; } = FuncLoader.LoadFunction< d_mrb_ary_ref > ( MRubyLibrary, "mrb_ary_ref" );
		

		//
		// Hash
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_hash_new ();

		public static d_mrb_hash_new mrb_hash_new { get; } = FuncLoader.LoadFunction< d_mrb_hash_new > ( MRubyLibrary, "mrb_hash_new" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_hash_set ( IntPtr mrb_state, mrb_value hash, mrb_value key, mrb_value val );

		public static d_mrb_hash_set mrb_hash_set { get; } = FuncLoader.LoadFunction< d_mrb_hash_set > ( MRubyLibrary, "mrb_hash_set" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_hash_get ( IntPtr mrb_state, mrb_value hash, mrb_value key );

		public static d_mrb_hash_get mrb_hash_get { get; } = FuncLoader.LoadFunction< d_mrb_hash_get > ( MRubyLibrary, "mrb_hash_get" );


		//
		// Context
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrbc_context_new ( IntPtr mrb_state );
		public static d_mrbc_context_new mrbc_context_new { get; } = FuncLoader.LoadFunction< d_mrbc_context_new > ( MRubyLibrary, "mrbc_context_new" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrbc_context_free ( IntPtr mrb_state, IntPtr mrbc_context );
		public static d_mrbc_context_free mrbc_context_free { get; } = FuncLoader.LoadFunction< d_mrbc_context_free > ( MRubyLibrary, "mrbc_context_free" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrbc_filename ( IntPtr mrb_state, IntPtr mrbc_context, string name );
		public static d_mrbc_filename mrbc_filename { get; } = FuncLoader.LoadFunction< d_mrbc_filename > ( MRubyLibrary, "mrbc_filename" );

		
		//
		// Data
		// 
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_data_object_alloc ( IntPtr mrb_state, IntPtr klass, IntPtr datap, IntPtr type );

		public static d_mrb_data_object_alloc mrb_data_object_alloc { get; } = FuncLoader.LoadFunction< d_mrb_data_object_alloc > ( MRubyLibrary, "mrb_data_object_alloc" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_data_init ( mrb_value v, IntPtr ptr, IntPtr data_type );

		public static d_mrb_data_init mrb_data_init { get; } = FuncLoader.LoadFunction< d_mrb_data_init > ( MRubyLibrary, "mrb_data_init_ex" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_data_get_ptr ( IntPtr mrb_state, mrb_value obj, IntPtr type );

		public static d_mrb_data_get_ptr mrb_data_get_ptr { get; } = FuncLoader.LoadFunction< d_mrb_data_get_ptr > ( MRubyLibrary, "mrb_data_get_ptr" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_set_instance_tt ( IntPtr klass, mrb_vtype tt );
		public static d_mrb_set_instance_tt mrb_set_instance_tt { get; } = FuncLoader.LoadFunction<d_mrb_set_instance_tt> ( MRubyLibrary, "mrb_set_instance_tt" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate IntPtr d_mrb_data_wrap_struct ( IntPtr mrb_state, IntPtr klass, IntPtr data_type, IntPtr ptr );

		public static d_mrb_data_wrap_struct mrb_data_wrap_struct { get; } = FuncLoader.LoadFunction< d_mrb_data_wrap_struct > ( MRubyLibrary, "mrb_data_wrap_struct" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_data_wrap_struct_obj ( IntPtr mrb_state, IntPtr klass, IntPtr data_type, IntPtr ptr );

		public static d_mrb_data_wrap_struct_obj mrb_data_wrap_struct_obj { get; } = FuncLoader.LoadFunction< d_mrb_data_wrap_struct_obj > ( MRubyLibrary, "mrb_data_wrap_struct_obj" );

		public static IntPtr ObjectToInPtr ( object obj ) {
			return GCHandle.ToIntPtr ( GCHandle.Alloc ( obj ) );
		}

		public static object IntPtrToObject ( IntPtr ptr ) {
			
			if ( ptr == IntPtr.Zero ) {
				return null;
			}

			return GCHandle.FromIntPtr ( ptr ).Target;
		}
		
		public static T ValueToDataObject<T> ( IntPtr mrb_state, mrb_value value, IntPtr data_type ) {
			if ( mrb_value.IsNil ( value ) ) {
				return default ( T );
			}
			return ( T )IntPtrToObject ( mrb_data_get_ptr ( mrb_state, value, data_type ) );
		}
		
		public static mrb_value DataObjectToValue ( IntPtr mrb_state, IntPtr klass, IntPtr data_type, object obj ) {
			return mrb_data_wrap_struct_obj ( mrb_state, klass, data_type, ObjectToInPtr ( obj ) );
		}

		
		//
		// 例外
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		private delegate void d_mrb_malloc ( IntPtr mrb_state, long len );
		private static d_mrb_malloc mrb_malloc { get; } = FuncLoader.LoadFunction < d_mrb_malloc >( MRubyLibrary, "mrb_malloc" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		private delegate void d_mrb_raise ( IntPtr mrb_state, IntPtr obj, byte[] msg );

		private static d_mrb_raise mrb_raise { get; } = FuncLoader.LoadFunction< d_mrb_raise > ( MRubyLibrary, "mrb_raise" );


		//
		// そのほか
		//
		//private const int FL_FREEZE = (1 << 10);
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_inspect ( IntPtr mrb_state, mrb_value obj );

		public static d_mrb_inspect mrb_inspect { get; } = FuncLoader.LoadFunction< d_mrb_inspect > ( MRubyLibrary, "mrb_inspect" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_top_self ( IntPtr mrb_state );
		public static d_mrb_top_self mrb_top_self { get; } = FuncLoader.LoadFunction< d_mrb_top_self > ( MRubyLibrary, "mrb_top_self" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_bool d_mrb_has_exc ( IntPtr mrb_state );
		public static d_mrb_has_exc mrb_has_exc { get; } = FuncLoader.LoadFunction< d_mrb_has_exc > ( MRubyLibrary, "mrb_has_exc" );
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_get_exc_value ( IntPtr mrb_state );
		public static d_mrb_get_exc_value mrb_get_exc_value { get; } = FuncLoader.LoadFunction< d_mrb_get_exc_value > ( MRubyLibrary, "mrb_get_exc_value" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_exc_detail ( IntPtr mrb_state );

		public static d_mrb_exc_detail mrb_exc_detail { get; } = FuncLoader.LoadFunction< d_mrb_exc_detail > ( MRubyLibrary, "mrb_exc_detail" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_get_backtrace ( IntPtr mrb_state );

		public static d_mrb_get_backtrace mrb_get_backtrace { get; } = FuncLoader.LoadFunction< d_mrb_get_backtrace > ( MRubyLibrary, "mrb_get_backtrace" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate mrb_value d_mrb_exc_backtrace ( IntPtr mrb_state, mrb_value exc );

		public static d_mrb_exc_backtrace mrb_exc_backtrace { get; } = FuncLoader.LoadFunction< d_mrb_exc_backtrace > ( MRubyLibrary, "mrb_exc_backtrace" );

		
		//
		// GC
		//
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate int d_mrb_gc_arena_save ( IntPtr mrb_state );
		public static d_mrb_gc_arena_save mrb_gc_arena_save { get; } = FuncLoader.LoadFunction< d_mrb_gc_arena_save > ( MRubyLibrary, "mrb_gc_arena_save_ex" );
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_gc_arena_restore ( IntPtr mrb_state, int idx );
		public static d_mrb_gc_arena_restore mrb_gc_arena_restore { get; } = FuncLoader.LoadFunction< d_mrb_gc_arena_restore > ( MRubyLibrary, "mrb_gc_arena_restore_ex" );

		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_garbage_collect ( IntPtr mrb_state );
		public static d_mrb_garbage_collect mrb_garbage_collect { get; } = FuncLoader.LoadFunction< d_mrb_garbage_collect > ( MRubyLibrary, "mrb_garbage_collect" );
		
		[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
		public delegate void d_mrb_full_gc ( IntPtr mrb_state );
		public static d_mrb_full_gc mrb_full_gc { get; } = FuncLoader.LoadFunction< d_mrb_full_gc > ( MRubyLibrary, "mrb_full_gc" );

		
		
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


    internal class FuncLoader {
	    private class Windows {
		    [DllImport ( "kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true )]
		    public static extern IntPtr GetProcAddress ( IntPtr hModule, string procName );

		    [DllImport ( "kernel32", SetLastError = true, CharSet = CharSet.Unicode )]
		    public static extern IntPtr LoadLibraryW ( string lpszLib );
	    }

	    private class Linux {
		    [DllImport ( "libdl.so.2" )]
		    public static extern IntPtr dlopen ( string path, int flags );

		    [DllImport ( "libdl.so.2" )]
		    public static extern IntPtr dlsym ( IntPtr handle, string symbol );
	    }

	    private class OSX {
		    [DllImport ( "/usr/lib/libSystem.dylib" )]
		    public static extern IntPtr dlopen ( string path, int flags );

		    [DllImport ( "/usr/lib/libSystem.dylib" )]
		    public static extern IntPtr dlsym ( IntPtr handle, string symbol );
	    }

	    private const int RTLD_LAZY = 0x0001;

	    public static IntPtr LoadLibrary ( string libname ) {
		    if ( CurrentPlatform.OS == OS.Windows )
			    return Windows.LoadLibraryW ( libname );

		    if ( CurrentPlatform.OS == OS.MacOSX )
			    return OSX.dlopen ( libname, RTLD_LAZY );

		    return Linux.dlopen ( libname, RTLD_LAZY );
	    }

	    public static T LoadFunction< T > ( IntPtr library, string function, bool throwIfNotFound = false ) {
		    var ret = IntPtr.Zero;

		    if ( CurrentPlatform.OS == OS.Windows )
			    ret = Windows.GetProcAddress ( library, function );
		    else if ( CurrentPlatform.OS == OS.MacOSX )
			    ret = OSX.dlsym ( library, function );
		    else
			    ret = Linux.dlsym ( library, function );

		    if ( ret == IntPtr.Zero ) {
			    if ( throwIfNotFound )
				    throw new EntryPointNotFoundException ( function );

			    return default ( T );
		    }

#if NETSTANDARD
			return Marshal.GetDelegateForFunctionPointer< T > ( ret );
#else
		    return ( T )( object )Marshal.GetDelegateForFunctionPointer ( ret, typeof ( T ) );
#endif
	    }
    }


    internal enum OS {
		Windows,
		Linux,
		MacOSX,
		Unknown
	}

	internal static class CurrentPlatform {
		private static bool init = false;
		private static OS   os;

		[DllImport ( "libc" )]
		static extern int uname ( IntPtr buf );

		private static void Init () {
			if ( !init ) {
				PlatformID pid = Environment.OSVersion.Platform;

				switch ( pid ) {
					case PlatformID.Win32NT:
					case PlatformID.Win32S:
					case PlatformID.Win32Windows:
					case PlatformID.WinCE:
						os = OS.Windows;
						break;
					case PlatformID.MacOSX:
						os = OS.MacOSX;
						break;
					case PlatformID.Unix:

						// Mac can return a value of Unix sometimes, We need to double check it.
						IntPtr buf = IntPtr.Zero;
						try {
							buf = Marshal.AllocHGlobal ( 8192 );

							if ( uname ( buf ) == 0 ) {
								string sos = Marshal.PtrToStringAnsi ( buf );
								if ( sos == "Darwin" ) {
									os = OS.MacOSX;
									return;
								}
							}
						}
						catch {}
						finally {
							if ( buf != IntPtr.Zero )
								Marshal.FreeHGlobal ( buf );
						}

						os = OS.Linux;
						break;
					default:
						os = OS.Unknown;
						break;
				}

				init = true;
			}
		}

		public static OS OS {
			get {
				Init ();
				return os;
			}
		}
	}
}
