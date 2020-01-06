namespace CandyFramework.mRuby {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.InteropServices;
	
	/// <summary>
	/// C#类自动生成包装类到mruby
	/// </summary>
	public class UserDataUtility {

		private const string RUBYSHARP_NAMESPACE = "CandyFramework.mRuby";
		private const string RUBYSHARP_CLASS_DLL = "mRubyDLL";
		private const string RUBYSHARP_WRAPPERCLASS_POSTFIX = "_Wrapper";
		private const string RUBYSHARP_FIELD_SET_FUNCTION_POSTFIX = "_eql";
		private const string RUBYSHARP_WrapFunction = "public static mrb_value {0} ( IntPtr mrb, mrb_value self )";
		private const string RUBYSHARP_GetFunctionArgs = "mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );";
		private const string RUBYSHARP_ValueToCSInstance = "{0} instance = mRubyDLL.ValueToDataObject< {0} > ( mrb, self, data_type_ptr );";
		private const string RUBYSHARP_StaticRegisterFunctionName = "__Register__";

		
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
			"Equals", "GetHashCode", "GetType"
		};
		
		private static List<string> ignored_fields = new List< string >() {
			"Item"
		};

		public static void GenByAssembly ( mRubyState state, Assembly assembly ) {
			
			List<Type> generatedType = new List< Type >();
			var types = assembly.GetTypes ();
			foreach ( var type in types ) {
				bool result = GenCSharpClass ( state, type );
				if ( result ) {
					generatedType.Add ( type );
				}
			}
			
			StringBuilder manifest = new StringBuilder ();
			foreach ( var type in generatedType ) {
				manifest.AppendLine (
					$"{type.FullName}{RUBYSHARP_WRAPPERCLASS_POSTFIX}.{RUBYSHARP_StaticRegisterFunctionName} ( state );" );
			}
			
			File.WriteAllText ( "_manifest.cs", manifest.ToString () );
		}

		public static void RegByNamespace ( mRubyState state, Assembly assembly, string @namespace ) {
			Type[] types = assembly.GetTypes ()
			                       .Where ( t => String.Equals ( t.Namespace, @namespace, StringComparison.Ordinal ) )
			                       .ToArray ();
			foreach ( var type in types ) {
				GenCSharpClass ( state, type );
			}
		}
		
		public static void GenCSharpClass<T> ( mRubyState state ) {
			Type type = typeof ( T );
			GenCSharpClass ( state, type );
		}

		public static IntPtr DefineCSharpClass<T> ( mRubyState state ) {
			Type type = typeof ( T );
			return DefineCSharpClass ( state, type );
		}

		public static IntPtr DefineCSharpClass ( mRubyState state, Type type ) {
			// 模块和类的开头必须是大写字母
			IntPtr   @class        = IntPtr.Zero;
			string[] namespacePath = type.FullName.Split ( '.' );

			if ( namespacePath.Length == 1 ) {
				@class = mRubyDLL.mrb_define_class ( state, type.Name, IntPtr.Zero );
			}
			else {
				foreach ( var name in namespacePath ) {

					string validName = name;
					
					// 检查命名开头字母大小写
					if ( !char.IsUpper ( name[ 0 ] ) ) {
						char head = char.ToUpper ( name[ 0 ] );
						string newName = name;
						newName = name.Remove ( 0, 1 );
						newName = newName.Insert ( 0, head.ToString () );
						
						Console.WriteLine ( $"{name} -> {newName}" );
						
						validName = newName;
					}
					
					if ( name.Equals ( namespacePath[ 0 ] ) ) {
						@class = mRubyDLL.mrb_define_module ( state, validName );
						mRubyDLL.mrb_set_instance_tt ( @class, mrb_vtype.MRB_TT_DATA );

					}
					else if ( name.Equals ( namespacePath[ namespacePath.Length - 1 ] ) ) {
						@class = mRubyDLL.mrb_define_class_under ( state, @class, validName, IntPtr.Zero );
						mRubyDLL.mrb_set_instance_tt ( @class, mrb_vtype.MRB_TT_DATA );
					}
					else {
						@class = mRubyDLL.mrb_define_module_under ( state, @class, validName, IntPtr.Zero );
						mRubyDLL.mrb_set_instance_tt ( @class, mrb_vtype.MRB_TT_DATA );
					}
				}
			}

			return @class;
		}

		public static bool GenCSharpClass ( mRubyState state, Type type ) {
			
			// Console.WriteLine ( $"Namespace: {type.Namespace}" );
			// Console.WriteLine ( $"FullName: {type.FullName}" );
			// Console.WriteLine ( $"Name: {type.Name}" );

			// TODO: 支持嵌套类
			if ( type.IsNested ) {
				return false;
			}
			
			// TODO: 支持Struct??
			// if ( !type.IsClass ) {
			// 	return IntPtr.Zero;
			// }
			
			// TODO: 支持泛型类??
			if ( type.IsGenericType ) {
				return false;
			}
			
			// TODO: 支持枚举类??
			if ( type.IsEnum ) {
				return false;
			}

			if ( ignored_classes.Contains ( type.FullName ) ) {
				Console.WriteLine ( $"skip: {type.FullName}" );
				return false;
			}
			
			GenCSharpClassSourceCode ( type, new StringBuilder () );
			
			return true;
		}

		public static string GetWrapperClassName ( Type type ) {
			return $"{type.FullName}{RUBYSHARP_WRAPPERCLASS_POSTFIX}";
		}
		
		public static void GenCSharpClassSourceCode ( Type type, StringBuilder builder ) {
			
			string wrapperClassName = $"{type.Name}{RUBYSHARP_WRAPPERCLASS_POSTFIX}";
			string rclassVarName = "@class";
			string dataTypeVarName = "data_type_ptr";
			string mRubyStateVarName = "state";
			string csInstanceToValue = "mRubyDLL.DataObjectToValue ( mrb, {0}, data_type_ptr, {1} );";
			
			// key: name in ruby | value: name in C#
			Dictionary<string, string> generatedMethods = new Dictionary<string, string>();
			Dictionary<string, string> generatedClassMethods = new Dictionary<string, string>();

			// Gen Using
			builder.AppendLine ( "using System;" );
			builder.AppendLine ( "using System.Runtime.InteropServices;" );
			builder.AppendLine ( "using CandyFramework.mRuby;" );
			builder.AppendLine ();
			
			// Gen Namespace and Class Sign
			builder.AppendLine ( "namespace " + type.Namespace + " {" );
			builder.AppendLine ();
			builder.AppendLine ( "\tpublic class " + wrapperClassName + " {" );
			builder.AppendLine ();
			
			// Gen Data Type Struct
			builder.AppendLine ( "\t\tpublic static readonly mrb_data_type data_type = new mrb_data_type () {" );
			builder.AppendLine ( $"\t\t\tstruct_name = \"{type.Name}\"," );
			builder.AppendLine ( "\t\t\tdfree = null" );
			builder.AppendLine ( "\t\t};" );
			builder.AppendLine ();
			
			// Gen Dep Static Field
			builder.AppendLine ( $"\t\tpublic static IntPtr {rclassVarName};" );
			builder.AppendLine ( $"\t\tpublic static IntPtr {dataTypeVarName};" );
			builder.AppendLine ( $"\t\tpublic static mRubyState {mRubyStateVarName};" );
			builder.AppendLine ();
			
			// Gen Ctor Function
			// TODO: 支持有参数构造方法
			ConstructorInfo publicCtor = type.GetConstructors ( BindingFlags.Public | BindingFlags.Instance ).Where ( c => c.GetParameters ().Length == 0 ).FirstOrDefault ();
			if ( publicCtor != null ) {
				builder.AppendFormat ( $"\t\t{RUBYSHARP_WrapFunction}" + " {{\n", "initialize" );
				builder.AppendFormat ( "\t\t\t{0} instance = new {0} ();\n", type.Name );
				builder.AppendLine ( $"\t\t\tmRubyDLL.mrb_data_init ( self, mRubyDLL.ObjectToInPtr ( instance ), data_type_ptr );" );
				builder.AppendLine ( $"\t\t\treturn self;" );
				builder.AppendLine ( "\t\t}\n" );
				generatedMethods.Add ( "initialize", "initialize" );
			}
			
			// Gen Public Field Get Set
			IList<FieldInfo> publicFields = type.GetFields ( BindingFlags.Public | BindingFlags.Instance );
			foreach ( var field in publicFields ) {
				if ( ignored_fields.Contains ( field.Name ) ) {
					continue;
				}
				
				GenGetSetInstanceFieldBody ( type.Name, field, builder );

				if ( !generatedMethods.ContainsKey ( field.Name ) ) {
					generatedMethods.Add ( field.Name, field.Name );
					generatedMethods.Add ( $"{field.Name}=", $"{field.Name}{RUBYSHARP_FIELD_SET_FUNCTION_POSTFIX}" );
				}
			}
			
			// Gen Public Field Get Set
			IList<PropertyInfo> publicPropertys = type.GetProperties ( BindingFlags.Public | BindingFlags.Instance );
			foreach ( var property in publicPropertys ) {
				if ( ignored_fields.Contains ( property.Name ) ) {
					continue;
				}
				
				GenGetSetInstancePropertyBody ( type.Name, property, builder );

				if ( !generatedMethods.ContainsKey ( property.Name ) ) {
					if ( property.CanRead ) {
						generatedMethods.Add ( property.Name, property.Name );
					}
					if ( property.CanWrite ) {
						generatedMethods.Add ( $"{property.Name}=", $"{property.Name}{RUBYSHARP_FIELD_SET_FUNCTION_POSTFIX}" );
					}
				}
			}

			// Gen Wrap Function
			IList< MethodInfo > publicMethods = type.GetMethods ( BindingFlags.Public | BindingFlags.Instance )
			                                        .Where ( m => !m.IsSpecialName ).ToArray ();
			foreach ( var method in publicMethods ) {
				if ( ignored_methods.Contains ( method.Name ) ) {
					continue;
				}
				
				// TODO: 支持方法重载??
				if ( generatedMethods.ContainsKey ( method.Name ) ) {
					continue;
				}
				
				GenInstanceBindingFunctionBody ( type.Name, string.Empty, method, builder );

				generatedMethods.Add ( method.Name, method.Name );
			}
			
			IList< MethodInfo > publicStaticMethods = type.GetMethods ( BindingFlags.Public | BindingFlags.Static )
			                                        .Where ( m => !m.IsSpecialName ).ToArray ();
			foreach ( var method in publicStaticMethods ) {
				if ( ignored_methods.Contains ( method.Name ) ) {
					continue;
				}
				
				// TODO: 支持方法重载??
				if ( generatedClassMethods.ContainsKey ( method.Name ) ) {
					continue;
				}
				
				GenStaticBindingFunctionBody ( type.Name, method.Name, method, builder );

				generatedClassMethods.Add ( method.Name, method.Name );
			}
			
			// Gen Operator Function
			foreach ( var kv in operator_methods ) {
				bool result = Gen_OpFunction_IfExist ( type, kv.Key, builder );
				if ( result ) {
					generatedMethods.Add ( kv.Value, $"_{kv.Key}" );
				}
			}
			
			// Gen Static Register function
			builder.AppendLine ( "\t\tpublic static void " + RUBYSHARP_StaticRegisterFunctionName + " ( mRubyState state ) {" );
			builder.AppendLine ( $"\t\t\t{wrapperClassName}.state = state;" );
			builder.AppendLine ( $"\t\t\t{wrapperClassName}.@class = UserDataUtility.DefineCSharpClass ( state, typeof ( {type.FullName} ) );" );
			builder.AppendLine ( $"\t\t\t{wrapperClassName}.data_type_ptr = mRubyDLL.ObjectToInPtr ( data_type );" );
			builder.AppendLine ();
			foreach ( var kv in generatedMethods ) {
				builder.AppendLine ( $"\t\t\tmRubyDLL.mrb_define_method ( state, @class, \"{kv.Key}\", {kv.Value}, mrb_args.ANY () );" );
			}
			builder.AppendLine ();
			foreach ( var kv in generatedClassMethods ) {
				builder.AppendLine ( $"\t\t\tmRubyDLL.mrb_define_class_method ( state, @class, \"{kv.Key}\", {kv.Value}, mrb_args.ANY () );" );
			}
			builder.AppendLine ( "\t\t}" );
			
			// Gen End
			builder.AppendLine ( "\t}" );
			builder.AppendLine ( "}" );
			
			File.WriteAllText ( $"{type.FullName.Replace ( ".", "_" )}{RUBYSHARP_WRAPPERCLASS_POSTFIX}.cs", builder.ToString () );
			
			// DEBUG
			// Console.WriteLine ( builder.ToString () );
		}

		private static void GenGetSetInstanceFieldBody ( string className, FieldInfo fieldInfo, StringBuilder builder ) {
			// Get
			builder.AppendFormat ( $"\t\t{RUBYSHARP_WrapFunction}" + " {{\n", fieldInfo.Name );
			builder.AppendFormat ( $"\t\t\t{RUBYSHARP_ValueToCSInstance}\n", className );
			builder.AppendLine ( $"\t\t\treturn {GenCSValueToValue ( fieldInfo.FieldType, $"instance.{fieldInfo.Name}" )};" );
			builder.AppendLine ( "\t\t}\n" );
			
			// Set
			builder.AppendFormat ( $"\t\t{RUBYSHARP_WrapFunction}" + " {{\n", $"{fieldInfo.Name}{RUBYSHARP_FIELD_SET_FUNCTION_POSTFIX}" );
			builder.AppendFormat ( $"\t\t\t{RUBYSHARP_ValueToCSInstance}\n", className );
			builder.AppendLine ( "\t\t\tmrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );" );
			builder.AppendLine ();
			GenCheckParamCount ( className, fieldInfo, builder );
			GenCheckParamsType ( className, fieldInfo, builder );
			builder.AppendLine ( $"\t\t\tinstance.{fieldInfo.Name} = {GenValueToCSValue ( fieldInfo.FieldType, "args[ 0 ]" )};" );
			builder.AppendLine ( $"\t\t\treturn self;" );
			builder.AppendLine ( "\t\t}\n" );
		}
		
		private static void GenGetSetInstancePropertyBody ( string className, PropertyInfo propertyInfo, StringBuilder builder ) {
			
			// Get
			if ( propertyInfo.CanRead ) {
				builder.AppendFormat ( $"\t\t{RUBYSHARP_WrapFunction}" + " {{\n", propertyInfo.Name );
				builder.AppendFormat ( $"\t\t\t{RUBYSHARP_ValueToCSInstance}\n", className );
				builder.AppendLine ( $"\t\t\treturn {GenCSValueToValue ( propertyInfo.PropertyType, $"instance.{propertyInfo.Name}" )};" );
				builder.AppendLine ( "\t\t}\n" );
			}
			
			// Set
			if ( propertyInfo.CanWrite ) {
				builder.AppendFormat ( $"\t\t{RUBYSHARP_WrapFunction}" + " {{\n", $"{propertyInfo.Name}{RUBYSHARP_FIELD_SET_FUNCTION_POSTFIX}" );
				builder.AppendFormat ( $"\t\t\t{RUBYSHARP_ValueToCSInstance}\n", className );
				builder.AppendLine ( "\t\t\tmrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );" );
				builder.AppendLine ();
				GenCheckParamCount ( className, propertyInfo, builder );
				GenCheckParamsType ( className, propertyInfo, builder );
				builder.AppendLine ( $"\t\t\tinstance.{propertyInfo.Name} = {GenValueToCSValue ( propertyInfo.PropertyType, "args[ 0 ]" )};" );
				builder.AppendLine ( $"\t\t\treturn self;" );
				builder.AppendLine ( "\t\t}\n" );
			}
		}

		private static void GenInstanceBindingFunctionBody ( string className, string functionName, MethodInfo methodInfo, StringBuilder builder ) {
			
			if ( string.IsNullOrEmpty ( functionName ) ) {
				functionName = methodInfo.Name;
			}
			
			builder.AppendFormat ( $"\t\t{RUBYSHARP_WrapFunction}" + " {{\n", functionName );
			builder.AppendFormat ( $"\t\t\t{RUBYSHARP_ValueToCSInstance}\n", className );
			builder.AppendLine ();
			
			if ( methodInfo.GetParameters ().Length != 0 ) {
				builder.AppendLine ( $"\t\t\t{RUBYSHARP_GetFunctionArgs}" );
				builder.AppendLine ();
			}

			GenCheckParamCount ( className, methodInfo, builder );
			GenCheckParamsType ( className, methodInfo, builder );

			if ( methodInfo.ReturnType == typeof ( void ) ) {
				builder.AppendLine ( $"\t\t\tinstance.{functionName} ({GenCallFunctionParams ( methodInfo )});" );
				builder.AppendLine ( "\t\t\treturn self;" );
			}
			else {
				builder.AppendLine ( $"\t\t\tvar ret = instance.{functionName} ({GenCallFunctionParams ( methodInfo )});" );
				builder.AppendLine ( $"\t\t\treturn {GenCSValueToValue ( methodInfo.ReturnType, "ret" )};" );
			}
			
			builder.AppendLine ( "\t\t}\n" );
		}
		
		private static void GenStaticBindingFunctionBody ( string className, string functionName, MethodInfo methodInfo, StringBuilder builder ) {
			
			builder.AppendFormat ( $"\t\t{RUBYSHARP_WrapFunction}" + " {{\n", functionName );
			builder.AppendLine ();
			
			if ( methodInfo.GetParameters ().Length != 0 ) {
				builder.AppendLine ( $"\t\t\t{RUBYSHARP_GetFunctionArgs}" );
				builder.AppendLine ();
			}

			GenCheckParamCount ( className, methodInfo, builder );
			GenCheckParamsType ( className, methodInfo, builder );

			if ( methodInfo.ReturnType == typeof ( void ) ) {
				builder.AppendLine ( $"\t\t\t{className}.{functionName} ({GenCallFunctionParams ( methodInfo )});" );
				builder.AppendLine ( "\t\t\treturn self;" );
			}
			else {
				builder.AppendLine ( $"\t\t\tvar ret = {className}.{functionName} ({GenCallFunctionParams ( methodInfo )});" );
				builder.AppendLine ( $"\t\t\treturn {GenCSValueToValue ( methodInfo.ReturnType, "ret" )};" );
			}
			
			builder.AppendLine ( "\t\t}\n" );
		}
		
		private static void GenCheckParamCount ( string className, FieldInfo fieldInfo, StringBuilder builder ) {
			string checkParamsCountFalse = "throw new ArgumentException ( $\"{0}.{1} parameter count mismatch: require {2} but got {{args.Length}}.\" );";
			
			builder.AppendLine ( "\t\t\tif ( args.Length != 1 ) {" );
			builder.AppendFormat ( $"\t\t\t\t{checkParamsCountFalse}\n", className, fieldInfo.Name, 1 );
			builder.AppendLine ( "\t\t\t}" );
			builder.AppendLine ();
		}
		
		private static void GenCheckParamCount ( string className, PropertyInfo propertyInfo, StringBuilder builder ) {
			string checkParamsCountFalse = "throw new ArgumentException ( $\"{0}.{1} parameter count mismatch: require {2} but got {{args.Length}}.\" );";
			
			builder.AppendLine ( "\t\t\tif ( args.Length != 1 ) {" );
			builder.AppendFormat ( $"\t\t\t\t{checkParamsCountFalse}\n", className, propertyInfo.Name, 1 );
			builder.AppendLine ( "\t\t\t}" );
			builder.AppendLine ();
		}
		
		private static void GenCheckParamCount ( string className, MethodInfo methodInfo, StringBuilder builder ) {
			ParameterInfo[] parameterInfos        = methodInfo.GetParameters (); 
			string          checkParamsCountFalse = "throw new ArgumentException ( $\"{0}.{1} parameter count mismatch: require {2} but got {{args.Length}}.\" );";

			if ( parameterInfos.Length == 0 ) {
				return;
			}

			builder.AppendLine ( "\t\t\tif ( " + $"args.Length != {parameterInfos.Length}" + " ) {" );
			builder.AppendFormat ( $"\t\t\t\t{checkParamsCountFalse}\n", className, methodInfo.Name, parameterInfos.Length );
			builder.AppendLine ( "\t\t\t}" );
			builder.AppendLine ();
		}
		
		private static void GenCheckOpFunctionParamCount ( string className, MethodInfo methodInfo, StringBuilder builder ) {
			string checkParamsCountFalse = "throw new ArgumentException ( $\"{0}.{1} parameter count mismatch: require {2} but got {{args.Length}}.\" );";
			
			builder.AppendLine ( "\t\t\tif ( args.Length != 1 ) {" );
			builder.AppendFormat ( $"\t\t\t\t{checkParamsCountFalse}\n", className, methodInfo.Name, 1 );
			builder.AppendLine ( "\t\t\t}" );
			builder.AppendLine ();
		}
		
		private static void GenCheckParamsType ( string className, FieldInfo fieldInfo, StringBuilder builder ) {
			int    index               = 0;
			string checkParamTypeFalse = "throw new ArgumentException ( $\"{2}.{3} parameter type mismatch: require {0} but got {{mRubyDLL.mrb_type ( args[ {1} ] )}}.\" );";
			string checkTypeFunction   = null;
			if ( fieldInfo.FieldType.IsValueType ) {
				if ( fieldInfo.FieldType == typeof ( int ) || fieldInfo.FieldType == typeof ( short ) || fieldInfo.FieldType == typeof ( uint )  || fieldInfo.FieldType == typeof ( long ) ) {
					checkTypeFunction = $"!mrb_value.IsFixnum ( args[ {index} ] )";
				}
				else if ( fieldInfo.FieldType == typeof ( float ) || fieldInfo.FieldType == typeof ( double ) ) {
					checkTypeFunction = $"!mrb_value.IsFloat ( args[ {index} ] )";
				}
				else if ( fieldInfo.FieldType == typeof ( bool ) ) {
					checkTypeFunction = $"!mrb_value.IsBool ( args[ {index} ] )";
				}
				else {
					checkTypeFunction = $"!mrb_value.IsData ( args[ {index} ] ) && !mrb_value.IsNil ( args[ {index} ] )";
				}
			}
			else {
				if ( fieldInfo.FieldType == typeof ( string ) ) {
					checkTypeFunction = $"!mrb_value.IsString ( args[ {index} ] )";
				}
				else {
					checkTypeFunction = $"!mrb_value.IsData ( args[ {index} ] ) && !mrb_value.IsNil ( args[ {index} ] )";
				}
			}

			builder.AppendLine ( "\t\t\tif ( " + $"{checkTypeFunction}" + " ) {" );
			builder.AppendFormat ( $"\t\t\t\t{checkParamTypeFalse}\n", fieldInfo.FieldType.Name, index, className, fieldInfo.Name );
			builder.AppendLine ( "\t\t\t}" );
			builder.AppendLine ();
		}
		
		private static void GenCheckParamsType ( string className, PropertyInfo propertyInfo, StringBuilder builder ) {
			int    index               = 0;
			string checkParamTypeFalse = "throw new ArgumentException ( $\"{2}.{3} parameter type mismatch: require {0} but got {{mRubyDLL.mrb_type ( args[ {1} ] )}}.\" );";
			string checkTypeFunction   = null;
			Type propertyType = propertyInfo.PropertyType;
			if ( propertyType.IsValueType ) {
				if ( propertyType == typeof ( int ) || propertyType == typeof ( short ) || propertyType == typeof ( uint )  || propertyType == typeof ( long ) ) {
					checkTypeFunction = $"!mrb_value.IsFixnum ( args[ {index} ] )";
				}
				else if ( propertyType == typeof ( float ) || propertyType == typeof ( double ) ) {
					checkTypeFunction = $"!mrb_value.IsFloat ( args[ {index} ] )";
				}
				else if ( propertyType == typeof ( bool ) ) {
					checkTypeFunction = $"!mrb_value.IsBool ( args[ {index} ] )";
				}
				else {
					checkTypeFunction = $"!mrb_value.IsData ( args[ {index} ] ) && !mrb_value.IsNil ( args[ {index} ] )";
				}
			}
			else {
				if ( propertyType == typeof ( string ) ) {
					checkTypeFunction = $"!mrb_value.IsString ( args[ {index} ] )";
				}
				else {
					checkTypeFunction = $"!mrb_value.IsData ( args[ {index} ] ) && !mrb_value.IsNil ( args[ {index} ] )";
				}
			}

			builder.AppendLine ( "\t\t\tif ( " + $"{checkTypeFunction}" + " ) {" );
			builder.AppendFormat ( $"\t\t\t\t{checkParamTypeFalse}\n", propertyType.Name, index, className, propertyInfo.Name );
			builder.AppendLine ( "\t\t\t}" );
			builder.AppendLine ();
		}

		private static void GenCheckParamsType ( string className, MethodInfo methodInfo, StringBuilder builder ) {
			ParameterInfo[] parameterInfos = methodInfo.GetParameters ();
			for ( var i = 0; i < parameterInfos.Length; ++i ) {
				GenCheckIndexedParamType ( className, i, methodInfo, builder );
			}
			if ( parameterInfos.Length != 0 ) {
				builder.AppendLine ();
			}
		}
		
		private static void GenCheckOpFunctionParamsType ( string className, MethodInfo methodInfo, StringBuilder builder ) {
			int    index               = 0;
			string checkParamTypeFalse = "throw new ArgumentException ( $\"{2}.{3} parameter type mismatch: require {0} but got {{mRubyDLL.mrb_type ( args[ {1} ] )}}.\" );";
			string checkTypeFunction   = null;
			Type   parameterType        = methodInfo.GetParameters ()[ 1 ].ParameterType;
			if ( parameterType.IsValueType ) {
				if ( parameterType == typeof ( int ) || parameterType == typeof ( short ) || parameterType == typeof ( uint )  || parameterType == typeof ( long ) ) {
					checkTypeFunction = $"!mrb_value.IsFixnum ( args[ {index} ] )";
				}
				else if ( parameterType == typeof ( float ) || parameterType == typeof ( double ) ) {
					checkTypeFunction = $"!mrb_value.IsFloat ( args[ {index} ] )";
				}
				else if ( parameterType == typeof ( bool ) ) {
					checkTypeFunction = $"!mrb_value.IsBool ( args[ {index} ] )";
				}
				else {
					checkTypeFunction = $"!mrb_value.IsData ( args[ {index} ] ) && !mrb_value.IsNil ( args[ {index} ] )";
				}
			}
			else {
				if ( parameterType == typeof ( string ) ) {
					checkTypeFunction = $"!mrb_value.IsString ( args[ {index} ] )";
				}
				else {
					checkTypeFunction = $"!mrb_value.IsData ( args[ {index} ] ) && !mrb_value.IsNil ( args[ {index} ] )";
				}
			}

			builder.AppendLine ( "\t\t\tif ( " + $"{checkTypeFunction}" + " ) {" );
			builder.AppendFormat ( $"\t\t\t\t{checkParamTypeFalse}\n", parameterType.Name, index, className, parameterType.Name );
			builder.AppendLine ( "\t\t\t}" );
			builder.AppendLine ();
			builder.AppendLine ();
		}
		
		private static void GenCheckIndexedParamType ( string className, int index, MethodInfo methodInfo, StringBuilder builder ) {
			ParameterInfo parameterInfo = methodInfo.GetParameters ()[ index ]; 
			string checkParamTypeFalse = "throw new ArgumentException ( $\"{2}.{3} parameter type mismatch: require {0} but got {{mRubyDLL.mrb_type ( args[ {1} ] )}}.\" );";
			string checkTypeFunction = null;
			if ( parameterInfo.ParameterType.IsValueType ) {
				if ( parameterInfo.ParameterType == typeof ( int ) || parameterInfo.ParameterType == typeof ( short ) || parameterInfo.ParameterType == typeof ( uint )  || parameterInfo.ParameterType == typeof ( long ) ) {
					checkTypeFunction = $"!mrb_value.IsFixnum ( args[ {index} ] )";
				}
				else if ( parameterInfo.ParameterType == typeof ( float ) || parameterInfo.ParameterType == typeof ( double ) ) {
					checkTypeFunction = $"!mrb_value.IsFloat ( args[ {index} ] )";
				}
				else if ( parameterInfo.ParameterType == typeof ( bool ) ) {
					checkTypeFunction = $"!mrb_value.IsBool ( args[ {index} ] )";
				}
				else {
					checkTypeFunction = $"!mrb_value.IsData ( args[ {index} ] ) && !mrb_value.IsNil ( args[ {index} ] )";
				}
			}
			else {
				if ( parameterInfo.ParameterType == typeof ( string ) ) {
					checkTypeFunction = $"!mrb_value.IsString ( args[ {index} ] )";
				}
				else {
					checkTypeFunction = $"!mrb_value.IsData ( args[ {index} ] ) && !mrb_value.IsNil ( args[ {index} ] )";
				}
			}

			builder.AppendLine ( "\t\t\tif ( " + $"{checkTypeFunction}" + " ) {" );
			builder.AppendFormat ( $"\t\t\t\t{checkParamTypeFalse}\n", parameterInfo.ParameterType.Name, index, className, methodInfo.Name );
			builder.AppendLine ( "\t\t\t}" );
		}
		
		private static string GenCallFunctionParams ( MethodInfo methodInfo ) {
			
			string valueToCSInstance = "mRubyDLL.ValueToDataObject< {0} > ( mrb, {2}, {1}.data_type_ptr )";

			ParameterInfo[] parameterInfos = methodInfo.GetParameters ();
			List<string> paramsStr = new List< string >( parameterInfos.Length );
			for ( var i = 0; i < parameterInfos.Length; ++i ) {

				string valueToCSValue = string.Empty;

				var parameterInfo = parameterInfos[ i ];
				if ( parameterInfo.ParameterType.IsValueType ) {
					if ( parameterInfo.ParameterType == typeof ( int ) || parameterInfo.ParameterType == typeof ( short ) || parameterInfo.ParameterType == typeof ( uint )  || parameterInfo.ParameterType == typeof ( long ) ) {
						valueToCSValue = $"( {parameterInfo.ParameterType.Name} )mRubyDLL.mrb_fixnum ( args[ {i} ] )";
					}
					else if ( parameterInfo.ParameterType == typeof ( float ) || parameterInfo.ParameterType == typeof ( double ) ) {
						valueToCSValue = $"( {parameterInfo.ParameterType.Name} )mRubyDLL.mrb_float ( args[ {i} ] )";
					}
					else if ( parameterInfo.ParameterType == typeof ( bool ) ) {
						valueToCSValue = $"mrb_value.Test ( args[ {i} ] )";
					}
					else {
						valueToCSValue = string.Format ( valueToCSInstance, parameterInfo.ParameterType.Name, GetWrapperClassName ( parameterInfo.ParameterType ), $"args[ {i} ]" );
					}
				}
				else {
					if ( parameterInfo.ParameterType == typeof ( string ) ) {
						valueToCSValue = $"args[ {i} ].ToString ( state )";
					}
					else {
						valueToCSValue = string.Format ( valueToCSInstance, parameterInfo.ParameterType.Name, GetWrapperClassName ( parameterInfo.ParameterType ), $"args[ {i} ]" );
					}
				}
				
				paramsStr.Add ( valueToCSValue );
			}

			if ( paramsStr.Count == 0 ) {
				return String.Empty;
			}
			
			if ( paramsStr.Count == 1 ) {
				return $" {paramsStr[ 0 ]} ";
			}

			string ret = " ";

			for ( var i = 0; i < paramsStr.Count; ++i ) {
				ret += paramsStr[ i ];
				if ( i != paramsStr.Count - 1 ) {
					ret += ", ";
				}
				else {
					ret += " ";
				}
			}

			return ret;
		}
		
		public static bool Gen_OpFunction_IfExist ( Type type, string op, StringBuilder builder ) {
			
			var methodInfo = type.GetMethods ( BindingFlags.Static | BindingFlags.Public )
			                     .Where ( m => {
				                     if ( !m.Name.Equals ( op ) ) {
					                     return false;
				                     }
				                     var parameters = m.GetParameters ();
				                     if ( parameters.Length != 2 ) {
					                     return false;
				                     }
				                     if ( parameters[ 0 ].ParameterType != type ) {
					                     return false;
				                     }
				                     return true;
			                     } ).FirstOrDefault ();
			
			if ( methodInfo == null ) {
				return false;
			}
			
			builder.AppendFormat ( $"\t\t{RUBYSHARP_WrapFunction}" + " {{\n", $"_{op}" );
			builder.AppendFormat ( $"\t\t\t{RUBYSHARP_ValueToCSInstance}\n", type.Name );
			builder.AppendLine ();
			
			builder.AppendLine ( $"\t\t\t{RUBYSHARP_GetFunctionArgs}" );
			builder.AppendLine ();

			GenCheckOpFunctionParamCount ( type.Name, methodInfo, builder );
			GenCheckOpFunctionParamsType ( type.Name, methodInfo, builder );
			
			builder.AppendLine ( $"\t\t\tvar right = {GenValueToCSValue ( methodInfo.GetParameters ()[ 1 ].ParameterType, "args[ 0 ]" )};" );
			builder.AppendLine ( $"\t\t\tvar ret = instance {operator_methods[ op ]} right;" );
			builder.AppendLine ( $"\t\t\treturn {GenCSValueToValue ( methodInfo.ReturnType, "ret" )};" );
			
			builder.AppendLine ( "\t\t}\n" );
			
			return true;
		}
		
		private static string GenValueToCSValue ( Type type, string varName ) {
			
			string valueToCSInstance = "mRubyDLL.ValueToDataObject< {0} > ( mrb, self, {1}.data_type_ptr )";
			
			string valueToCSValue = string.Empty;
            
			if ( type.IsValueType ) {
				if ( type == typeof ( int ) || type == typeof ( short ) || type == typeof ( uint ) || type == typeof ( long ) ) {
					valueToCSValue = $"( {type.Name} )mRubyDLL.mrb_fixnum ( {varName} )";
				}
				else if ( type == typeof ( float ) || type == typeof ( double ) ) {
					valueToCSValue = $"( {type.Name} )mRubyDLL.mrb_float ( {varName} )";
				}
				else if ( type == typeof ( bool ) ) {
					valueToCSValue = $"mrb_value.Test ( {varName} )";
				}
				else {
					valueToCSValue = string.Format ( valueToCSInstance, type.FullName, GetWrapperClassName ( type ) );
				}
			}
			else {
				if ( type == typeof ( string ) ) {
					valueToCSValue = $"{varName}.ToString ( state )";
				}
				else {
					valueToCSValue = string.Format ( valueToCSInstance, type.FullName, GetWrapperClassName ( type ) );
				}
			}

			return valueToCSValue;
		}

		private static string GenCSValueToValue ( Type type, string retVarName ) {
			string csValueToValue = null;
			if ( type.IsValueType ) {
				if ( type == typeof ( int ) || type == typeof ( short ) || type == typeof ( uint )  || type == typeof ( long ) ) {
					csValueToValue = $"mRubyDLL.mrb_fixnum_value ( ( int ){retVarName} )";
				}
				else if ( type == typeof ( float ) || type == typeof ( double ) ) {
					csValueToValue = $"mRubyDLL.mrb_float_value ( mrb, ( double ){retVarName} )";
				}
				else if ( type == typeof ( bool ) ) {
					csValueToValue = $"mrb_value.Create ( {retVarName} )";
				}
				else {
					csValueToValue = $"mRubyDLL.DataObjectToValue ( mrb, {GetWrapperClassName ( type )}.@class, {GetWrapperClassName ( type )}.data_type_ptr, {retVarName} )";
				}
			}
			else {
				if ( type == typeof ( string ) ) {
					csValueToValue = $"mrb_value.Create ( mrb, {retVarName} )";
				}
				else {
					csValueToValue = $"mRubyDLL.DataObjectToValue ( mrb, {GetWrapperClassName ( type )}.@class, {GetWrapperClassName ( type )}.data_type_ptr, {retVarName} )";
				}
			}

			if ( string.IsNullOrEmpty ( csValueToValue ) ) {
				Console.WriteLine ( $"GenCSValueToValue type: {type} not impl." );
			}

			return csValueToValue;
		}
	}
}
