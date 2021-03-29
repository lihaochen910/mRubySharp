namespace RubySharp {
    
    using System;
    using System.IO;
    using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;
	using RubySharp.Utilities;


	public class RubyState : IDisposable {
        
        public IntPtr rb_state { get; private set; }

        public R_VAL rb_object_class  { get; private set; }
        public R_VAL rb_kernel_module { get; private set; }

        public IntPtr SystemObjectRClass { get; private set; }

        /// <summary>
        /// 保存当前VM从C#传入mruby的方法指针，防止C#方法指针进入mruby后被C#端GC掉
        /// </summary>
        public List< Delegate > MethodDelegates { get; private set; } = new List< Delegate > ();
        
        /// <summary>
        /// search path
        /// </summary>
        private IList<string> requiredSearchPaths = new List<string> ();
		
		private ObjectTranslator translator = new ObjectTranslator ();

		// private Dictionary< int, GCHandle > gcHandleDictionary = new Dictionary< int, GCHandle > ();
		private Map< IntPtr, int > typedReferenceMap = new Map< IntPtr , int > ();
		private Map< object, R_VAL > dataValueMap = new Map< object , R_VAL > ();

		// 记录绑定类传递到ruby中mrb_data_type结构体, 包括反射绑定和生成代码绑定
		private Dictionary< Type, mrb_data_type > registedDataTypeDictionary = new Dictionary< Type, mrb_data_type > ();
		private Dictionary< Type, IntPtr > registedDataTypePtrDictionary = new Dictionary< Type, IntPtr > ();
		private Dictionary< Type, RegistedTypeInfo > registedTypeInfoDictionary = new Dictionary< Type, RegistedTypeInfo > ();

		
        public RubyState () {
            #if MRUBY
            
            rb_state = RubyDLL.mrb_open ();
            
            #else
	        
            string[] argv = System.Environment.GetCommandLineArgs ();
            int argc = argv.Length;
            RubyDLL.ruby_sysinit ( ref argc, argv );
            RubyDLL.ruby_init ();
            RubyDLL.ruby_init_loadpath ();
            
            #endif
			
			rb_object_class = DoString ( "Object" );
			rb_kernel_module = DoString ( "Kernel" );
            
            InitBaseTypeBinding ();
            
#if MRUBY
            // load file
            DefineMethod ( "dofile", _doFile, rb_args.REQ ( 1 ) );
#endif
        }

		
        internal void InitBaseTypeBinding () {
#if MRUBY
            // SystemObjectRClass = UserDataUtility.DefineCSharpClass ( this, typeof ( System.Object ) );
			
			AffirmDataTypeStruct< System.Object > ( out _ObjectDataType, out _ObjectDataTypePtr );
			AffirmDataTypeStruct< System.Enum > ( out _EnumDataType, out _EnumDataTypePtr );
			
			UserDataUtility.RegisterClass< System.Object > ( this );
			SystemObjectRClass = GetRegistedTypeInfo< System.Object > ().@class;
			UserDataUtility.RegisterClass< System.Type > ( this );
			// UserDataUtility.RegisterClass< System.Array > ( this );
			UserDataUtility.RegisterClass< System.Delegate > ( this );
#endif
        }
		
		
		public void GC () {
#if MRUBY
			RubyDLL.mrb_garbage_collect ( rb_state );
#endif
		}


		public void FullGC () {
#if MRUBY
			RubyDLL.mrb_full_gc ( rb_state );
#endif
		}

		
        public R_VAL DoString ( string str ) {
#if MRUBY
            int arena = RubyDLL.mrb_gc_arena_save ( rb_state );
            IntPtr mrbc_context = RubyDLL.mrbc_context_new ( rb_state );
            RubyDLL.mrbc_filename ( this, mrbc_context, "*interactive*" );
            var ret = RubyDLL.mrb_load_string_cxt ( rb_state, RubyDLL.ToCBytes ( str ), mrbc_context );
            RubyDLL.mrbc_context_free ( rb_state, mrbc_context );
            
            if ( RubyDLL.mrb_has_exc ( rb_state ) ) {
                Console.WriteLine ( GetExceptionBackTrace () );
				RubyDLL.mrb_exc_clear ( rb_state );
            }
            RubyDLL.mrb_gc_arena_restore ( rb_state, arena );
            return ret;
#else
			int status;
			
			// return RubyDLL.rb_eval_string ( str );
            return RubyDLL.rb_eval_string_protect ( str, out status );
#endif
        }
        
		
        public R_VAL DoFile ( string path ) {
            
            if ( !File.Exists ( path ) ) {
                return R_VAL.NIL;
            }
            
#if MRUBY
            string filename = Path.GetFileName ( path );
            int arena = RubyDLL.mrb_gc_arena_save ( rb_state );
            IntPtr mrbc_context = RubyDLL.mrbc_context_new ( rb_state );
            RubyDLL.mrbc_filename ( this, mrbc_context, filename );
            var ret = RubyDLL.mrb_load_string_cxt ( rb_state, RubyDLL.ToCBytes ( File.ReadAllText ( path ) ), mrbc_context );
            RubyDLL.mrbc_context_free ( rb_state, mrbc_context );
            if ( RubyDLL.mrb_has_exc ( rb_state ) ) {
                Console.WriteLine ( GetExceptionBackTrace () );
				RubyDLL.mrb_exc_clear ( rb_state );
            }
            RubyDLL.mrb_gc_arena_restore ( rb_state, arena );
            return ret;
#else
            int status;
            RubyDLL.rb_load_protect ( R_VAL.Create ( path ), 0, out status );
            return status == 0 ? R_VAL.TRUE : R_VAL.FALSE;
#endif
        }
        
		
        public static R_VAL DoFile ( IntPtr mrb, string path ) {
            
            if ( !File.Exists ( path ) ) {
                return R_VAL.NIL;
            }
            
#if MRUBY
            string filename = Path.GetFileName ( path );
            int arena = RubyDLL.mrb_gc_arena_save ( mrb );
            IntPtr mrbc_context = RubyDLL.mrbc_context_new ( mrb );
            RubyDLL.mrbc_filename ( mrb, mrbc_context, filename );
            var ret = RubyDLL.mrb_load_string_cxt ( mrb, RubyDLL.ToCBytes ( File.ReadAllText ( path ) ), mrbc_context );
            RubyDLL.mrbc_context_free ( mrb, mrbc_context );
            if ( RubyDLL.mrb_has_exc ( mrb ) ) {
                Console.WriteLine ( GetExceptionBackTrace ( mrb ) );
				RubyDLL.mrb_exc_clear ( mrb );
            }
            RubyDLL.mrb_gc_arena_restore ( mrb, arena );
            return ret;
#else
            int status;
            RubyDLL.rb_load_protect ( R_VAL.Create ( path ), 0, out status );
            return status == 0 ? R_VAL.TRUE : R_VAL.FALSE;
#endif
        }

		
#if MRUBY
        public R_VAL DoByteCode ( byte[] bytecode ) {
            return RubyDLL.mrb_load_irep ( rb_state, bytecode );
        }
#endif
        
		
        public bool RequirePath ( string path ) {
            if ( Directory.Exists ( path ) ) {
                requiredSearchPaths.Insert ( 0, Path.GetDirectoryName ( path ) );
                return true;
            }
            return false;
        }

		
        public R_VAL Call ( string funcName ) {
#if MRUBY
            return RubyDLL.r_funcall ( rb_state, RubyDLL.mrb_top_self ( rb_state ), funcName, 0 );
#else
            return RubyDLL.r_funcall ( RubyDLL.rb_vm_top_self (), R_VAL.Create ( funcName ), 0 );
#endif
        }

		
        public R_VAL Call ( string funcName, R_VAL arg ) {
#if MRUBY
            return RubyDLL.r_funcall_1 ( rb_state, RubyDLL.mrb_top_self ( rb_state ), funcName, 1, arg );
#else
            return RubyDLL.r_funcall_1 ( RubyDLL.rb_vm_top_self (), R_VAL.Create ( funcName ), 1, arg );
#endif
        }


#if MRUBY
        /// <summary>
        /// 获取当前异常调用堆栈信息
        /// https://qiita.com/seanchas_t/items/ca293f9dd4454cd6cb6d
        /// </summary>
        public string GetExceptionBackTrace () {
            
            System.Text.StringBuilder builder = new System.Text.StringBuilder ();
            

            R_VAL exc = RubyDLL.mrb_get_exc_value ( rb_state );
			if ( !RubyDLL.mrb_exception_p ( exc ) ) {
				return string.Empty;
			}
            R_VAL backtrace = RubyDLL.r_exc_backtrace ( rb_state, exc );
            
            builder.AppendLine ( RubyDLL.r_funcall ( rb_state, exc, "inspect", 0 ).ToString ( rb_state ) );

            builder.AppendLine ( "trace:" );
            for ( var i = 0; i < RubyDLL.r_funcall ( rb_state, backtrace, "size", 0 ); ++i ) {
                R_VAL v = RubyDLL.r_ary_ref ( rb_state, backtrace, i );
                builder.AppendLine ( $"  [{i}] {v.ToString ( rb_state )}" );
            }

            return builder.ToString ();

			// R_VAL exc = RubyDLL.rb_errinfo ();
			// R_VAL backtrace = RubyDLL.r_exc_backtrace ( exc );
   //          
			// builder.AppendLine ( RubyDLL.r_inspect ( exc ).ToString () );
			//
			// builder.AppendLine ( "trace:" );
			// for ( var i = 0; i < RubyDLL.r_funcall ( backtrace, R_VAL.Create ( "size" ), 0 ); ++i ) {
			// 	R_VAL v = RubyDLL.r_ary_ref ( backtrace, R_VAL.Create ( i ) );
			// 	builder.AppendLine ( $"  [{i}] {v.ToString ()}" );
			// }
			//
			// return builder.ToString ();
        }
#endif
        
		
#if MRUBY
        public static string GetExceptionBackTrace ( IntPtr mrb ) {
#else
		public static string GetExceptionBackTrace () {
#endif
            
            System.Text.StringBuilder builder = new System.Text.StringBuilder ();
            
#if MRUBY
            R_VAL exc = RubyDLL.mrb_get_exc_value ( mrb );
            R_VAL backtrace = RubyDLL.r_exc_backtrace ( mrb, exc );
            
            builder.AppendLine ( RubyDLL.r_funcall ( mrb, exc, "inspect", 0 ).ToString ( mrb ) );

            builder.AppendLine ( "trace:" );
            for ( var i = 0; i < RubyDLL.r_funcall ( mrb, backtrace, "size", 0 ); ++i ) {
                R_VAL v = RubyDLL.r_ary_ref ( mrb, backtrace, i );
                builder.AppendLine ( $"  [{i}] {v.ToString ( mrb )}" );
            }
            
            return builder.ToString ();
#else
			R_VAL exc = RubyDLL.rb_errinfo ();
			R_VAL backtrace = RubyDLL.r_exc_backtrace ( exc );
            
			builder.AppendLine ( RubyDLL.rb_obj_inspect ( exc ).ToString () );

			builder.AppendLine ( "trace:" );
			for ( var i = 0; i < RubyDLL.r_funcall ( backtrace, R_VAL.Create ( "size" ), 0 ); ++i ) {
				R_VAL v = RubyDLL.r_ary_ref ( backtrace, R_VAL.Create ( i ) );
				builder.AppendLine ( $"  [{i}] {v.ToString ()}" );
			}
            
			return builder.ToString ();
#endif
        }

        
        public string GetCurrentBackTrace () {
#if MRUBY
            System.Text.StringBuilder builder = new System.Text.StringBuilder ();
            
            R_VAL backtrace = RubyDLL.r_get_backtrace ( rb_state );
            
            builder.AppendLine ( "trace:" );
            for ( var i = 0; i < RubyDLL.r_funcall ( rb_state, backtrace, "size", 0 ); ++i ) {
                R_VAL v = RubyDLL.r_ary_ref ( rb_state, backtrace, i );
                builder.AppendLine ( $"  [{i}] {v.ToString ( rb_state )}" );
            }
            
            return builder.ToString ();
#else
            System.Text.StringBuilder builder = new System.Text.StringBuilder ();

			R_VAL exc = DoString ( "Exception.new('*interactive*')" );
            R_VAL backtrace = RubyDLL.r_get_backtrace ( exc );
            
            builder.AppendLine ( "trace:" );
            for ( var i = 0; i < RubyDLL.r_funcall ( backtrace, R_VAL.Create ( "size" ), 0 ); ++i ) {
                R_VAL v = RubyDLL.r_ary_ref ( backtrace, R_VAL.Create ( i ) );
                builder.AppendLine ( $"  [{i}] {v.ToString ()}" );
            }
            
            return builder.ToString ();
#endif
            // return RubyDLL.mrb_inspect ( mrb_state, RubyDLL.mrb_get_backtrace ( mrb_state ) ).ToString ( mrb_state );
            // return RubyDLL.mrb_funcall ( mrb_state, RubyDLL.mrb_exc_backtrace ( mrb_state, RubyDLL.mrb_get_exc_value ( mrb_state ) ), "to_s", 0 ).ToString ( mrb_state );
            // return RubyDLL.mrb_inspect ( mrb_state, RubyDLL.mrb_get_exc_value ( mrb_state ) ).ToString ( mrb_state );
            // return RubyDLL.mrb_inspect ( mrb_state, RubyDLL.mrb_exc_backtrace ( mrb_state, RubyDLL.mrb_get_exc_value ( mrb_state ) ) ).ToString ( mrb_state );
        }

		
        /// <summary>
        /// 在mruby全局中定义方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="receiver"></param>
        /// <param name="aspec"></param>
        public void DefineMethod ( string name, RubyDLL.RubyCSFunction receiver, rb_args aspec ) {
            // 防止被C#端GC
            MethodDelegates.Add ( receiver );

#if MRUBY
            RubyDLL.r_define_module_function ( rb_state, rb_kernel_module, name, receiver, aspec );
#else
	        // RubyDLL.RubyCSFunction receiverDelegate = receiver;
	        // GCHandle gch = GCHandle.Alloc(receiverDelegate);
	        // IntPtr intptr_delegate = Marshal.GetFunctionPointerForDelegate(receiverDelegate);
            RubyDLL.r_define_module_function ( rb_kernel_module, name, receiver, aspec.value );
#endif
        }
        
		
#if MRUBY
        /// <summary>
        /// 在mruby全局中定义方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="delegate">实例方法/静态方法</param>
        /// <param name="aspec"></param>
        public void DefineMethod ( string name, Delegate @delegate, rb_args aspec ) {

            var function = CallbackFunction.FromDelegate ( @delegate, RubyState.ObjectDataTypePtr );
			var method = new RubyDLL.RubyCSFunction ( ( state, self ) => function.Invoke ( this, self ) );
            
            // 防止被C#端GC
            MethodDelegates.Add ( method );

#if MRUBY
            RubyDLL.r_define_module_function ( rb_state, rb_kernel_module, name, method, aspec );
#else
            RubyDLL.r_define_module_function ( rb_kernel_module, name, method, aspec );
#endif
        }
#endif


		#region 查询注册类信息

		public RegistedTypeInfo GetRegistedTypeInfo< T > () {
			return GetRegistedTypeInfo ( typeof ( T ) );
		}


		public RegistedTypeInfo GetRegistedTypeInfo ( Type t ) {
			foreach ( var kv in registedTypeInfoDictionary ) {
				if ( kv.Key == t ) {
					return kv.Value;
				}
			}

			return null;
		}

		
		public void AddRegistedTypeInfo< T > ( IntPtr @class, mrb_data_type dataType, IntPtr dataTypePtr ) {
			registedTypeInfoDictionary.Add ( typeof ( T ),
				new RegistedTypeInfo () { @class = @class, dataType = dataType, dataTypePtr = dataTypePtr } );
		}
		
		#endregion


		#region C#与ruby沟通的GC处理

		/// <summary>
		/// 向ruby中传递C#引用类型非空实例之前, 应该先调用此方法将绑定实例进行管理, 这样不会被C#释放
		/// </summary>
		/// <param name="obj">已在ruby中注册的C#实例</param>
		/// <returns>可以传递给ruby中的非托管地址</returns>
		public IntPtr PushRegistedCSharpObject ( object obj ) {
			
			#if DEBUG
			if ( obj == null ) {
				return IntPtr.Zero;
			}
			if ( IsValueType ( obj ) ) {
				return IntPtr.Zero;
			}
			#endif
			
			int index;
			if ( !translator.Getudata ( obj, out index ) ) {
				index = translator.AddObject ( obj );
				
				// using GCHandle
				// GCHandle handle = GCHandle.Alloc ( obj, GCHandleType.WeakTrackResurrection );
				// gcHandleDictionary.Add ( index, handle );
				
				// using AllocHGlobal
				IntPtr indexPtr = Marshal.AllocHGlobal ( Marshal.SizeOf ( index ) );
				typedReferenceMap.Add ( indexPtr, index );
				
				#if TRACING_COMMUNICATION
				Console.WriteLine ( $"[DEBUG] PushRegistedCSharpObject: {obj} #{index} 0x{indexPtr.ToString("X")} hash:{obj.GetHashCode()}" );
				#endif
				
				// return GCHandle.ToIntPtr ( handle );
				return indexPtr;
			}
			
			#if TRACING_COMMUNICATION
			Console.WriteLine ( $"[DEBUG] PushRegistedCSharpObject(exist): {obj} #{index} 0x{typedReferenceDictionaryReverse[ index ].ToString("X")} hash:{obj.GetHashCode()}" );
			#endif

			// 使用已注册过的实例
			// return GCHandle.ToIntPtr ( gcHandleDictionary[ index ] );
			return typedReferenceMap.Reverse[ index ];
		}
		
		
		/// <summary>
		/// 某些情况向ruby中直接传递C#引用
		/// TODO: 如果该实例类型是注册类型,需要获得对应ruby实例,创建或使用现有的？
		/// </summary>
		/// <param name="obj">未在ruby中注册的C#实例</param>
		/// <returns>可以传递给ruby中的非托管地址</returns>
		public IntPtr PushCSharpObject ( object obj ) {
			
#if DEBUG
			if ( obj == null ) {
				return IntPtr.Zero;
			}
			if ( IsValueType ( obj ) ) {
				return IntPtr.Zero;
			}
#endif

			
			
			int index;
			if ( !translator.Getudata ( obj, out index ) ) {
				index = translator.AddObject ( obj );
				
				IntPtr indexPtr = Marshal.AllocHGlobal ( Marshal.SizeOf ( index ) );
				typedReferenceMap.Add ( indexPtr, index );
				
#if DEBUG
				Console.WriteLine ( $"[DEBUG] PushCSharpObject: {obj} #{index} hash:{obj.GetHashCode()}" );
#endif
				
				return indexPtr;
			}

			// 使用已注册过的实例
			// return ( IntPtr )gcHandleDictionary[ index ];
			return typedReferenceMap.Reverse[ index ];
		}


		private void AllocDataTypeStruct ( string structName, out mrb_data_type data_type_t, out IntPtr data_type_ptr ) {
			data_type_t = new mrb_data_type {
				struct_name = structName,
				dfree = DFree
			};
			
			data_type_ptr = Marshal.AllocHGlobal ( Marshal.SizeOf ( data_type_t ) );
			Marshal.StructureToPtr ( data_type_t, data_type_ptr, false );
		}


		public void AffirmDataTypeStruct< T > ( out mrb_data_type data_type_t, out IntPtr data_type_ptr ) {
			data_type_t = default;
			data_type_ptr = IntPtr.Zero;
			
			Type type = typeof ( T );
			
			// Use exist
			if ( registedDataTypeDictionary.ContainsKey ( type ) ) {
				data_type_t = registedDataTypeDictionary[ type ];
				
				if ( registedDataTypePtrDictionary.ContainsKey ( type ) ) {
					data_type_ptr = registedDataTypePtrDictionary[ type ];
				}
				return;
			}
			
			// Create new
			AllocDataTypeStruct ( type.FullName, out data_type_t, out data_type_ptr );
			
			registedDataTypeDictionary.Add ( type, data_type_t );
			registedDataTypePtrDictionary.Add ( type, data_type_ptr );
		}


		/// <summary>
		/// Add CSharp instance <=> Ruby instance map
		/// </summary>
		public void AddCRInstanceMap ( object obj, R_VAL val ) {
			dataValueMap.Add ( obj, val );
		}


		/// <summary>
		/// 查找一个C#已绑定类实例是否有对应的ruby实例
		/// </summary>
		/// <param name="obj">已绑定类实例</param>
		public bool HasC2RInstanceMap ( object obj ) {
			return dataValueMap.Forward.Contains ( obj );
		}


		/// <summary>
		/// 查找一个ruby实例是否有对应的C#已绑定类实例
		/// </summary>
		/// <param name="val">ruby类实例</param>
		public bool HasR2CInstanceMap ( R_VAL val ) {
			return dataValueMap.Reverse.Contains ( val );
		}
		
		
		/// <summary>
		/// 使用此方法管理Ruby端GC释放的C#绑定类实例
		/// </summary>
		/// <param name="data">data应该为绑定ruby类实例化时创建的GCHandle</param>
		public void DFree ( IntPtr mrb, IntPtr data ) {

			if ( data == IntPtr.Zero ) {
				return;
			}

			// GCHandle handle = GCHandle.FromIntPtr ( data );
			//
			// if ( handle.Target == null ) {
			// 	Console.WriteLine ( "[DEBUG] Warning! DFree release a null csharp object", handle );
			// }

			int index;
			object obj = null;
			if ( typedReferenceMap.Forward.TryGetValue ( data, out index ) ) {
				obj = translator.GetObject ( index );
			}
			
			if ( obj != null && translator.Getudata ( obj, out index ) ) {
				
				#if TRACING_COMMUNICATION
				// Console.WriteLine ( $"[DEBUG] DFree: {gcHandleDictionary[ index ].Target} #{index}" );
				Console.WriteLine ( $"[DEBUG] DFree: {obj} #{index} 0x{data.ToString("X")}" );
				#endif
				
				translator.RemoveObject ( index );
				typedReferenceMap.Remove ( data );
				// gcHandleDictionary[ index ].Free ();
				Marshal.FreeHGlobal ( data );
				return;
			}

			if ( obj != null ) {
				Console.WriteLine ( $"[DEBUG] Error! DFree catch a unmanaged object: {obj} #{index}" );
				return;
			}
			
			Console.WriteLine ( $"[DEBUG] Error! DFree catch a unmanaged object: {data.ToString("X")}" );
		}
		
		
		public static object[] RubyFunctionParamsToObjects ( IntPtr mrb, IntPtr data_type_ptr ) {
			R_VAL[] value = RubyDLL.GetFunctionArgs ( mrb );
			object[] ret = new object[ value.Length ];
			for ( int i = 0; i < ret.Length; i++ ) {
				ref R_VAL val = ref value[ i ];
				if ( !R_VAL.IsData ( val ) ) {
					ret[ i ] = ValueToObject ( mrb, val );
				}
				else {
					IntPtr ptr = RubyDLL.mrb_data_get_ptr ( mrb, val, data_type_ptr );
					ret[ i ] =  ( ( GCHandle )ptr ).Target;
				}
			}
			return ret;
		}


		/// <summary>
		/// 用于从ruby绑定类实例=>C#绑定类实例
		/// </summary>
		/// <param name="state"></param>
		/// <param name="value"></param>
		/// <param name="data_type_ptr"></param>
		public static object ValueToRefObject ( RubyState state, R_VAL value, IntPtr data_type_ptr ) {
			if ( R_VAL.IsNil ( value ) ) {
				return null;
			}

			// 类似从C#缓存中直接取值
			if ( state.HasR2CInstanceMap ( value ) ) {
				#if TRACING_COMMUNICATION
				Console.WriteLine ( $"[DEBUG] 使用已绑定C#实例:{value} => {state.dataValueMap.Reverse[ value ]}" );
				#endif
				return state.dataValueMap.Reverse[ value ];
			}
			
			IntPtr ptr = RubyDLL.mrb_data_get_ptr ( state, value, data_type_ptr );
			// return ( ( GCHandle )ptr ).Target;
			return state.translator.GetObject ( state.typedReferenceMap.Forward[ ptr ] );
		}
		
		
		public static object ValueToObject ( IntPtr mrb, R_VAL value ) {
			
			if ( R_VAL.IsNil ( value ) ) {
				return null;
			}

			switch ( value.tt ) {
				case rb_vtype.RUBY_T_FALSE:
					return false;
				case rb_vtype.RUBY_T_TRUE:
					return true;
				case rb_vtype.RUBY_T_INTEGER:
					return ( int )RubyDLL.mrb_fixnum ( value );
				case rb_vtype.RUBY_T_SYMBOL:
					return RubyDLL.mrb_symbol ( value );
				case rb_vtype.RUBY_T_UNDEF:
					return null;
				case rb_vtype.RUBY_T_FLOAT:
					return ( float )RubyDLL.mrb_float ( value );
				case rb_vtype.RUBY_T_STRING:
					return value.ToString ( mrb );
				case rb_vtype.RUBY_T_CPTR:
					return RubyDLL.mrb_cptr ( value );
				case rb_vtype.RUBY_T_OBJECT:
				case rb_vtype.RUBY_T_CLASS:
				case rb_vtype.RUBY_T_MODULE:
				case rb_vtype.RUBY_T_ICLASS:
				case rb_vtype.RUBY_T_SCLASS:
				case rb_vtype.RUBY_T_PROC:
				case rb_vtype.RUBY_T_RANGE:
				case rb_vtype.RUBY_T_EXCEPTION:
				case rb_vtype.RUBY_T_ENV:
				case rb_vtype.RUBY_T_FIBER:
				case rb_vtype.RUBY_T_ISTRUCT:
				case rb_vtype.RUBY_T_BREAK:
				case rb_vtype.RUBY_T_MAXDEFINE:
					return RubyDLL.mrb_ptr ( value );
				case rb_vtype.RUBY_T_ARRAY:
				case rb_vtype.RUBY_T_HASH:
					// TODO:
					return RubyDLL.mrb_ptr ( value );
				case rb_vtype.RUBY_T_DATA: 
					throw new NotSupportedException ( "ValueToObject not support Data Type." );
					// return IntPtrToObject ( mrb_data_get_ptr ( mrb, value, RubyState.DATA_TYPE_PTR ) );
				default:
					return null;
			}
		}
		
		
		public static object ValueToObjectOfType ( RubyState mrb, R_VAL value, IntPtr dataTypePtr, Type desiredType, object defaultValue, bool isOptional ) {
			
			if ( desiredType.IsByRef ) {
				desiredType = desiredType.GetElementType ();
			}

			if ( desiredType == typeof ( R_VAL ) ) {
				return value;
			}

			if ( desiredType == typeof ( object ) || ( desiredType.IsClass && desiredType != typeof ( string ) ) ) {
				return RubyState.ValueToRefObject ( mrb, value, dataTypePtr );
			}

			Type nt = Nullable.GetUnderlyingType ( desiredType );
			Type nullableType = null;

			if ( nt != null ) {
				nullableType = desiredType;
				desiredType = nt;
			}

			if ( R_VAL.IsNil ( value ) ) {
				if ( isOptional ) {
					return defaultValue;
				}
				if ( !desiredType.IsValueType || nullableType != null ) {
					return null;
				}
			}

			switch ( value.tt ) {
				case rb_vtype.RUBY_T_FALSE:
					if ( desiredType == typeof ( bool ) ) {
						return false;
					}
					break;
				case rb_vtype.RUBY_T_TRUE:
					if ( desiredType == typeof ( bool ) ) {
						return true;
					}
					break;
				case rb_vtype.RUBY_T_INTEGER:
					if ( desiredType.IsEnum ) {
						Type underType = Enum.GetUnderlyingType ( desiredType );
						return IntToType ( underType, RubyDLL.mrb_fixnum ( value ) );
					}
					return RubyDLL.mrb_fixnum ( value );
				case rb_vtype.RUBY_T_SYMBOL:
					return RubyDLL.mrb_symbol ( value );
				case rb_vtype.RUBY_T_UNDEF:
					return null;
				case rb_vtype.RUBY_T_FLOAT:
					return ( float )RubyDLL.mrb_float ( value );
				case rb_vtype.RUBY_T_STRING:
					return value.ToString ( mrb );
				case rb_vtype.RUBY_T_CPTR:
					return RubyDLL.mrb_cptr ( value );
				case rb_vtype.RUBY_T_OBJECT:
				case rb_vtype.RUBY_T_CLASS:
				case rb_vtype.RUBY_T_MODULE:
				case rb_vtype.RUBY_T_ICLASS:
				case rb_vtype.RUBY_T_SCLASS:
				case rb_vtype.RUBY_T_PROC:
				case rb_vtype.RUBY_T_RANGE:
				case rb_vtype.RUBY_T_EXCEPTION:
				case rb_vtype.RUBY_T_ENV:
				case rb_vtype.RUBY_T_FIBER:
				case rb_vtype.RUBY_T_ISTRUCT:
				case rb_vtype.RUBY_T_BREAK:
				case rb_vtype.RUBY_T_MAXDEFINE:
					return RubyDLL.mrb_ptr ( value );
				case rb_vtype.RUBY_T_ARRAY:
				case rb_vtype.RUBY_T_HASH:
					// TODO:
					return RubyDLL.mrb_ptr ( value );
				case rb_vtype.RUBY_T_DATA:
					return RubyState.ValueToRefObject ( mrb, value, dataTypePtr );
				default:
					return null;
			}

			return null;
		}
		
		
		public static T ValueToDataObject<T> ( RubyState state, R_VAL value, IntPtr data_type ) /*where T : class*/ {
			if ( R_VAL.IsNil ( value ) ) {
				return default;
			}

			int index;
			if ( state.typedReferenceMap.Forward.TryGetValue ( RubyDLL.mrb_data_get_ptr ( state, value, data_type ), out index ) ) {
				return ( T )state.translator.GetObject ( index );
			}

			return default;
		}
		
		
		/// <summary>
		/// 用于从C#实例转ruby实例
		/// </summary>
		/// <param name="state"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static R_VAL ObjectToValue ( RubyState state, object obj ) {

			if ( obj == null ) {
				return R_VAL.NIL;
			}

			Type type = obj.GetType ();

			if ( type == typeof ( R_VAL ) ) {
				return ( R_VAL )obj;
			}
			
			if ( type.IsValueType ) {
				if ( type == typeof ( int ) || type == typeof ( short ) || type == typeof ( long ) ||
				     type == typeof ( uint ) || type == typeof ( ushort ) || type == typeof ( ulong ) ||
				     type == typeof ( byte ) || type == typeof ( sbyte ) ||
				     type.IsEnum ) {
					return RubyDLL.mrb_fixnum_value ( state, Convert.ToInt32 ( obj ) );
				}
				else if ( type == typeof ( int? ) || type == typeof ( short? ) || type == typeof ( long? ) ||
				          type == typeof ( uint? ) || type == typeof ( ushort? ) || type == typeof ( ulong? ) ||
				          type == typeof ( byte? ) || type == typeof ( sbyte? ) ) {
					return RubyDLL.mrb_fixnum_value ( state, ( ( int? )obj ).HasValue ? ( ( int? )obj ).Value : 0 );
				}
				else if ( type == typeof ( float ) || type == typeof ( double ) ) {
					return RubyDLL.mrb_float_value ( state, Convert.ToDouble ( obj ) );
				}
				else if ( type == typeof ( float? ) || type == typeof ( double? ) ) {
					return RubyDLL.mrb_float_value ( state, ( ( double? )obj ).HasValue ? ( ( double? )obj ).Value : 0f );
				}
				else if ( type == typeof ( bool ) ) {
					return R_VAL.Create ( ( bool )obj );
				}
				else if ( type == typeof ( bool? ) ) {
					return R_VAL.Create ( ( ( bool? )obj ).HasValue ? ( ( bool? )obj ).Value : false );
				}
				else if ( type.IsArray ) {
					// csValueToValue = $"RubyDLL.DataObjectToValue ( mrb, {GetWrapperClassName ( typeof ( System.Array ) )}.@class, {GetWrapperClassName ( typeof ( System.Array ) )}.data_type_ptr, {retVarName} )";
					RegistedTypeInfo info = state.GetRegistedTypeInfo ( type );
					return RubyState.DataObjectToValue ( state, info.@class, info.dataTypePtr, obj );
				}
				else if ( System.Nullable.GetUnderlyingType ( type ) != null ) {
					// csValueToValue = $"RubyDLL.DataObjectToValue ( mrb, {GetWrapperClassName ( System.Nullable.GetUnderlyingType ( type ) )}.@class, {GetWrapperClassName ( System.Nullable.GetUnderlyingType ( type ) )}.data_type_ptr, {retVarName} )";
					return RubyState.DataObjectToValue ( state, state.SystemObjectRClass, RubyState.ObjectDataTypePtr, obj );
				}
				else {
					// csValueToValue = $"RubyDLL.DataObjectToValue ( mrb, {GetWrapperClassName ( type )}.@class, {GetWrapperClassName ( type )}.data_type_ptr, {retVarName} )";
					if ( state.HasC2RInstanceMap ( obj ) ) {
						#if TRACING_COMMUNICATION
						Console.WriteLine ( $"[DEBUG] 使用已绑定ruby实例:{obj} => {state.dataValueMap.Forward[ obj ]}" );
						#endif
						return state.dataValueMap.Forward[ obj ];
					}
					RegistedTypeInfo info = state.GetRegistedTypeInfo ( type );
					R_VAL ret = RubyState.DataObjectToValue ( state, info.@class, info.dataTypePtr, obj );
					state.AddCRInstanceMap ( obj, ret );
					return ret;
				}
			}
			else {
				if ( type == typeof ( string ) || type == typeof ( System.String ) ) {
					return R_VAL.Create ( state, ( string )obj );
				}
				else if ( type.IsArray ) {
					// csValueToValue = $"RubyDLL.DataObjectToValue ( mrb, {GetWrapperClassName ( typeof ( System.Array ) )}.@class, {GetWrapperClassName ( typeof ( System.Array ) )}.data_type_ptr, {retVarName} )";
					RegistedTypeInfo info = state.GetRegistedTypeInfo ( type );
					return RubyState.DataObjectToValue ( state, info.@class, info.dataTypePtr, obj );
				}
				else if ( type == typeof ( System.Type ) ) {
					// csValueToValue = $"RubyDLL.DataObjectToValue ( mrb, {GetWrapperClassName ( typeof ( System.Object ) )}.@class, {GetWrapperClassName ( typeof ( System.Object ) )}.data_type_ptr, {retVarName} )";
					RegistedTypeInfo info = state.GetRegistedTypeInfo ( type );
					return RubyState.DataObjectToValue ( state, info.@class, info.dataTypePtr, obj );
				}
				else {
					// csValueToValue = $"RubyDLL.DataObjectToValue ( mrb, {GetWrapperClassName ( type )}.@class, {GetWrapperClassName ( type )}.data_type_ptr, {retVarName} )";
					if ( state.HasC2RInstanceMap ( obj ) ) {
						#if TRACING_COMMUNICATION
						Console.WriteLine ( $"[DEBUG] 使用已绑定ruby实例:{obj} => {state.dataValueMap.Forward[ obj ]}" );
						#endif
						return state.dataValueMap.Forward[ obj ];
					}
					RegistedTypeInfo info = state.GetRegistedTypeInfo ( type );
					R_VAL ret = RubyState.DataObjectToValue ( state, info.@class, info.dataTypePtr, obj );
					state.AddCRInstanceMap ( obj, ret );
					return ret;
				}
			}
		}
		
		
		/// <summary>
		/// 向ruby中传递C#实例但没有已有ruby实例时, 创建对应的ruby实例
		/// </summary>
		/// <param name="klass">RClass</param>
		/// <param name="data_type_ptr"></param>
		/// <param name="obj"></param>
		public static R_VAL DataObjectToValue ( RubyState state, IntPtr klass, IntPtr data_type_ptr, object obj ) {
			#if !TRACING_COMMUNICATION
			Console.WriteLine ( $"[DEBUG] 创建绑定ruby实例:{obj}" );
			#endif
			return RubyDLL.mrb_data_wrap_struct_obj ( state, klass, data_type_ptr, state.PushRegistedCSharpObject ( obj ) );
		}
		
		
		// public static R_VAL DataObjectToValue ( RubyState state, IntPtr klass, IntPtr data_type, object obj ) {
		// 	int index;
		// 	if ( state.translator.Getudata ( obj, out index ) ) {
		// 		return RubyDLL.mrb_data_wrap_struct_obj ( state, klass, data_type, state.typedReferenceDictionaryReverse[ index ] );
		// 	}
		// 	
		// 	return R_VAL.NIL;
		// }
		
		
		internal static object IntToType ( Type type, int i ) {
			type = Nullable.GetUnderlyingType ( type ) ?? type;

			if ( type == typeof ( double ) ) return i;
			if ( type == typeof ( sbyte ) ) return ( sbyte )i;
			if ( type == typeof ( byte ) ) return ( byte )i;
			if ( type == typeof ( short ) ) return ( short )i;
			if ( type == typeof ( ushort ) ) return ( ushort )i;
			if ( type == typeof ( int ) ) return i;
			if ( type == typeof ( uint ) ) return ( uint )i;
			if ( type == typeof ( long ) ) return ( long )i;
			if ( type == typeof ( ulong ) ) return ( ulong )i;
			if ( type == typeof ( float ) ) return ( float )i;
			if ( type == typeof ( decimal ) ) return ( decimal )i;
			return i;
		}
		
		#endregion

        
        void IDisposable.Dispose () {
#if MRUBY
            RubyDLL.mrb_close ( rb_state );
#else
            RubyDLL.ruby_finalize ();
#endif
			translator.Dispose ();
			translator = null;

			// free all alloced data_type_ptr
			foreach ( var kv in registedDataTypePtrDictionary ) {
				Marshal.FreeHGlobal ( kv.Value );
			}
			
			// gcHandleDictionary.Clear ();
			typedReferenceMap.Clear ();
			dataValueMap.Clear ();
			registedDataTypeDictionary.Clear ();
			registedDataTypePtrDictionary.Clear ();
			
			rb_state = IntPtr.Zero;
			
			Console.WriteLine ( "[DEBUG] RubyState::Disposed" );
		}

		
#if MRUBY
        private static R_VAL _doFile ( IntPtr mrb, R_VAL self ) {
            R_VAL[] args = RubyDLL.GetFunctionArgs ( mrb );

            if ( args.Length != 1 ) {
                throw new ArgumentException ( $"RubyState.DoFile parameter count mismatch: require 1 but got {args.Length}." );
            }

            if ( !R_VAL.IsString ( args[ 0 ] ) ) {
                throw new ArgumentException ( $"RubyState.DoFile parameter type mismatch: require String but got {RubyDLL.r_type ( args[ 0 ] )}." );
            }

            return RubyState.DoFile ( mrb, args[ 0 ].ToString ( mrb ) );
        }
#endif
		
		
		private static bool IsValueType ( object obj ) {
			Type t = obj.GetType ();
			return !t.IsEnum && t.IsValueType;
		}
		
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static implicit operator IntPtr ( RubyState state ) {
            return state.rb_state;
        }

        
        #region All Wrapper Class Used Data Type
#if MRUBY
		private static mrb_data_type _ObjectDataType;
		private static IntPtr _ObjectDataTypePtr;
		private static mrb_data_type _EnumDataType;
		private static IntPtr _EnumDataTypePtr;
		
        /// <summary>
        /// All ByRef Type Use This Data Type
        /// </summary>
        public static mrb_data_type ObjectDataType => _ObjectDataType;
		public static IntPtr ObjectDataTypePtr => _ObjectDataTypePtr;

		public static mrb_data_type EnumDataType => _EnumDataType;
		public static IntPtr EnumDataTypePtr => _EnumDataTypePtr;
#endif
        #endregion
    }


	public class RegistedTypeInfo {
		public IntPtr @class;
		public mrb_data_type dataType;
		public IntPtr dataTypePtr;
		public RubyDLL.RubyCSFunction ctorFunction;
		public Dictionary< string, RubyDLL.RubyCSFunction > instanceFunction;
		public Dictionary< string, RubyDLL.RubyCSFunction > classFunction;
	}
}
