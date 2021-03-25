using System;
using System.IO;
using System.Runtime.InteropServices;


namespace RubySharp {
    
    class Program {


		static RubyState state;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main () {
            
            // https://github.com/FNA-XNA/FNA/wiki/4:-FNA-and-Windows-API#64-bit-support
            if ( Environment.OSVersion.Platform == PlatformID.Unix ) {
                string path = Environment.GetEnvironmentVariable ( "PATH" );
                Environment.SetEnvironmentVariable ( "PATH", Path.Combine ( AppDomain.CurrentDomain.BaseDirectory, "lib", "osx" ) + ";" + path );
			
                // string libPath = Environment.GetEnvironmentVariable ( "LD_LIBRARY_PATH" );
                // Environment.SetEnvironmentVariable ( "LD_LIBRARY_PATH", Path.Combine ( AppDomain.CurrentDomain.BaseDirectory, "lib", "osx" ) + ";" + libPath );
            }
			
			Console.WriteLine ( (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB" );
			
            state = new RubyState ();
			
			Console.WriteLine ( (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB" );

			R_VAL v1 = R_VAL.Create ( state, 2333 );
#if MRUBY
            R_VAL v2 = R_VAL.Create ( state, 3.1415926535897932f );
#else
            R_VAL v2 = R_VAL.Create ( 3.1415926535897932d );
#endif
            R_VAL v3 = R_VAL.FALSE;
            R_VAL v4 = R_VAL.TRUE;
            R_VAL v5 = R_VAL.NIL;

#if MRUBY
            Console.WriteLine ( v1.ToString ( state ) );
            Console.WriteLine ( v2.ToString ( state ) );
            Console.WriteLine ( v3.ToString ( state ) );
            Console.WriteLine ( v4.ToString ( state ) );
            Console.WriteLine ( v5.ToString ( state ) );
			
			state.DefineMethod ( "WriteLine", WriteLine, rb_args.ANY () );
            state.DefineMethod ( "show_backtrace", ShowBackTrace, rb_args.NONE () );
            state.DefineMethod ( "WriteLineNormal", new System.Action< string, int, float, bool, object > ( TestDelegate ), rb_args.ANY () );
			
			mRubyClass klass = new mRubyClass ( state, "CSharpClass" );

            klass.DefineMethod ( "write", WriteLine, rb_args.ANY () );
			
			// WrapperUtility.GenCSharpClass ( typeof ( System.Object ) );
		    // WrapperUtility.GenCSharpClass ( typeof ( System.Array ) );
		    // WrapperUtility.GenCSharpClass ( typeof ( System.TimeSpan ) );
            // WrapperUtility.GenByAssembly ( typeof ( Microsoft.Xna.Framework.Game ).Assembly );
            // WrapperUtility.GenUnityEngineCommon ();
            // WrapperUtility.GenCSharpClass ( typeof ( CustomClass ) );
            // WrapperUtility.GenCSharpClass ( typeof ( CustomEnum ) );

            UserDataUtility.RegisterType< CustomClass > ( state );
            UserDataUtility.RegisterType< CustomEnum > ( state );

            // CustomClass_Wrapper.__Register__ ( state );
            // CustomEnum_Wrapper.__Register__ ( state );

            // state.DoFile ( "main.rb" );

#else
			Console.WriteLine ( v1.ToString () );
			Console.WriteLine ( v2.ToString () );
			Console.WriteLine ( v3.ToString () );
			Console.WriteLine ( v4.ToString () );
			Console.WriteLine ( v5.ToString () );
			
			state.DefineMethod ( "WriteLine", WriteLine, rb_args.ANY () );
			state.DefineMethod ( "show_backtrace", ShowBackTrace, rb_args.NONE () );
#endif
			
            state.DoString ( "puts \"ruby #{RUBY_VERSION}\"" );
            state.DoString ( "WriteLine(\"Object.new\")" );
            state.DoString ( "show_backtrace" );
            state.DoString ( "WriteLineNormal( 'mruby ok!', 1, 9.9, true )" );
            state.DoString ( "puts RubySharp::CustomEnum::A" );
            state.DoString ( "puts RubySharp::CustomEnum::B" );
            state.DoString ( "puts RubySharp::CustomClass.new.FuncB( 999 )" );
            state.DoString ( "puts RubySharp::CustomClass.new.FuncC( 'AABB' )" );
            state.DoString ( "puts RubySharp::CustomClass.new.FuncD( 1, 2.0, true, 'HelloString', RubySharp::CustomClass.new )" );
            // state.DoString ( "puts RubySharp::CustomClass.new + 100" );
            state.DoString ( "puts RubySharp::CustomClass::FuncE( nil )" );
            state.DoString ( "puts RubySharp::CustomClass.FuncF( RubySharp::CustomClass.new, 900.0 )" );
            state.DoString ( "puts RubySharp::CustomClass.new.FuncG( RubySharp::CustomClass.new )" );

#if MRUBY
            if ( RubyDLL.mrb_has_exc ( state ) ) {
                Console.WriteLine ( state.GetExceptionBackTrace () );
                RubyDLL.mrb_exc_clear ( state );
            }
#else
			R_VAL exc = RubyDLL.rb_errinfo ();
			if ( RubyDLL.r_type ( exc ) != rb_vtype.RUBY_T_NIL ) {
				Console.WriteLine ( RubyState.GetExceptionBackTrace () );
			}
#endif
            
            // Console.ReadKey ();
        }

#if MRUBY
        static R_VAL WriteLine ( IntPtr state, R_VAL context ) {
            R_VAL[] args = RubyDLL.GetFunctionArgs ( state );

            string str = args[ 0 ].ToString ( state );

            Console.WriteLine ( str );

            return context;
        }

        static R_VAL ShowBackTrace ( IntPtr state, R_VAL context ) {
            Console.WriteLine ( Program.state.GetExceptionBackTrace () );
            return context;
        }

        static void TestDelegate ( string str, int a, float b, bool c, object obj ) {
            Console.WriteLine ( $"{str}, {a}, {b}, {c}, {obj}" );
        }
#else
		
		static R_VAL WriteLine ( int argc, R_VAL[] argv, R_VAL self ) {
			string str = argv[ 0 ].ToString ();
		
			Console.WriteLine ( str );
		
			return self;
		}
		
		static R_VAL ShowBackTrace ( int argc, R_VAL[] argv, R_VAL self ) {
			Console.WriteLine ( state.GetCurrentBackTrace () );
			return self;
		}
#endif

    }
}
