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

		
		/// <summary>
		/// DefineCSharpClass in ruby and set data type
		/// </summary>
		/// <param name="state"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IntPtr DefineCSharpClass ( RubyState state, Type type ) {
			// 模块和类的开头必须是大写字母
			IntPtr @class = IntPtr.Zero;
			string[] namespacePath = type.FullName.Split ( '.' );

			if ( namespacePath.Length == 1 ) {
				@class = RubyDLL.r_define_class ( state, type.Name, state.SystemObjectRClass );
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
						@class = RubyDLL.r_define_class_under ( state, @class, validName, state.SystemObjectRClass );
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
			IntPtr @class = IntPtr.Zero;
			string[] namespacePath = type.FullName.Split ( '.' );

			if ( namespacePath.Length == 1 ) {
				@class = RubyDLL.r_define_module ( state, type.Name );
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
						// RubyDLL.mrb_set_instance_tt ( @class, mrb_vtype.MRB_TT_DATA );

					}
					else {
						@class = RubyDLL.r_define_module_under ( state, @class, validName, IntPtr.Zero );
						// RubyDLL.mrb_set_instance_tt ( @class, mrb_vtype.MRB_TT_DATA );
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
				RubyDLL.r_define_const ( state, module, System.Enum.GetName ( type, i ), RubyDLL.mrb_fixnum_value ( state, i ) );
			}
		}

		
		public static void RegisterClass < T > ( RubyState state ) {
			
			Type type = typeof ( T );
			
			IntPtr @class = UserDataUtility.DefineCSharpClass ( state, type );
			
			mrb_data_type dataType = RubyState.ObjectDataType;
			IntPtr dataTypePtr = RubyState.ObjectDataTypePtr;
			// state.AffirmDataTypeStruct< T >( out dataType, out dataTypePtr );
			state.AddRegistedTypeInfo< T >( @class, dataType, dataTypePtr );
			RegistedTypeInfo registedTypeInfo = state.GetRegistedTypeInfo ( type );
			Dictionary< string, RubyDLL.RubyCSFunction > instanceFunction = new Dictionary< string, RubyDLL.RubyCSFunction > ();
			Dictionary< string, RubyDLL.RubyCSFunction > classFunction = new Dictionary< string, RubyDLL.RubyCSFunction > ();
			
			// Reg Ctor
			if ( !type.IsAbstract || !type.IsSealed ) {
				ConstructorInfo publicCtor = type.GetConstructors ( BindingFlags.Public | BindingFlags.Instance ).OrderBy ( c => c.GetParameters ().Length ).FirstOrDefault ();
				if ( publicCtor != null ) {

					// CallbackFunction function = CallbackFunction.FromMethodInfo ( publicCtor );

					RubyDLL.RubyCSFunction rubyFunction = ( mrb, self ) => {
						// T obj = RubyDLL.ValueToDataObject< T > ( mrb, function.Invoke ( state, self ), RubyState.DATA_TYPE_PTR );
						// RubyDLL.mrb_data_init ( self, RubyDLL.ObjectToInPtr ( obj ), RubyState.DATA_TYPE_PTR );
						
						object obj = Activator.CreateInstance ( type, RubyState.RubyFunctionParamsToObjects ( mrb, dataTypePtr ) );
						RubyDLL.mrb_data_init ( self, state.PushRegistedCSharpObject ( obj ), dataTypePtr );

						// 添加实例映射
						state.AddCRInstanceMap ( obj, self );
						
						return self;
					};

					RubyDLL.r_define_method ( state, @class, "initialize", rubyFunction, rb_args.REQ ( (uint)publicCtor.GetParameters ().Length ) );
					registedTypeInfo.ctorFunction = rubyFunction;
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

				CallbackFunction getFunc = CallbackFunction.FromFieldInfo_Get ( field, dataTypePtr );
				CallbackFunction setFunc = CallbackFunction.FromFieldInfo_Set ( field, dataTypePtr );

				RubyDLL.RubyCSFunction getFunction = ( mrb, self ) => {
					// T obj = RubyDLL.ValueToDataObject< T > ( mrb, self, dataTypePtr );
					object obj = RubyState.ValueToRefObject ( state, self, dataTypePtr );
					return getFunc.SetCallbackTarget ( obj ).Invoke ( state, self );
				};

				RubyDLL.r_define_method ( state, @class, field.Name, getFunction, rb_args.NONE () );
				instanceFunction.Add ( field.Name, getFunction );
				
				if ( setFunc != null ) {
					RubyDLL.RubyCSFunction setFunction = ( mrb, self ) => {
						// T obj = RubyDLL.ValueToDataObject< T > ( mrb, self, dataTypePtr );
						object obj = RubyState.ValueToRefObject ( state, self, dataTypePtr );
						return setFunc.SetCallbackTarget ( obj ).Invoke ( state, self );
					};

					RubyDLL.r_define_method ( state, @class, $"{field.Name}=", setFunction, rb_args.REQ ( 1 ) );
					instanceFunction.Add ( $"{field.Name}=", setFunction );
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
				
				CallbackFunction getFunc = CallbackFunction.FromPropertyInfo_Get ( property, dataTypePtr );
				CallbackFunction setFunc = CallbackFunction.FromPropertyInfo_Set ( property, dataTypePtr );

				if ( getFunc != null ) {
					RubyDLL.RubyCSFunction getFunction = ( mrb, self ) => {
						// T obj = RubyDLL.ValueToDataObject< T > ( mrb, self, dataTypePtr );
						object obj = RubyState.ValueToRefObject ( state, self, dataTypePtr );
						return getFunc.SetCallbackTarget ( obj ).Invoke ( state, self );
					};

					RubyDLL.r_define_method ( state, @class, property.Name, getFunction, rb_args.NONE () );
					instanceFunction.Add ( property.Name, getFunction );
				}
				
				if ( setFunc != null ) {
					RubyDLL.RubyCSFunction setFunction = ( mrb, self ) => {
						// T obj = RubyDLL.ValueToDataObject< T > ( mrb, self, dataTypePtr );
						object obj = RubyState.ValueToRefObject ( state, self, dataTypePtr );
						return setFunc.SetCallbackTarget ( obj ).Invoke ( state, self );
					};

					RubyDLL.r_define_method ( state, @class,  $"{property.Name}=", setFunction, rb_args.REQ ( 1 ) );
					instanceFunction.Add ( $"{property.Name}=", setFunction );
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

				if ( instanceFunction.ContainsKey ( method.Name ) ) {
					continue;
				}
				
				CallbackFunction function = CallbackFunction.FromMethodInfo ( method, dataTypePtr );

				RubyDLL.RubyCSFunction rubyFunction = ( mrb, self ) => {
					// T obj = RubyDLL.ValueToDataObject< T > ( mrb, self, dataTypePtr );
					object obj = RubyState.ValueToRefObject ( state, self, dataTypePtr );
					return function.SetCallbackTarget ( obj ).Invoke ( state, self );
				};

				RubyDLL.r_define_method ( state, @class, method.Name, rubyFunction, rb_args.ANY () );
				instanceFunction.Add ( method.Name, rubyFunction );
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
				
				if ( classFunction.ContainsKey ( method.Name ) ) {
					continue;
				}
				
				CallbackFunction function = CallbackFunction.FromMethodInfo ( method, dataTypePtr );

				RubyDLL.RubyCSFunction rubyFunction = ( mrb, self ) => {
					return function.Invoke ( state, self );
				};

				RubyDLL.r_define_class_method ( state, @class, method.Name, rubyFunction, rb_args.ANY () );
				classFunction.Add ( method.Name, rubyFunction );
			}
			
			// Reg Operator Function Static ??
			// 当前可以注册为ruby类方法问题，运算符在ruby中是实例方法，在C#中是静态方法
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

				CallbackFunction function = CallbackFunction.FromMethodInfo ( methodInfo, dataTypePtr );

				RubyDLL.RubyCSFunction rubyFunction = ( mrb, self ) => {
					return function.Invoke ( state, self );
				};

				RubyDLL.r_define_class_method ( state, @class, kv.Value, rubyFunction, rb_args.REQ ( 1 ) );
				classFunction.Add ( kv.Value, rubyFunction );
			}

			registedTypeInfo.instanceFunction = instanceFunction;
			registedTypeInfo.classFunction = classFunction;
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
		
		
		public static bool IsValueType ( object obj ) {
			Type t = obj.GetType ();
			return !t.IsEnum && t.IsValueType;
		}
	}
}
