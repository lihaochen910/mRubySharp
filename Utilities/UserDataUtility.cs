namespace RubySharp {
	
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	
	/// <summary>
	/// C#类运行时注册类到mruby(使用反射)
	/// </summary>
	public class UserDataUtility {
		
		private static Dictionary<string, string> operator_methods = new Dictionary<string, string> {
			// { "op_LogicalNot", "!" },
			{ "op_Addition", "+" }, { "op_Subtraction", "-" }, { "op_Multiply", "*" },
			{ "op_Division", "/" }, { "op_BitwiseAnd", "&" }, { "op_BitwiseOr", "|" },
			{ "op_ExclusiveOr", "^" }, { "op_OnesComplement", "~" }, { "op_Equality", "==" },
			{ "op_Inequality", "!=" }, { "op_LessThan", "<" }, { "op_GreaterThan", ">" },
			{ "op_LessThanOrEqual", "<=" }, { "op_GreaterThanOrEqual", ">=" }, { "op_LeftShift", "<<" },
			{ "op_RightShift", ">>" }, { "op_Modulus", "%" }
		};
		
#if MRUBY
		
		public static IntPtr DefineCSharpClass<T> ( RubyState state ) {
			Type type = typeof ( T );
			return DefineCSharpClass ( state, type );
		}

		public static IntPtr DefineCSharpClass ( RubyState state, Type type ) {
			// 模块和类的开头必须是大写字母
			IntPtr   @class        = IntPtr.Zero;
			string[] namespacePath = type.FullName.Split ( '.' );

			if ( namespacePath.Length == 1 ) {
				@class = RubyDLL.r_define_class ( state, type.Name, IntPtr.Zero );
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
						@class = RubyDLL.r_define_module ( state, validName );
						RubyDLL.mrb_set_instance_tt ( @class, rb_vtype.RUBY_T_DATA );

					}
					else if ( name.Equals ( namespacePath[ namespacePath.Length - 1 ] ) ) {
						@class = RubyDLL.r_define_class_under ( state, @class, validName, IntPtr.Zero );
						RubyDLL.mrb_set_instance_tt ( @class, rb_vtype.RUBY_T_DATA );
					}
					else {
						@class = RubyDLL.r_define_module_under ( state, @class, validName, IntPtr.Zero );
						RubyDLL.mrb_set_instance_tt ( @class, rb_vtype.RUBY_T_DATA );
					}
				}
			}

			return @class;
		}
		
		public static IntPtr DefineCSharpEnum ( RubyState state, Type type ) {
			// 模块的开头必须是大写字母
			IntPtr   @class        = IntPtr.Zero;
			string[] namespacePath = type.FullName.Split ( '.' );

			if ( namespacePath.Length == 1 ) {
				@class = RubyDLL.r_define_module ( state, type.Name );
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
						@class = RubyDLL.r_define_module ( state, validName );
						// mRubyDLL.mrb_set_instance_tt ( @class, mrb_vtype.MRB_TT_DATA );

					}
					else {
						@class = RubyDLL.r_define_module_under ( state, @class, validName, IntPtr.Zero );
						// mRubyDLL.mrb_set_instance_tt ( @class, mrb_vtype.MRB_TT_DATA );
					}
				}
			}

			return @class;
		}

		
		public static void RegisterType < T > ( RubyState state ) {
			Type type = typeof ( T );

			if ( type.IsEnum ) {
				RegisterEnum < T > ( state );
				return;
			}
			
			RegisterClass < T > ( state );
		}
		
		public static void RegisterEnum < T > ( RubyState state ) {
			
			Type type = typeof ( T );

			IntPtr module = UserDataUtility.DefineCSharpEnum ( state, type );
			
			foreach ( int i in System.Enum.GetValues ( type ) ) {
				RubyDLL.r_define_const ( state, module, System.Enum.GetName ( type, i ), RubyDLL.INT2FIX ( i ) );
			}
		}

		public static void RegisterClass < T > ( RubyState state ) {
			
			Type type = typeof ( T );
			
			IntPtr @class = UserDataUtility.DefineCSharpClass ( state, type );
			
			// Reg Ctor
			if ( !type.IsAbstract || !type.IsSealed ) {
				ConstructorInfo publicCtor = type.GetConstructors ( BindingFlags.Public | BindingFlags.Instance ).OrderBy ( c => c.GetParameters ().Length ).FirstOrDefault ();
				if ( publicCtor != null ) {

					CallbackFunction function = CallbackFunction.FromMethodInfo ( publicCtor );

					RubyDLL.RubyCSFunction rubyFunction = ( mrb, self ) => {
						T obj = RubyDLL.ValueToDataObject< T > ( mrb, function.Invoke ( state, self ), RubyState.DATA_TYPE_PTR );
						RubyDLL.mrb_data_init ( self, RubyDLL.ObjectToInPtr ( obj ), RubyState.DATA_TYPE_PTR );
						return self;
					};

					RubyDLL.r_define_method ( state, @class, "initialize", rubyFunction, rb_args.ANY () );
					
				}
			}
			
			// Reg Public Field Get Set
			IList<FieldInfo> publicFields = type.GetFields ( BindingFlags.Public | BindingFlags.Instance );
			foreach ( var field in publicFields ) {

				// skip Obsolete
				if ( field.IsDefined ( typeof ( System.ObsoleteAttribute ), false ) ) {
					continue;
				}
				
				if ( !TestTypeSupport ( field.FieldType ) ) {
					continue;
				}

				CallbackFunction getFunc = CallbackFunction.FromFieldInfo_Get ( field );
				CallbackFunction setFunc = CallbackFunction.FromFieldInfo_Set ( field );

				RubyDLL.RubyCSFunction getFunction = ( mrb, self ) => {
					T obj = RubyDLL.ValueToDataObject< T > ( mrb, self, RubyState.DATA_TYPE_PTR );
					return getFunc.SetCallbackTarget ( obj ).Invoke ( state, self );
				};

				RubyDLL.r_define_method ( state, @class, field.Name, getFunction, rb_args.NONE () );
				
				if ( setFunc != null ) {
					RubyDLL.RubyCSFunction setFunction = ( mrb, self ) => {
						T obj = RubyDLL.ValueToDataObject< T > ( mrb, self, RubyState.DATA_TYPE_PTR );
						return setFunc.SetCallbackTarget ( obj ).Invoke ( state, self );
					};

					RubyDLL.r_define_method ( state, @class, field.Name + "=", setFunction, rb_args.REQ ( 1 ) );
				}
			}
			
			// Reg Public Field Get Set
			IList<PropertyInfo> publicPropertys = type.GetProperties ( BindingFlags.Public | BindingFlags.Instance );
			foreach ( var property in publicPropertys ) {

				// skip Obsolete
				if ( property.IsDefined ( typeof ( System.ObsoleteAttribute ), false ) ) {
					continue;
				}
				
				if ( !TestTypeSupport ( property.PropertyType ) ) {
					continue;
				}
				
				CallbackFunction getFunc = CallbackFunction.FromPropertyInfo_Get ( property );
				CallbackFunction setFunc = CallbackFunction.FromPropertyInfo_Set ( property );

				if ( getFunc != null ) {
					RubyDLL.RubyCSFunction getFunction = ( mrb, self ) => {
						T obj = RubyDLL.ValueToDataObject< T > ( mrb, self, RubyState.DATA_TYPE_PTR );
						return getFunc.SetCallbackTarget ( obj ).Invoke ( state, self );
					};

					RubyDLL.r_define_method ( state, @class, property.Name, getFunction, rb_args.NONE () );
				}
				
				if ( setFunc != null ) {
					RubyDLL.RubyCSFunction setFunction = ( mrb, self ) => {
						T obj = RubyDLL.ValueToDataObject< T > ( mrb, self, RubyState.DATA_TYPE_PTR );
						return setFunc.SetCallbackTarget ( obj ).Invoke ( state, self );
					};

					RubyDLL.r_define_method ( state, @class, property.Name + "=", setFunction, rb_args.REQ ( 1 ) );
				}
			}
			
			// Gen Wrap Function
			IList< MethodInfo > publicMethods = type.GetMethods ( BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly )
			                                        .Where ( m => !m.IsSpecialName ).ToArray ();
			foreach ( var method in publicMethods ) {

				if ( !TestFunctionSupport ( method ) ) {
					Console.WriteLine ( $"{type.Name}.{method.Name} not support." );
					continue;
				}
				
				// TODO: 支持方法重载??
				// if ( generatedMethods.ContainsKey ( method.Name ) ) {
				// 	continue;
				// }
				
				CallbackFunction function = CallbackFunction.FromMethodInfo ( method );

				RubyDLL.RubyCSFunction rubyFunction = ( mrb, self ) => {
					T obj = RubyDLL.ValueToDataObject< T > ( mrb, self, RubyState.DATA_TYPE_PTR );
					return function.SetCallbackTarget ( obj ).Invoke ( state, self );
				};

				RubyDLL.r_define_method ( state, @class, method.Name, rubyFunction, rb_args.ANY () );
			}
			
			IList< MethodInfo > publicStaticMethods = type.GetMethods ( BindingFlags.Public | BindingFlags.Static )
			                                              .Where ( m => !m.IsSpecialName ).ToArray ();
			foreach ( var method in publicStaticMethods ) {

				if ( !TestFunctionSupport ( method ) ) {
					Console.WriteLine ( $"{type.Name}.{method.Name} not support." );
					continue;
				}
				
				// TODO: 支持方法重载??
				// if ( generatedMethods.ContainsKey ( method.Name ) ) {
				// 	continue;
				// }
				
				CallbackFunction function = CallbackFunction.FromMethodInfo ( method );

				RubyDLL.RubyCSFunction rubyFunction = ( mrb, self ) => {
					return function.Invoke ( state, self );
				};

				RubyDLL.r_define_class_method ( state, @class, method.Name, rubyFunction, rb_args.ANY () );
			}
			
			// Reg Operator Function Static ??
			foreach ( var kv in operator_methods ) {
				
				var methodInfo = type.GetMethods ( BindingFlags.Public | BindingFlags.Static )
				                     .Where ( m => {
					                     if ( !m.Name.Equals ( kv.Key ) ) {
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
					continue;
				}

				if ( !TestTypeSupport ( methodInfo.GetParameters ()[ 1 ].ParameterType ) ) {
					continue;
				}
				
				CallbackFunction function = CallbackFunction.FromMethodInfo ( methodInfo );

				RubyDLL.RubyCSFunction rubyFunction = ( mrb, self ) => {
					return function.Invoke ( state, self );
				};

				RubyDLL.r_define_class_method ( state, @class, kv.Value, rubyFunction, rb_args.ANY () );
			}
		}
#endif
		
		public static void TestPropertyCanAccess ( PropertyInfo propertyInfo, out bool getterCanAccess, out bool setterCanAccess ) {
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
			
			// pass by ref unsupport
			// if ( type.IsValueType && type.IsByRef ) {
			// 	return false;
			// }
			
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
				// if ( p.ParameterType.IsValueType && p.ParameterType.IsByRef ) {
				// 	return false;
				// }
				
				if ( !TestTypeSupport ( p.ParameterType ) ) {
					return false;
				}
				
			}
			
			if ( methodInfo.ReturnType != typeof ( void ) && !TestTypeSupport ( methodInfo.ReturnType ) ) {
				return false;
			}
			
			return true;
		}
	}
}
