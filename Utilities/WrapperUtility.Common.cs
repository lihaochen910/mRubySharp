namespace RubySharp {
	
	using System;
	using System.Collections.Generic;
	using System.Reflection;


	public partial class WrapperUtility {
		
		private const string RUBYSHARP_NAMESPACE = "RubySharp";
		private const string RUBYSHARP_CLASS_DLL = "RubyDLL";
		private const string RUBYSHARP_WRAPPERCLASS_POSTFIX = "_Wrapper";
		private const string RUBYSHARP_STATIC_FUNCTION_PREFIX = "STATIC_";
		
		
		private static Dictionary<string, string> operator_methods = new Dictionary<string, string> {
			// { "op_LogicalNot", "!" },
			{ "op_Addition", "+" }, { "op_Subtraction", "-" }, { "op_Multiply", "*" },
			{ "op_Division", "/" }, { "op_BitwiseAnd", "&" }, { "op_BitwiseOr", "|" },
			{ "op_ExclusiveOr", "^" }, { "op_OnesComplement", "~" }, { "op_Equality", "==" },
			{ "op_Inequality", "!=" }, { "op_LessThan", "<" }, { "op_GreaterThan", ">" },
			{ "op_LessThanOrEqual", "<=" }, { "op_GreaterThanOrEqual", ">=" }, { "op_LeftShift", "<<" },
			{ "op_RightShift", ">>" }, { "op_Modulus", "%" }
		};
		
		private static List<string> ignored_classes = new List< string >() {
			"<PrivateImplementationDetails>"
		};
		
		private static List<string> ignored_methods = new List< string >() {
			"Equals", "GetHashCode", "GetType", "ToString", "GetEnumerator"
		};
		
		private static List<string> ignored_fields = new List< string >() {
			"Item"
		};
		
		private static List< Type > base_value_types = new List< Type > {
			typeof ( int ), typeof ( short ), typeof ( long ),
			typeof ( uint ), typeof ( ushort ), typeof ( ulong ),
			typeof ( int? ), typeof ( short? ), typeof ( long? ),
			typeof ( uint? ), typeof ( ushort? ), typeof ( ulong? ),
			typeof ( byte ), typeof ( sbyte ),
			typeof ( byte? ), typeof ( sbyte? ),
			typeof ( float ), typeof ( double ),
			typeof ( float? ), typeof ( double? ),
			typeof ( bool ), typeof ( bool? ),
			typeof ( string ), typeof ( object )
		};

		private static IList< Type > exportingTypeList;
		
		
		private static bool TestTypeSupport ( Type type ) {
			
			// ignore unexported type
			if ( exportingTypeList != null && !exportingTypeList.Contains ( type ) && !type.IsEnum ) {
				// base type
				if ( !base_value_types.Contains ( type ) ) {
					return false;
				}
			}
			
			// pass by ref unsupport
			if ( type.IsValueType && type.IsByRef ) {
				return false;
			}
			
			if ( type.IsNested ) {
				return false;
			}
			
			if ( type.IsInterface ) {
				return false;
			}
			
			return true;
		}

		
		private static bool TestFunctionSupport ( MethodInfo methodInfo ) {
			
			if ( methodInfo.IsGenericMethod ) {
				return false;
			}
				
			// skip Obsolete
			if ( methodInfo.IsDefined ( typeof ( System.ObsoleteAttribute ), false ) ) {
				return false;
			}
			
			// check param
			var parameters = methodInfo.GetParameters ();
			foreach ( var p in parameters ) {
				
				// out
				if ( p.IsOut ) {
					return false;
				}
				
				// ref
				if ( p.ParameterType.IsValueType && p.ParameterType.IsByRef ) {
					return false;
				}
				
				if ( !TestTypeSupport ( p.ParameterType ) ) {
					return false;
				}
				
			}
			
			if ( methodInfo.ReturnType != typeof ( void ) && !TestTypeSupport ( methodInfo.ReturnType ) ) {
				return false;
			}
			
			return true;
		}

		
		private static bool TestTypeIsPublic ( Type t ) {
			return
				t.IsVisible
				&& t.IsPublic
				&& !t.IsNotPublic
				&& !t.IsNested
				&& !t.IsNestedPublic
				&& !t.IsNestedFamily
				&& !t.IsNestedPrivate
				&& !t.IsNestedAssembly
				&& !t.IsNestedFamORAssem
				&& !t.IsNestedFamANDAssem;
		}
	}

}