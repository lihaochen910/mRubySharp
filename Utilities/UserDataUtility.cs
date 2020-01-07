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
		private const string RUBYSHARP_STATIC_FUNCTION_PREFIX = "STATIC_";
		private const string RUBYSHARP_FIELD_SET_FUNCTION_POSTFIX = "_eql";
		private const string RUBYSHARP_FIELD_RClassVarName = "@class";
		private const string RUBYSHARP_FIELD_RModuleVarName = "@module";
		private const string RUBYSHARP_FIELD_DataTypeVarName = "data_type_ptr";
		private const string RUBYSHARP_FIELD_MRubyStateVarName = "state";
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
			typeof ( string )
		};

		private static IList< Type > exportingTypeList;

		public static void GenByAssembly ( Assembly assembly ) {
			
			List<Type> generatedType = new List< Type >();
			var types = assembly.GetTypes ();
			
			UserDataUtility.exportingTypeList = types;
			
			foreach ( var type in types ) {
				bool result = GenCSharpClass ( type );
				if ( result ) {
					generatedType.Add ( type );
				}
			}
			
			UserDataUtility.exportingTypeList = null;

			GenBindingManifestFile ( generatedType );
		}

		public static void RegByNamespace ( mRubyState state, Assembly assembly, string @namespace ) {
			Type[] types = assembly.GetTypes ()
			                       .Where ( t => String.Equals ( t.Namespace, @namespace, StringComparison.Ordinal ) )
			                       .ToArray ();
			foreach ( var type in types ) {
				GenCSharpClass ( type );
			}
		}

		#region UnityEngine
#if UNITY_ENGINE
		public static void GenUnityEngineCommon () {
			
			IList< Type > typeList = new List< Type > {
				// 静态类
				typeof ( UnityEngine.Application ),
				typeof ( UnityEngine.Time ),
				typeof ( UnityEngine.Screen ),
				typeof ( UnityEngine.SleepTimeout ),
				typeof ( UnityEngine.Input ),
				typeof ( UnityEngine.Resources ),
				typeof ( UnityEngine.Physics ),
				typeof ( UnityEngine.Physics2D ),
				typeof ( UnityEngine.RenderSettings ),
				typeof ( UnityEngine.QualitySettings ),
				typeof ( UnityEngine.GL ),
				typeof ( UnityEngine.Graphics ),

				// struct
				typeof ( UnityEngine.Vector2 ),
				typeof ( UnityEngine.Vector2Int ),
				typeof ( UnityEngine.Vector3 ),
				typeof ( UnityEngine.Vector3Int ),
				typeof ( UnityEngine.Vector4 ),
				typeof ( UnityEngine.Quaternion ),
				typeof ( UnityEngine.Matrix4x4 ),
				typeof ( UnityEngine.Ray ),
				typeof ( UnityEngine.Ray2D ),
				typeof ( UnityEngine.Bounds ),
				typeof ( UnityEngine.Rect ),
				typeof ( UnityEngine.RectInt ),
				typeof ( UnityEngine.Color ),
				typeof ( UnityEngine.Color32 ),
				
				// comp
				typeof ( UnityEngine.Object ),
				typeof ( UnityEngine.Component ),
				typeof ( UnityEngine.Transform ),
				typeof ( UnityEngine.Material ),
				typeof ( UnityEngine.Light ),
				typeof ( UnityEngine.Rigidbody ),
				typeof ( UnityEngine.Camera ),
				typeof ( UnityEngine.AudioSource ),

				typeof ( UnityEngine.Behaviour ),
				typeof ( UnityEngine.MonoBehaviour ),
				typeof ( UnityEngine.GameObject ),
				typeof ( UnityEngine.TrackedReference ),
				typeof ( UnityEngine.Application ),
				typeof ( UnityEngine.Physics ),
				typeof ( UnityEngine.Collider ),
				typeof ( UnityEngine.Time ),
				typeof ( UnityEngine.Texture ),
				typeof ( UnityEngine.Texture2D ),
				typeof ( UnityEngine.Shader ),
				typeof ( UnityEngine.Renderer ),
				typeof ( UnityEngine.WWW ),
				typeof ( UnityEngine.Screen ),
				typeof ( UnityEngine.CameraClearFlags ),
				typeof ( UnityEngine.AudioClip ),
				typeof ( UnityEngine.AssetBundle ),
				typeof ( UnityEngine.ParticleSystem ),
				typeof ( UnityEngine.AsyncOperation ),
				typeof ( UnityEngine.LightType ),
				typeof ( UnityEngine.SleepTimeout ),
#if UNITY_5_3_OR_NEWER && !UNITY_5_6_OR_NEWER
				typeof(UnityEngine.UnityEngine.Experimental.Director.DirectorPlayer),
#endif
				typeof ( UnityEngine.Animator ),
				typeof ( UnityEngine.Input ),
				typeof ( UnityEngine.KeyCode ),
				typeof ( UnityEngine.SkinnedMeshRenderer ),
				typeof ( UnityEngine.Space ),


				typeof ( UnityEngine.MeshRenderer ),
#if !UNITY_5_4_OR_NEWER
				// typeof ( UnityEngine.ParticleEmitter ),
				// typeof ( UnityEngine.ParticleRenderer ),
				// typeof ( UnityEngine.ParticleAnimator ),
#endif

				typeof ( UnityEngine.BoxCollider ),
				typeof ( UnityEngine.MeshCollider ),
				typeof ( UnityEngine.SphereCollider ),
				typeof ( UnityEngine.CharacterController ),
				typeof ( UnityEngine.CapsuleCollider ),

				typeof ( UnityEngine.Animation ),
				typeof ( UnityEngine.AnimationClip ),
				typeof ( UnityEngine.AnimationState ),
				typeof ( UnityEngine.AnimationBlendMode ),
				typeof ( UnityEngine.QueueMode ),
				typeof ( UnityEngine.PlayMode ),
				typeof ( UnityEngine.WrapMode ),

				typeof ( UnityEngine.QualitySettings ),
				typeof ( UnityEngine.RenderSettings ),
				typeof ( UnityEngine.BlendWeights ),
				typeof ( UnityEngine.RenderTexture ),
				typeof ( UnityEngine.Resources ),
			};

			UserDataUtility.exportingTypeList = typeList;

			foreach ( var type in typeList ) {
				GenCSharpClass ( type );
			}
			
			UserDataUtility.exportingTypeList = null;
			
			GenBindingManifestFile ( typeList );
		}
#endif
		#endregion
		
		public static void GenCSharpClass<T> () {
			Type type = typeof ( T );
			GenCSharpClass ( type );
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
		
		public static IntPtr DefineCSharpEnum ( mRubyState state, Type type ) {
			// 模块的开头必须是大写字母
			IntPtr   @class        = IntPtr.Zero;
			string[] namespacePath = type.FullName.Split ( '.' );

			if ( namespacePath.Length == 1 ) {
				@class = mRubyDLL.mrb_define_module ( state, type.Name );
			}
			else {
				foreach ( var name in namespacePath ) {

					string validName = name;
					
					// 检查命名开头字母大小写
					if ( !char.IsUpper ( name[ 0 ] ) ) {
						char   head    = char.ToUpper ( name[ 0 ] );
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
					else {
						@class = mRubyDLL.mrb_define_module_under ( state, @class, validName, IntPtr.Zero );
						mRubyDLL.mrb_set_instance_tt ( @class, mrb_vtype.MRB_TT_DATA );
					}
				}
			}

			return @class;
		}

		public static bool GenCSharpClass ( Type type ) {
			
			// Console.WriteLine ( $"Namespace: {type.Namespace}" );
			// Console.WriteLine ( $"FullName: {type.FullName}" );
			// Console.WriteLine ( $"Name: {type.Name}" );
			
			// skip Attribute
			if ( type.IsSubclassOf ( typeof ( System.Attribute ) ) ) {
				return false;
			}
			
			// skip Exception
			if ( type.IsSubclassOf ( typeof ( System.Exception ) ) ) {
				return false;
			}
			
			// TODO: 支持嵌套类
			if ( type.IsNested ) {
				return false;
			}
			
			// skip non public
			if ( !TestTypeIsPublic ( type ) ) {
				return false;
			}
			
			// skip interface
			if ( type.IsInterface ) {
				return false;
			}
			
			// TODO: 支持泛型类??
			if ( type.IsGenericType ) {
				return false;
			}
			
			if ( type.IsEnum ) {
				GenCSharpEnumClassSourceCode ( type, new StringBuilder () );
				return true;
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
		
		public static string GetShortWrapperClassName ( Type type ) {
			return $"{type.Name}{RUBYSHARP_WRAPPERCLASS_POSTFIX}";
		}

		public static void GenBindingManifestFile ( IList<Type> generatedType ) {
			
			StringBuilder manifest = new StringBuilder ();
			
			manifest.AppendLine ( "// This file was auto-generated by mRubySharp, do not modify it." );
			
			// Gen Using
			manifest.AppendLine ( "using System;" );
			manifest.AppendLine ( "using CandyFramework.mRuby;" );
			manifest.AppendLine ();
			
			manifest.AppendLine ( "public static class RubyBinder {" );
			manifest.AppendLine ();
			
			manifest.AppendLine ( "\tpublic static void Bind ( mRubyState state ) {" );
			manifest.AppendLine ();
			
			foreach ( var type in generatedType ) {
				manifest.AppendLine (
					$"\t\t{type.FullName}{RUBYSHARP_WRAPPERCLASS_POSTFIX}.{RUBYSHARP_StaticRegisterFunctionName} ( state );" );
			}
			manifest.AppendLine ();
			
			manifest.AppendLine ( "\t}" );
			manifest.AppendLine ( "}" );
			
			File.WriteAllText ( "_BindingManifest.cs", manifest.ToString () );
		}

		
		/// <summary>
		/// Gen Enum 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="builder"></param>
		public static void GenCSharpEnumClassSourceCode ( Type type, StringBuilder builder ) {
			
			string wrapperClassName = GetShortWrapperClassName ( type );
			bool   hasNamespace     = type.FullName.Split ( '.' ).Length != 1;
			
			
			// Gen Using
			builder.AppendLine ( "using System;" );
			builder.AppendLine ( "using System.Runtime.InteropServices;" );
			builder.AppendLine ( "using CandyFramework.mRuby;" );
			builder.AppendLine ();
			
			// Gen Namespace and Class Sign
			if ( hasNamespace ) {
				builder.AppendLine ( "namespace " + type.Namespace + " {" );
				builder.AppendLine ();
			}
			builder.AppendLine ( "\tpublic class " + wrapperClassName + " {" );
			builder.AppendLine ();
			
			// Gen Data Type Struct
			builder.AppendLine ( "\t\tpublic static readonly mrb_data_type data_type = new mrb_data_type () {" );
			builder.AppendLine ( $"\t\t\tstruct_name = \"{type.Name}\"," );
			builder.AppendLine ( "\t\t\tdfree = null" );
			builder.AppendLine ( "\t\t};" );
			builder.AppendLine ();
			
			// Gen Dep Static Field
			builder.AppendLine ( $"\t\tpublic static IntPtr {RUBYSHARP_FIELD_RModuleVarName};" );
			builder.AppendLine ( $"\t\tpublic static IntPtr {RUBYSHARP_FIELD_DataTypeVarName};" );
			builder.AppendLine ( $"\t\tpublic static mRubyState {RUBYSHARP_FIELD_MRubyStateVarName};" );
			builder.AppendLine ();
			
			// Gen Static Register function
			builder.AppendLine ( "\t\tpublic static void " + RUBYSHARP_StaticRegisterFunctionName + " ( mRubyState state ) {" );
			builder.AppendLine ( $"\t\t\t{wrapperClassName}.{RUBYSHARP_FIELD_MRubyStateVarName} = state;" );
			builder.AppendLine ( $"\t\t\t{wrapperClassName}.{RUBYSHARP_FIELD_RModuleVarName} = UserDataUtility.DefineCSharpEnum ( state, typeof ( {type.FullName} ) );" );
			builder.AppendLine ( $"\t\t\t{wrapperClassName}.{RUBYSHARP_FIELD_DataTypeVarName} = mRubyDLL.ObjectToInPtr ( data_type );" );
			builder.AppendLine ();
			foreach ( int i in System.Enum.GetValues ( type ) ) {
				builder.AppendLine ( $"\t\t\tmRubyDLL.mrb_define_const ( state, {RUBYSHARP_FIELD_RModuleVarName}, \"{System.Enum.GetName ( type, i )}\", mRubyDLL.mrb_fixnum_value ( ( int ){type.FullName}.{System.Enum.GetName ( type, i )} ) );" );
			}
			builder.AppendLine ();
			builder.AppendLine ( "\t\t}" );
			
			// Gen End
			builder.AppendLine ( "\t}" );
			if ( hasNamespace ) {
				builder.AppendLine ( "}" );
			}
			
			File.WriteAllText ( $"{type.FullName.Replace ( ".", "_" )}{RUBYSHARP_WRAPPERCLASS_POSTFIX}.cs", builder.ToString () );
		}
		
		public static void GenCSharpClassSourceCode ( Type type, StringBuilder builder ) {
			
			string wrapperClassName = GetShortWrapperClassName ( type );
			bool hasNamespace = type.FullName.Split ( '.' ).Length != 1;
			
			// key: name in ruby | value: name in C#
			Dictionary<string, string> generatedMethods = new Dictionary<string, string>();
			Dictionary<string, string> generatedClassMethods = new Dictionary<string, string>();

			// Gen Using
			builder.AppendLine ( "using System;" );
			builder.AppendLine ( "using System.Runtime.InteropServices;" );
			builder.AppendLine ( "using CandyFramework.mRuby;" );
			builder.AppendLine ();
			
			// Gen Namespace and Class Sign
			if ( hasNamespace ) {
				builder.AppendLine ( "namespace " + type.Namespace + " {" );
				builder.AppendLine ();
			}
			builder.AppendLine ( "\tpublic class " + wrapperClassName + " {" );
			builder.AppendLine ();
			
			// Gen Data Type Struct
			builder.AppendLine ( "\t\tpublic static readonly mrb_data_type data_type = new mrb_data_type () {" );
			builder.AppendLine ( $"\t\t\tstruct_name = \"{type.Name}\"," );
			builder.AppendLine ( "\t\t\tdfree = null" );
			builder.AppendLine ( "\t\t};" );
			builder.AppendLine ();
			
			// Gen Dep Static Field
			builder.AppendLine ( $"\t\tpublic static IntPtr {RUBYSHARP_FIELD_RClassVarName};" );
			builder.AppendLine ( $"\t\tpublic static IntPtr {RUBYSHARP_FIELD_DataTypeVarName};" );
			builder.AppendLine ( $"\t\tpublic static mRubyState {RUBYSHARP_FIELD_MRubyStateVarName};" );
			builder.AppendLine ();
			
			// Gen Ctor Function
			// TODO: 支持有参数构造方法
			if ( !type.IsAbstract || !type.IsSealed ) {
				ConstructorInfo publicCtor = type.GetConstructors ( BindingFlags.Public | BindingFlags.Instance ).Where ( c => c.GetParameters ().Length == 0 ).FirstOrDefault ();
				if ( publicCtor != null ) {
					builder.AppendFormat ( $"\t\t{RUBYSHARP_WrapFunction}" + " {{\n", "initialize" );
					builder.AppendFormat ( "\t\t\t{0} instance = new {0} ();\n", type.Name );
					builder.AppendLine ( $"\t\t\tmRubyDLL.mrb_data_init ( self, mRubyDLL.ObjectToInPtr ( instance ), data_type_ptr );" );
					builder.AppendLine ( $"\t\t\treturn self;" );
					builder.AppendLine ( "\t\t}\n" );
					generatedMethods.Add ( "initialize", "initialize" );
				}
			}

			// Gen Public Field Get Set
			IList<FieldInfo> publicFields = type.GetFields ( BindingFlags.Public | BindingFlags.Instance );
			foreach ( var field in publicFields ) {
				if ( ignored_fields.Contains ( field.Name ) ) {
					continue;
				}

				// skip Obsolete
				if ( field.IsDefined ( typeof ( System.ObsoleteAttribute ), false ) ) {
					continue;
				}
				
				if ( !TestTypeSupport ( field.FieldType ) ) {
					continue;
				}
				
				GenGetSetInstanceFieldBody ( type.Name, field, builder );

				if ( !generatedMethods.ContainsKey ( field.Name ) ) {
					generatedMethods.Add ( field.Name, field.Name );
					if ( !field.IsInitOnly ) {
						generatedMethods.Add ( $"{field.Name}=", $"{field.Name}{RUBYSHARP_FIELD_SET_FUNCTION_POSTFIX}" );
					}
				}
			}
			
			// Gen Public Field Get Set
			IList<PropertyInfo> publicPropertys = type.GetProperties ( BindingFlags.Public | BindingFlags.Instance );
			foreach ( var property in publicPropertys ) {
				if ( ignored_fields.Contains ( property.Name ) ) {
					continue;
				}
				
				// skip Obsolete
				if ( property.IsDefined ( typeof ( System.ObsoleteAttribute ), false ) ) {
					continue;
				}
				
				if ( !TestTypeSupport ( property.PropertyType ) ) {
					continue;
				}
				
				GenGetSetInstancePropertyBody ( type.Name, property, builder );

				if ( !generatedMethods.ContainsKey ( property.Name ) ) {
					
					TestPropertyCanAccess ( property, out var getterCanAccess, out var setterCanAccess );
					
					if ( property.CanRead && getterCanAccess ) {
						generatedMethods.Add ( property.Name, property.Name );
					}
					if ( property.CanWrite && setterCanAccess ) {
						generatedMethods.Add ( $"{property.Name}=", $"{property.Name}{RUBYSHARP_FIELD_SET_FUNCTION_POSTFIX}" );
					}
				}
			}

			// Gen Wrap Function
			IList< MethodInfo > publicMethods = type.GetMethods ( BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly )
			                                        .Where ( m => !m.IsSpecialName ).ToArray ();
			foreach ( var method in publicMethods ) {
				if ( ignored_methods.Contains ( method.Name ) ) {
					continue;
				}

				if ( !TestFunctionSupport ( method ) ) {
					continue;
				}
				
				// TODO: 支持方法重载??
				if ( generatedMethods.ContainsKey ( method.Name ) ) {
					continue;
				}
				
				GenInstanceBindingFunctionBody ( type, string.Empty, method, builder );

				generatedMethods.Add ( method.Name, method.Name );
			}
			
			IList< MethodInfo > publicStaticMethods = type.GetMethods ( BindingFlags.Public | BindingFlags.Static )
			                                        .Where ( m => !m.IsSpecialName ).ToArray ();
			foreach ( var method in publicStaticMethods ) {
				if ( ignored_methods.Contains ( method.Name ) ) {
					continue;
				}
				
				if ( !TestFunctionSupport ( method ) ) {
					continue;
				}
				
				// TODO: 支持方法重载??
				if ( generatedClassMethods.ContainsKey ( method.Name ) ) {
					continue;
				}

				string staticMethodName = $"{RUBYSHARP_STATIC_FUNCTION_PREFIX}{method.Name}";
				
				GenStaticBindingFunctionBody ( type.Name, method.Name, method, builder );

				generatedClassMethods.Add ( method.Name, staticMethodName );
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
			if ( hasNamespace ) {
				builder.AppendLine ( "}" );
			}
			
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
			if ( !fieldInfo.IsInitOnly ) {
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
		}
		
		private static void GenGetSetInstancePropertyBody ( string className, PropertyInfo propertyInfo, StringBuilder builder ) {

			TestPropertyCanAccess ( propertyInfo, out var getterCanAccess, out var setterCanAccess );
			
			// Get
			if ( propertyInfo.CanRead && getterCanAccess ) {
				builder.AppendFormat ( $"\t\t{RUBYSHARP_WrapFunction}" + " {{\n", propertyInfo.Name );
				builder.AppendFormat ( $"\t\t\t{RUBYSHARP_ValueToCSInstance}\n", className );
				builder.AppendLine ( $"\t\t\treturn {GenCSValueToValue ( propertyInfo.PropertyType, $"instance.{propertyInfo.Name}" )};" );
				builder.AppendLine ( "\t\t}\n" );
			}
			
			// Set
			if ( propertyInfo.CanWrite && setterCanAccess ) {
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

		private static void GenInstanceBindingFunctionBody ( Type type, string functionName, MethodInfo methodInfo, StringBuilder builder ) {

			string className = type.Name;
			
			if ( string.IsNullOrEmpty ( functionName ) ) {
				functionName = methodInfo.Name;
			}
			
			builder.AppendFormat ( $"\t\t{RUBYSHARP_WrapFunction}" + " {{\n", functionName );
			builder.AppendFormat ( $"\t\t\t{RUBYSHARP_ValueToCSInstance}\n", type.FullName );
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
			
			builder.AppendFormat ( $"\t\t{RUBYSHARP_WrapFunction}" + " {{\n", $"{RUBYSHARP_STATIC_FUNCTION_PREFIX}{functionName}" );
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
			string checkTypeFunction   = GenValueCheck ( fieldInfo.FieldType, $"args[ {index} ]" );
			
			builder.AppendLine ( "\t\t\tif ( " + $"{checkTypeFunction}" + " ) {" );
			builder.AppendFormat ( $"\t\t\t\t{checkParamTypeFalse}\n", fieldInfo.FieldType.Name, index, className, fieldInfo.Name );
			builder.AppendLine ( "\t\t\t}" );
			builder.AppendLine ();
		}
		
		private static void GenCheckParamsType ( string className, PropertyInfo propertyInfo, StringBuilder builder ) {
			int    index               = 0;
			string checkParamTypeFalse = "throw new ArgumentException ( $\"{2}.{3} parameter type mismatch: require {0} but got {{mRubyDLL.mrb_type ( args[ {1} ] )}}.\" );";
			string checkTypeFunction   = GenValueCheck ( propertyInfo.PropertyType, $"args[ {index} ]" );

			builder.AppendLine ( "\t\t\tif ( " + $"{checkTypeFunction}" + " ) {" );
			builder.AppendFormat ( $"\t\t\t\t{checkParamTypeFalse}\n", propertyInfo.PropertyType.Name, index, className, propertyInfo.Name );
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
			Type   type                = methodInfo.GetParameters ()[ 1 ].ParameterType;
			string checkTypeFunction   = GenValueCheck ( type, $"args[ {index} ]" );

			builder.AppendLine ( "\t\t\tif ( " + $"{checkTypeFunction}" + " ) {" );
			builder.AppendFormat ( $"\t\t\t\t{checkParamTypeFalse}\n", type.Name, index, className, type.Name );
			builder.AppendLine ( "\t\t\t}" );
			builder.AppendLine ();
			builder.AppendLine ();
		}
		
		private static void GenCheckIndexedParamType ( string className, int index, MethodInfo methodInfo, StringBuilder builder ) {
			ParameterInfo parameterInfo = methodInfo.GetParameters ()[ index ]; 
			string checkParamTypeFalse = "throw new ArgumentException ( $\"{2}.{3} parameter type mismatch: require {0} but got {{mRubyDLL.mrb_type ( args[ {1} ] )}}.\" );";
			string checkTypeFunction = GenValueCheck ( parameterInfo.ParameterType, $"args[ {index} ]" );

			builder.AppendLine ( "\t\t\tif ( " + $"{checkTypeFunction}" + " ) {" );
			builder.AppendFormat ( $"\t\t\t\t{checkParamTypeFalse}\n", parameterInfo.ParameterType.Name, index, className, methodInfo.Name );
			builder.AppendLine ( "\t\t\t}" );
		}
		
		private static string GenCallFunctionParams ( MethodInfo methodInfo ) {
			
			ParameterInfo[] parameterInfos = methodInfo.GetParameters ();
			List<string> paramsStr = new List< string >( parameterInfos.Length );
			for ( var i = 0; i < parameterInfos.Length; ++i ) {
				paramsStr.Add ( GenValueToCSValue ( parameterInfos[ i ].ParameterType, $"args[ {i} ]" ) );
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

			if ( !TestTypeSupport ( methodInfo.GetParameters ()[ 1 ].ParameterType ) ) {
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

		private static string GenValueCheck ( Type type, string varName ) {
			string ret = null;
			
			if ( type.IsValueType ) {
				if ( type == typeof ( int ) || type == typeof ( short ) || type == typeof ( long ) ||
				     type == typeof ( uint ) || type == typeof ( ushort ) || type == typeof ( ulong ) ||
				     type == typeof ( int? ) || type == typeof ( short? ) || type == typeof ( long? ) ||
				     type == typeof ( uint? ) || type == typeof ( ushort? ) || type == typeof ( ulong? ) ||
				     type == typeof ( byte ) || type == typeof ( sbyte ) ||
				     type == typeof ( byte? ) || type == typeof ( sbyte? ) ||
				     type.IsEnum ) {
					ret = $"!mrb_value.IsFixnum ( {varName} )";
				}
				else if ( type == typeof ( float ) || type == typeof ( double ) ||
				          type == typeof ( float? ) || type == typeof ( double? ) ) {
					ret = $"!mrb_value.IsFloat ( {varName} )";
				}
				else if ( type == typeof ( bool ) || type == typeof ( bool? ) ) {
					ret = $"!mrb_value.IsBool ( {varName} )";
				}
				else {
					ret = $"!mrb_value.IsData ( {varName} ) && !mrb_value.IsNil ( {varName} )";
				}
			}
			else {
				if ( type == typeof ( string ) || type == typeof ( System.String ) ) {
					ret = $"!mrb_value.IsString ( {varName} )";
				}
				else {
					ret = $"!mrb_value.IsData ( {varName} ) && !mrb_value.IsNil ( {varName} )";
				}
			}

			return ret;
		}
		
		private static string GenValueToCSValue ( Type type, string varName ) {
			
			string valueToCSInstance = "mRubyDLL.ValueToDataObject< {0} > ( mrb, {2}, {1}.data_type_ptr )";
			
			string valueToCSValue = string.Empty;
            
			if ( type.IsValueType ) {
				if ( type == typeof ( int ) || type == typeof ( short ) || type == typeof ( long ) ||
				     type == typeof ( uint ) || type == typeof ( ushort ) || type == typeof ( ulong ) ||
				     type == typeof ( int? ) || type == typeof ( short? ) || type == typeof ( long? ) ||
				     type == typeof ( uint? ) || type == typeof ( ushort? ) || type == typeof ( ulong? ) ||
				     type == typeof ( byte ) || type == typeof ( sbyte ) ||
				     type == typeof ( byte? ) || type == typeof ( sbyte? ) ||
				     type.IsEnum ) {
					valueToCSValue = $"( {type.FullName} )mRubyDLL.mrb_fixnum ( {varName} )";
				}
				else if ( type == typeof ( float ) || type == typeof ( double ) ||
				          type == typeof ( float? ) || type == typeof ( double? ) ) {
					valueToCSValue = $"( {type.Name} )mRubyDLL.mrb_float ( {varName} )";
				}
				else if ( type == typeof ( bool ) || type == typeof ( bool? ) ) {
					valueToCSValue = $"mrb_value.Test ( {varName} )";
				}
				else if ( type.IsArray ) {
					valueToCSValue = $"( {type.FullName} )" + string.Format ( valueToCSInstance, "System.Array", GetWrapperClassName ( typeof ( System.Array ) ), varName );
				}
				else if ( System.Nullable.GetUnderlyingType ( type ) != null ) {
					valueToCSValue = string.Format ( valueToCSInstance, System.Nullable.GetUnderlyingType ( type ).FullName, GetWrapperClassName ( System.Nullable.GetUnderlyingType ( type ) ), varName );
				}
				else {
					valueToCSValue = string.Format ( valueToCSInstance, type.FullName, GetWrapperClassName ( type ), varName );
				}
			}
			else {
				if ( type == typeof ( string ) || type == typeof ( System.String ) ) {
					valueToCSValue = $"{varName}.ToString ( state )";
				}
				else if ( type.IsArray ) {
					valueToCSValue = $"( {type.FullName} )" + string.Format ( valueToCSInstance, "System.Array", GetWrapperClassName ( typeof ( System.Array ) ), varName );
				}
				else if ( type == typeof ( System.Type ) ) {
					valueToCSValue = $"( {type.FullName} )" + string.Format ( valueToCSInstance, "System.Object", GetWrapperClassName ( typeof ( System.Object ) ), varName );
				}
				else {
					valueToCSValue = string.Format ( valueToCSInstance, type.FullName, GetWrapperClassName ( type ), varName );
				}
			}

			return valueToCSValue;
		}

		private static string GenCSValueToValue ( Type type, string retVarName ) {
			string csValueToValue = null;
			if ( type.IsValueType ) {
				if ( type == typeof ( int ) || type == typeof ( short ) || type == typeof ( long ) ||
				     type == typeof ( uint ) || type == typeof ( ushort ) || type == typeof ( ulong ) ||
				     type == typeof ( byte ) || type == typeof ( sbyte ) ||
				     type.IsEnum ) {
					csValueToValue = $"mRubyDLL.mrb_fixnum_value ( ( int ){retVarName} )";
				}
				else if ( type == typeof ( int? ) || type == typeof ( short? ) || type == typeof ( long? ) ||
				          type == typeof ( uint? ) || type == typeof ( ushort? ) || type == typeof ( ulong? ) ||
				          type == typeof ( byte? ) || type == typeof ( sbyte? ) ) {
					csValueToValue = $"mRubyDLL.mrb_fixnum_value ( ( int )( {retVarName}.HasValue ? {retVarName}.Value : 0 ) )";
				}
				else if ( type == typeof ( float ) || type == typeof ( double ) ) {
					csValueToValue = $"mRubyDLL.mrb_float_value ( mrb, ( double ){retVarName} )";
				}
				else if ( type == typeof ( float? ) || type == typeof ( double? ) ) {
					csValueToValue = $"mRubyDLL.mrb_float_value ( mrb, ( double )( {retVarName}.HasValue ? {retVarName}.Value : 0f ) )";
				}
				else if ( type == typeof ( bool ) ) {
					csValueToValue = $"mrb_value.Create ( {retVarName} )";
				}
				else if ( type == typeof ( bool? ) ) {
					csValueToValue = $"mrb_value.Create ( {retVarName}.HasValue ? {retVarName}.Value : false )";
				}
				else if ( type.IsArray ) {
					csValueToValue = $"mRubyDLL.DataObjectToValue ( mrb, {GetWrapperClassName ( typeof ( System.Array ) )}.@class, {GetWrapperClassName ( typeof ( System.Array ) )}.data_type_ptr, {retVarName} )";
				}
				else if ( System.Nullable.GetUnderlyingType ( type ) != null ) {
					csValueToValue = $"mRubyDLL.DataObjectToValue ( mrb, {GetWrapperClassName ( System.Nullable.GetUnderlyingType ( type ) )}.@class, {GetWrapperClassName ( System.Nullable.GetUnderlyingType ( type ) )}.data_type_ptr, {retVarName} )";
				}
				else {
					csValueToValue = $"mRubyDLL.DataObjectToValue ( mrb, {GetWrapperClassName ( type )}.@class, {GetWrapperClassName ( type )}.data_type_ptr, {retVarName} )";
				}
			}
			else {
				if ( type == typeof ( string ) || type == typeof ( System.String ) ) {
					csValueToValue = $"mrb_value.Create ( mrb, {retVarName} )";
				}
				else if ( type.IsArray ) {
					csValueToValue = $"mRubyDLL.DataObjectToValue ( mrb, {GetWrapperClassName ( typeof ( System.Array ) )}.@class, {GetWrapperClassName ( typeof ( System.Array ) )}.data_type_ptr, {retVarName} )";
				}
				else if ( type == typeof ( System.Type ) ) {
					csValueToValue = $"mRubyDLL.DataObjectToValue ( mrb, {GetWrapperClassName ( typeof ( System.Object ) )}.@class, {GetWrapperClassName ( typeof ( System.Object ) )}.data_type_ptr, {retVarName} )";
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

		private static bool TestFunctionParamTypeSupport ( Type type ) {
			
			if ( !TestTypeSupport ( type ) ) {
				return false;
			}

			if ( type.IsInterface ) {
				return false;
			}
			
			return true;
		}

		private static void TestPropertyCanAccess ( PropertyInfo propertyInfo, out bool getterCanAccess, out bool setterCanAccess ) {
			getterCanAccess = setterCanAccess = false;
			
			foreach ( var accessor in propertyInfo.GetAccessors () ) {
				if ( accessor.ReturnType == typeof ( void ) ) {
					setterCanAccess = accessor.IsPublic && !accessor.IsFamily;
				}
				else {
					getterCanAccess = accessor.IsPublic && !accessor.IsFamily;
				}
			}
		}

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
				
				if ( !TestFunctionParamTypeSupport ( p.ParameterType ) ) {
					return false;
				}
				
			}
			
			if ( !TestFunctionParamTypeSupport ( methodInfo.ReturnType ) ) {
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
