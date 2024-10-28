using System;
using System.IO;
using System.Runtime.InteropServices;


namespace RubySharp.Sample {
    
	class Program {
		
		private static RubyState _state;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static unsafe void Main () {
            
            // https://github.com/FNA-XNA/FNA/wiki/4:-FNA-and-Windows-API#64-bit-support
            if ( Environment.OSVersion.Platform == PlatformID.Unix ) {
                string path = Environment.GetEnvironmentVariable( "PATH" );
                Environment.SetEnvironmentVariable( "PATH", Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "lib", "osx" ) + ";" + path );
			
                // string libPath = Environment.GetEnvironmentVariable ( "LD_LIBRARY_PATH" );
                // Environment.SetEnvironmentVariable ( "LD_LIBRARY_PATH", Path.Combine ( AppDomain.CurrentDomain.BaseDirectory, "lib", "osx" ) + ";" + libPath );
            }
			
			// Console.WriteLine ( (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB" );
			
            _state = new RubyState( ( IntPtr )RubyDLL.mrb_open() );
			_state.DoString( "puts \"ruby: #{RUBY_VERSION}\"" );

			// Console.WriteLine ( (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB" );

#if MRUBY
			R_VAL v1 = R_VAL.Create( _state, 2333 );
			R_VAL v2 = R_VAL.Create( _state, 3.1415926535897932f );
#else
			R_VAL v1 = R_VAL.Create( 2333 );
            R_VAL v2 = R_VAL.Create( 3.1415926535897932d );
#endif
            R_VAL v3 = R_VAL.FALSE;
            R_VAL v4 = R_VAL.TRUE;
            R_VAL v5 = R_VAL.NIL;

#if MRUBY
            Console.WriteLine( v1.ToString( _state ) );
            Console.WriteLine( v2.ToString( _state ) );
            Console.WriteLine( v3.ToString( _state ) );
            Console.WriteLine( v4.ToString( _state ) );
            Console.WriteLine( v5.ToString( _state ) );
			
			_state.DefineMethod( "WriteLine", WriteLine, rb_args.ANY() );
            _state.DefineMethod( "show_backtrace", ShowBackTrace, rb_args.NONE() );
            _state.DefineMethod( "WriteLineNormal", new System.Action< string, int, float, bool >( TestDelegate ), rb_args.ANY() );
			
			mRubyClass klass = new mRubyClass( _state, "CSharpClass" );

            klass.DefineMethod( "write", WriteLine, rb_args.ANY() );
			
			// WrapperUtility.GenCSharpClass( typeof( System.Object ) );
		    // WrapperUtility.GenCSharpClass( typeof( System.Array ) );
		    // WrapperUtility.GenCSharpClass( typeof( System.TimeSpan ) );
            // WrapperUtility.GenByAssembly( typeof( Microsoft.Xna.Framework.Game ).Assembly );
            // WrapperUtility.GenUnityEngineCommon();
            // WrapperUtility.GenCSharpClass( typeof( CustomClass ) );
            // WrapperUtility.GenCSharpClass( typeof( CustomEnum ) );

            UserDataUtility.RegisterType< CustomClass >( _state );
            UserDataUtility.RegisterType< CustomEnum >( _state );

            // CustomClass_Wrapper.__Register__( state );
            // CustomEnum_Wrapper.__Register__( state );

            // state.DoFile( "main.rb" );

#else
			Console.WriteLine( v1.ToString() );
			Console.WriteLine( v2.ToString() );
			Console.WriteLine( v3.ToString() );
			Console.WriteLine( v4.ToString() );
			Console.WriteLine( v5.ToString() );
			
			state.DefineMethod( "WriteLine", WriteLine, rb_args.ANY() );
			state.DefineMethod( "show_backtrace", ShowBackTrace, rb_args.NONE() );
#endif
			
            _state.DoString( "WriteLine(\"System::Object.new\")" );
            _state.DoString( "show_backtrace" );
            _state.DoString( "WriteLineNormal( 'mruby ok!', 1, 9.9, true )" );
            _state.DoString( "puts RubySharp::Sample::CustomEnum::A" );
            _state.DoString( "puts RubySharp::Sample::CustomEnum::B" );
            _state.DoString( "puts RubySharp::Sample::CustomEnum::C" );
            _state.DoString( "puts RubySharp::Sample::CustomClass.new.FuncB( 999 )" );
            _state.DoString( "puts RubySharp::Sample::CustomClass.new.FuncC( 'AABB' )" );
            _state.DoString( "puts RubySharp::Sample::CustomClass.new.FuncD( 1, 2.0, true, 'HelloString', RubySharp::Sample::CustomClass.new )" );
            _state.DoString( "puts RubySharp::Sample::CustomClass.+( RubySharp::Sample::CustomClass.new, 100 )" );
            _state.DoString( "puts RubySharp::Sample::CustomClass::FuncE( nil )" );
            _state.DoString( "puts RubySharp::Sample::CustomClass.FuncF( RubySharp::Sample::CustomClass.new, 900.0 )" );
            _state.DoString( "puts RubySharp::Sample::CustomClass.new.FuncG( RubySharp::Sample::CustomClass.new )" );

#if MRUBY
            if ( RubyDLL.mrb_has_exc( _state ) ) {
                Console.WriteLine( _state.GetExceptionBackTrace() );
                RubyDLL.mrb_exc_clear( _state );
            }
#else
			R_VAL exc = RubyDLL.rb_errinfo();
			if ( RubyDLL.r_type( exc ) != rb_vtype.RUBY_T_NIL ) {
				Console.WriteLine( RubyState.GetExceptionBackTrace() );
			}
#endif
			
			( ( IDisposable )_state ).Dispose();
            
            // Console.ReadKey();
        }

#if MRUBY
		// [UnmanagedCallersOnly]
        static R_VAL WriteLine( IntPtr state, R_VAL context ) {
            R_VAL[] args = RubyDLL.GetFunctionArgs( state );

            string str = args[ 0 ].ToString( state );

            Console.WriteLine( str );

            return context;
        }

        static R_VAL ShowBackTrace( IntPtr state, R_VAL context ) {
            Console.WriteLine( _state.GetExceptionBackTrace() );
            return context;
        }

        static void TestDelegate( string str, int a, float b, bool c ) {
            Console.WriteLine( $"{str}, {a}, {b}, {c}" );
        }
#else
		
		static R_VAL WriteLine( int argc, R_VAL[] argv, R_VAL self ) {
			string str = argv[ 0 ].ToString();
		
			Console.WriteLine( str );
		
			return self;
		}
		
		static R_VAL ShowBackTrace( int argc, R_VAL[] argv, R_VAL self ) {
			Console.WriteLine( state.GetCurrentBackTrace() );
			return self;
		}
#endif

    }
}
