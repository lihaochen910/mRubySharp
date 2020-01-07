using System;
using System.IO;
using System.Collections.Generic;


namespace CandyFramework.mRuby {
    
    public class mRubyState : IDisposable {
        
        public IntPtr mrb_state { get; private set; }

        public mrb_value mrb_object_class  { get; private set; }
        public mrb_value mrb_kernel_module { get; private set; }

        /// <summary>
        /// 保存当前VM从C#传入mruby的方法指针，防止C#方法指针进入mruby后被C#端GC掉
        /// </summary>
        public List< Delegate > MethodDelegates { get; private set; } = new List< Delegate > ();
        
        /// <summary>
        /// search path
        /// </summary>
        private IList<string> requiredSearchPaths = new List<string> ();

        public mRubyState () {
            mrb_state = mRubyDLL.mrb_open ();

            mrb_object_class  = DoString ( "Object" );
            mrb_kernel_module = DoString ( "Kernel" );
            
            // load file
            DefineMethod ( "dofile", _doFile, mrb_args.REQ ( 1 ) );
        }

        public mrb_value DoString ( string str ) {
            int    arena        = mRubyDLL.mrb_gc_arena_save ( mrb_state );
            IntPtr mrbc_context = mRubyDLL.mrbc_context_new ( mrb_state );
            mRubyDLL.mrbc_filename ( this, mrbc_context, "*interactive*" );
            var ret = mRubyDLL.mrb_load_string_cxt ( mrb_state, mRubyDLL.ToCBytes ( str ), mrbc_context );
            mRubyDLL.mrbc_context_free ( mrb_state, mrbc_context );
            
            if ( mRubyDLL.mrb_has_exc ( mrb_state ) != 0 ) {
                Console.WriteLine ( GetExceptionBackTrace () );
            }
            mRubyDLL.mrb_gc_arena_restore ( mrb_state, arena );
            return ret;
        }
        
        public mrb_value DoFile ( string path ) {
            
            if ( !File.Exists ( path ) ) {
                return mrb_value.NIL;
            }
            
            string filename      = Path.GetFileName ( path );
            int    arena        = mRubyDLL.mrb_gc_arena_save ( mrb_state );
            IntPtr mrbc_context = mRubyDLL.mrbc_context_new ( mrb_state );
            mRubyDLL.mrbc_filename ( this, mrbc_context, filename );
            var ret = mRubyDLL.mrb_load_string_cxt ( mrb_state, mRubyDLL.ToCBytes ( File.ReadAllText ( path ) ), mrbc_context );
            mRubyDLL.mrbc_context_free ( mrb_state, mrbc_context );
            if ( mRubyDLL.mrb_has_exc ( mrb_state ) != 0 ) {
                Console.WriteLine ( GetExceptionBackTrace () );
            }
            mRubyDLL.mrb_gc_arena_restore ( mrb_state, arena );
            return ret;
        }
        
        public static mrb_value DoFile ( IntPtr mrb_state, string path ) {
            
            if ( !File.Exists ( path ) ) {
                return mrb_value.NIL;
            }
            
            string filename     = Path.GetFileName ( path );
            int    arena        = mRubyDLL.mrb_gc_arena_save ( mrb_state );
            IntPtr mrbc_context = mRubyDLL.mrbc_context_new ( mrb_state );
            mRubyDLL.mrbc_filename ( mrb_state, mrbc_context, filename );
            var ret = mRubyDLL.mrb_load_string_cxt ( mrb_state, mRubyDLL.ToCBytes ( File.ReadAllText ( path ) ), mrbc_context );
            mRubyDLL.mrbc_context_free ( mrb_state, mrbc_context );
            if ( mRubyDLL.mrb_has_exc ( mrb_state ) != 0 ) {
                Console.WriteLine ( GetExceptionBackTrace ( mrb_state ) );
            }
            mRubyDLL.mrb_gc_arena_restore ( mrb_state, arena );
            return ret;
        }

        public mrb_value DoByteCode ( byte[] bytecode ) {
            return mRubyDLL.mrb_load_irep ( mrb_state, bytecode );
        }
        
        public bool RequirePath ( string path ) {
            if ( Directory.Exists ( path ) ) {
                requiredSearchPaths.Insert ( 0, Path.GetDirectoryName ( path ) );
                return true;
            }
            return false;
        }

        public mrb_value Call ( string funcName ) {
            return mRubyDLL.mrb_funcall ( mrb_state, mRubyDLL.mrb_top_self ( mrb_state ), funcName, 0 );
        }

        public mrb_value Call ( string funcName, mrb_value arg ) {
            return mRubyDLL.mrb_funcall_1 ( mrb_state, mRubyDLL.mrb_top_self ( mrb_state ), funcName, 1, arg );
        }

        /// <summary>
        /// 获取当前异常调用堆栈信息
        /// https://qiita.com/seanchas_t/items/ca293f9dd4454cd6cb6d
        /// </summary>
        public string GetExceptionBackTrace () {
            
            System.Text.StringBuilder builder = new System.Text.StringBuilder ();
            
            mrb_value exc = mRubyDLL.mrb_get_exc_value ( mrb_state );
            mrb_value backtrace = mRubyDLL.mrb_exc_backtrace ( mrb_state, exc );
            
            builder.AppendLine ( "trace:" );
            for ( var i = 0; i < mRubyDLL.mrb_funcall ( mrb_state, backtrace, "size", 0 ); ++i ) {
                mrb_value v = mRubyDLL.mrb_ary_ref ( mrb_state, backtrace, i );
                builder.AppendLine ( $"  [{i}] {v.ToString ( mrb_state )}" );
            }
            
            builder.AppendLine ( mRubyDLL.mrb_funcall ( mrb_state, exc, "inspect", 0 ).ToString ( mrb_state ) );

            return builder.ToString ();
            
            // return mRubyDLL.mrb_inspect ( mrb_state, mRubyDLL.mrb_get_backtrace ( mrb_state ) ).ToString ( mrb_state );
            // return mRubyDLL.mrb_funcall ( mrb_state, mRubyDLL.mrb_exc_backtrace ( mrb_state, mRubyDLL.mrb_get_exc_value ( mrb_state ) ), "to_s", 0 ).ToString ( mrb_state );
            // return mRubyDLL.mrb_inspect ( mrb_state, mRubyDLL.mrb_get_exc_value ( mrb_state ) ).ToString ( mrb_state );
            // return mRubyDLL.mrb_inspect ( mrb_state, mRubyDLL.mrb_exc_backtrace ( mrb_state, mRubyDLL.mrb_get_exc_value ( mrb_state ) ) ).ToString ( mrb_state );
        }
        
        public static string GetExceptionBackTrace ( IntPtr mrb_state ) {
            
            System.Text.StringBuilder builder = new System.Text.StringBuilder ();
            
            mrb_value exc       = mRubyDLL.mrb_get_exc_value ( mrb_state );
            mrb_value backtrace = mRubyDLL.mrb_exc_backtrace ( mrb_state, exc );
            
            builder.AppendLine ( "trace:" );
            for ( var i = 0; i < mRubyDLL.mrb_funcall ( mrb_state, backtrace, "size", 0 ); ++i ) {
                mrb_value v = mRubyDLL.mrb_ary_ref ( mrb_state, backtrace, i );
                builder.AppendLine ( $"  [{i}] {v.ToString ( mrb_state )}" );
            }
            
            builder.AppendLine ( mRubyDLL.mrb_funcall ( mrb_state, exc, "inspect", 0 ).ToString ( mrb_state ) );

            return builder.ToString ();
        }
        
        public string GetCurrentBackTrace () {
            
            System.Text.StringBuilder builder = new System.Text.StringBuilder ();
            
            mrb_value backtrace = mRubyDLL.mrb_get_backtrace ( mrb_state );
            
            builder.AppendLine ( "trace:" );
            for ( var i = 0; i < mRubyDLL.mrb_funcall ( mrb_state, backtrace, "size", 0 ); ++i ) {
                mrb_value v = mRubyDLL.mrb_ary_ref ( mrb_state, backtrace, i );
                builder.AppendLine ( $"  [{i}] {v.ToString ( mrb_state )}" );
            }
            
            return builder.ToString ();
            
            // return mRubyDLL.mrb_inspect ( mrb_state, mRubyDLL.mrb_get_backtrace ( mrb_state ) ).ToString ( mrb_state );
            // return mRubyDLL.mrb_funcall ( mrb_state, mRubyDLL.mrb_exc_backtrace ( mrb_state, mRubyDLL.mrb_get_exc_value ( mrb_state ) ), "to_s", 0 ).ToString ( mrb_state );
            // return mRubyDLL.mrb_inspect ( mrb_state, mRubyDLL.mrb_get_exc_value ( mrb_state ) ).ToString ( mrb_state );
            // return mRubyDLL.mrb_inspect ( mrb_state, mRubyDLL.mrb_exc_backtrace ( mrb_state, mRubyDLL.mrb_get_exc_value ( mrb_state ) ) ).ToString ( mrb_state );
        }

        /// <summary>
        /// 在mruby全局中定义方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="receiver"></param>
        /// <param name="aspec"></param>
        public void DefineMethod ( string name, mRubyDLL.MRubyCSFunction receiver, mrb_args aspec ) {
            // 防止被C#端GC
            MethodDelegates.Add ( receiver );

            mRubyDLL.mrb_define_module_function ( mrb_state, mrb_kernel_module, name, receiver, aspec );
        }

        void IDisposable.Dispose () {
            mRubyDLL.mrb_close ( mrb_state );
        }


        private static mrb_value _doFile ( IntPtr mrb, mrb_value self ) {
            mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );

            if ( args.Length != 1 ) {
                throw new ArgumentException ( $"mRubyState.DoFile parameter count mismatch: require 1 but got {args.Length}." );
            }

            if ( !mrb_value.IsString ( args[ 0 ] ) ) {
                throw new ArgumentException ( $"mRubyState.DoFile parameter type mismatch: require String but got {mRubyDLL.mrb_type ( args[ 0 ] )}." );
            }

            return mRubyState.DoFile ( mrb, args[ 0 ].ToString ( mrb ) );
        }
        

        static public implicit operator IntPtr ( mRubyState state ) {
            return state.mrb_state;
        }
    }
}
