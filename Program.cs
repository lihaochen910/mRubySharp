using System;
using System.Runtime.InteropServices;


namespace CandyFramework.mRuby {
    
    class Program {
        
        static mRubyState state;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main () {
            state = new mRubyState ();

            mrb_value v1 = mrb_value.Create ( 2333 );
            mrb_value v2 = mrb_value.Create ( state, 65.5f );
            mrb_value v3 = mrb_value.FALSE;
            mrb_value v4 = mrb_value.TRUE;
            mrb_value v5 = mrb_value.NIL;

            Console.WriteLine ( v1.ToString ( state ) );
            Console.WriteLine ( v2.ToString ( state ) );
            Console.WriteLine ( v3.ToString ( state ) );
            Console.WriteLine ( v4.ToString ( state ) );
            Console.WriteLine ( v5.ToString ( state ) );

            state.DefineMethod ( "log", WriteLine, mrb_args.ANY () );
            state.DefineMethod ( "show_backtrace", ShowBackTrace, mrb_args.NONE () );

            mRubyClass klass = new mRubyClass ( state, "CSharpClass" );

            klass.DefineMethod ( "write", WriteLine, mrb_args.ANY () );
            
            // UserDataUtility.GenCSharpClass ( typeof ( System.Object ) );
            // UserDataUtility.GenCSharpClass ( typeof ( System.Array ) );
            // UserDataUtility.GenCSharpClass ( typeof ( System.TimeSpan ) );
            // UserDataUtility.GenByAssembly ( typeof ( Microsoft.Xna.Framework.Game ).Assembly );
            // UserDataUtility.GenByAssembly ( typeof ( Godot.Node ).Assembly );
            // UserDataUtility.GenUnityEngineCommon ();
            UserDataUtility.GenCSharpClass ( typeof ( CustomClass ) );
            UserDataUtility.GenCSharpClass ( typeof ( CustomEnum ) );

            CustomClass_Wrapper.__Register__ ( state );
            CustomEnum_Wrapper.__Register__ ( state );

            state.DoFile ( "main.rb" );
            
            // state.DoString ( "puts \"mruby #{RUBY_VERSION}\"" );
            // state.DoString ( "puts CandyFramework::MRuby::CustomEnum::A" );
            // state.DoString ( "puts CandyFramework::MRuby::CustomEnum::B" );
            // state.DoString ( "puts CandyFramework::MRuby::CustomClass.new.FuncB( 999 )" );
            // state.DoString ( "puts CandyFramework::MRuby::CustomClass.new.FuncC( 'AABB' )" );
            // state.DoString ( "puts CandyFramework::MRuby::CustomClass.new.FuncD( 1, 2.0, true, 'HelloString', CandyFramework::MRuby::CustomClass.new )" );
            // state.DoString ( "puts CandyFramework::MRuby::CustomClass.new + 100" );
            // state.DoString ( "puts CandyFramework::MRuby::CustomClass::FuncE( nil )" );
            // state.DoString ( "puts CandyFramework::MRuby::CustomClass.FuncF( CandyFramework::MRuby::CustomClass.new, 900.0 )" );
            // state.DoString ( "puts CandyFramework::MRuby::CustomClass.FuncG()" );
            
            // Console.ReadKey ();
        }

        static mrb_value WriteLine ( IntPtr state, mrb_value context ) {
            mrb_value[] args = mRubyDLL.GetFunctionArgs ( state );

            string str = args[ 0 ].ToString ( state );

            Console.WriteLine ( str );

            return context;
        }

        static mrb_value ShowBackTrace ( IntPtr state, mrb_value context ) {
            Console.WriteLine ( Program.state.GetExceptionBackTrace () );
            return context;
        }
    }
}
