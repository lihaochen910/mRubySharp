namespace RubySharp {
    
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    
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
            
            InitSystemObjectWrapper ();
            
#if MRUBY
            // load file
            DefineMethod ( "dofile", _doFile, rb_args.REQ ( 1 ) );
#endif
        }

        internal void InitSystemObjectWrapper () {
#if MRUBY
            SystemObjectRClass = UserDataUtility.DefineCSharpClass ( this, typeof ( System.Object ) );
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
            string filename      = Path.GetFileName ( path );
            int    arena        = RubyDLL.mrb_gc_arena_save ( rb_state );
            IntPtr mrbc_context = RubyDLL.mrbc_context_new ( rb_state );
            RubyDLL.mrbc_filename ( this, mrbc_context, filename );
            var ret = RubyDLL.mrb_load_string_cxt ( rb_state, RubyDLL.ToCBytes ( File.ReadAllText ( path ) ), mrbc_context );
            RubyDLL.mrbc_context_free ( rb_state, mrbc_context );
            if ( RubyDLL.mrb_has_exc ( rb_state ) ) {
                Console.WriteLine ( GetExceptionBackTrace () );
            }
            RubyDLL.mrb_gc_arena_restore ( rb_state, arena );
            return ret;
#else
            int status;
            RubyDLL.rb_load_protect ( R_VAL.Create ( path ), 0, out status );
            return status == 0 ? R_VAL.TRUE : R_VAL.FALSE;
#endif
        }
        
        public static R_VAL DoFile ( IntPtr mrb_state, string path ) {
            
            if ( !File.Exists ( path ) ) {
                return R_VAL.NIL;
            }
            
#if MRUBY
            string filename = Path.GetFileName ( path );
            int arena = RubyDLL.mrb_gc_arena_save ( mrb_state );
            IntPtr mrbc_context = RubyDLL.mrbc_context_new ( mrb_state );
            RubyDLL.mrbc_filename ( mrb_state, mrbc_context, filename );
            var ret = RubyDLL.mrb_load_string_cxt ( mrb_state, RubyDLL.ToCBytes ( File.ReadAllText ( path ) ), mrbc_context );
            RubyDLL.mrbc_context_free ( mrb_state, mrbc_context );
            if ( RubyDLL.mrb_has_exc ( mrb_state ) ) {
                Console.WriteLine ( GetExceptionBackTrace ( mrb_state ) );
            }
            RubyDLL.mrb_gc_arena_restore ( mrb_state, arena );
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
        public static string GetExceptionBackTrace ( IntPtr mrb_state ) {
#else
		public static string GetExceptionBackTrace () {
#endif
            
            System.Text.StringBuilder builder = new System.Text.StringBuilder ();
            
#if MRUBY
            R_VAL exc = RubyDLL.mrb_get_exc_value ( mrb_state );
            R_VAL backtrace = RubyDLL.r_exc_backtrace ( mrb_state, exc );
            
            builder.AppendLine ( RubyDLL.r_funcall ( mrb_state, exc, "inspect", 0 ).ToString ( mrb_state ) );

            builder.AppendLine ( "trace:" );
            for ( var i = 0; i < RubyDLL.r_funcall ( mrb_state, backtrace, "size", 0 ); ++i ) {
                R_VAL v = RubyDLL.r_ary_ref ( mrb_state, backtrace, i );
                builder.AppendLine ( $"  [{i}] {v.ToString ( mrb_state )}" );
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

            var function = CallbackFunction.FromDelegate ( @delegate );
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

        
        void IDisposable.Dispose () {
#if MRUBY
            RubyDLL.mrb_close ( rb_state );
#else
            RubyDLL.ruby_finalize ();
#endif
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
		
		private static void _dFree ( IntPtr mrb, IntPtr data ) {
            Console.WriteLine ( $"dfree: {data}" );
        }
#endif
		
        public static implicit operator IntPtr ( RubyState state ) {
            return state.rb_state;
        }

        
        #region All Wrapper Class Used Data Type
#if MRUBY
        /// <summary>
        /// All ByRef Type Use This Data Type
        /// </summary>
        public static readonly mrb_data_type DATA_TYPE = new mrb_data_type {
            struct_name = "Object",
            dfree       = _dFree
        };

        private static bool _DATA_TYPE_PTR_CREATED;
        private static IntPtr _DATA_TYPE_PTR;

        public static IntPtr DATA_TYPE_PTR {
            get {
                if ( !_DATA_TYPE_PTR_CREATED ) {
                    int nSizeOfPerson = Marshal.SizeOf ( DATA_TYPE );
                    _DATA_TYPE_PTR = Marshal.AllocHGlobal ( nSizeOfPerson );
                    Marshal.StructureToPtr ( DATA_TYPE, _DATA_TYPE_PTR, false );
                    _DATA_TYPE_PTR_CREATED = true;
                }

                return _DATA_TYPE_PTR;
            }
        }
#endif
        #endregion
    }
}
