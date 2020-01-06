using System;
using System.Runtime.InteropServices;


namespace CandyFramework.mRuby {
	
	/// <summary>
	/// data type release function pointer
	/// </summary>
	/// <param name="state">mrb_state *mrb</param>
	/// <param name="data">void*</param>
	/// <returns></returns>
	[UnmanagedFunctionPointer ( CallingConvention.Cdecl )]
	public delegate void MRubyDFreeFunction ( IntPtr state, IntPtr data );

	/**
	 *  mrb_data_type Wrapper
	 * 
	 *  Create Helper to convert MrbValue to C# value types
	 */
	[StructLayout ( LayoutKind.Sequential )]
	public struct mrb_data_type {
		
		[MarshalAs ( UnmanagedType.BStr )] 
		public string struct_name;

		[MarshalAs ( UnmanagedType.FunctionPtr )]
		public MRubyDFreeFunction dfree;
	}
}
