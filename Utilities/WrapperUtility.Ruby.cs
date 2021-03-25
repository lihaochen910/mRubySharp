#if !MRUBY
namespace RubySharp {

	public partial class WrapperUtility {
		
		private const string RUBYSHARP_FIELD_SET_FUNCTION_POSTFIX = "_eql";
		private const string RUBYSHARP_COMMON_DATA_TYPE_PTR = "RubySharp.mRubyState.DATA_TYPE_PTR";
		private const string RUBYSHARP_FIELD_RClassVarName = "@class";
		private const string RUBYSHARP_FIELD_RModuleVarName = "@module";
		private const string RUBYSHARP_FIELD_DataTypePtrName = "data_type";
		private const string RUBYSHARP_FIELD_DataTypePtrVarName = "data_type_ptr";
		private const string RUBYSHARP_FIELD_MRubyStateVarName = "state";
		private const string RUBYSHARP_WrapFunction = "public static mrb_value {0} ( IntPtr mrb, mrb_value self )";
		private const string RUBYSHARP_GetFunctionArgs = "mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );";
		// private const string RUBYSHARP_ValueToCSInstance = "{0} instance = mRubyDLL.ValueToDataObject< {0} > ( mrb, self, data_type_ptr );";
		private const string RUBYSHARP_ValueToCSInstance = "{0} instance = mRubyDLL.ValueToDataObject< {0} > ( mrb, self, RubySharp.mRubyState.DATA_TYPE_PTR );";
		private const string RUBYSHARP_StaticRegisterFunctionName = "__Register__";
	}

}
#endif