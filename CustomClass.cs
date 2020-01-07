using System;
using System.Runtime.InteropServices;


namespace CandyFramework.mRuby {
	
	/// <summary>
	/// Example to export to mruby
	/// </summary>
	public class CustomClass {
		
		public int a;
		public float b;
		public string c;

		public int A {
			get => a;
			set => a = value;
		}
		
		public string C {
			get => c;
			set => c = value;
		}

		public CustomClass() {}

		public void FuncA () {
			Console.WriteLine ( "CustomClass::FuncA()" );
		}
		
		public void FuncB ( int first ) {
			Console.WriteLine ( $"CustomClass::FuncB() {first}" );
		}
		
		public float FuncC ( string first ) {
			Console.WriteLine ( $"CustomClass::FuncC() {first}" );
			return -1f;
		}
		
		public CustomClass FuncD ( int first, float second, bool third, string fourth, CustomClass fifth ) {
			Console.WriteLine ( $"CustomClass::FuncD() {first} {second} {third} {fourth} {fifth}" );
			return this;
		}
		
		public object FuncG ( object first ) {
			Console.WriteLine ( $"CustomClass::FuncG() {first}" );
			return this;
		}

		public static void FuncE ( CustomClass first ) {
			Console.WriteLine ( $"CustomClass::FuncE() {first}" );
		}
		
		public static CustomClass FuncF ( CustomClass first, float second ) {
			Console.WriteLine ( $"CustomClass::FuncF() {first}" );
			return first;
		}

		public static int operator + ( CustomClass left, int right ) {
			int result = left.a + right;
			return result;
		}

		public class CustomClassSubClass {
			public CustomClass parent;
		}
	}

	public enum CustomEnum {
		A,
		B,
		C
	}

	public class CustomEnum_Wrapper {
		
		public static readonly mrb_data_type data_type = new mrb_data_type () {
			struct_name = "CustomEnum",
			dfree       = null
		};

		public static IntPtr     @module;
		public static IntPtr     data_type_ptr;
		public static mRubyState state;
		
		public static void __Register__ ( mRubyState state ) {
			CustomEnum_Wrapper.state         = state;
			CustomEnum_Wrapper.@module       = UserDataUtility.DefineCSharpEnum ( state, typeof ( CandyFramework.mRuby.CustomEnum ) );
			CustomEnum_Wrapper.data_type_ptr = mRubyDLL.ObjectToInPtr ( data_type );
			
			mRubyDLL.mrb_define_const ( state, @module, "A", mRubyDLL.mrb_fixnum_value ( ( int )CandyFramework.mRuby.CustomEnum.A ) );
			mRubyDLL.mrb_define_const ( state, @module, "B", mRubyDLL.mrb_fixnum_value ( ( int )CandyFramework.mRuby.CustomEnum.B ) );
		}
	}
	
	/// <summary>
	/// CustomClass导入Ruby中生成的代码
	/// </summary>
	public class CustomClass_Wrapper {
		
		public static readonly mrb_data_type data_type = new mrb_data_type () {
			struct_name = "CustomClass",
			dfree = null
		};

		public static IntPtr @class;
		public static IntPtr data_type_ptr;
		public static mRubyState state;

		public static mrb_value initialize ( IntPtr mrb, mrb_value self ) {
			CustomClass instance = new CustomClass ();
			mRubyDLL.mrb_data_init ( self, mRubyDLL.ObjectToInPtr ( instance ), CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );
			return self;
		}

		public static mrb_value a ( IntPtr mrb, mrb_value self ) {
			CustomClass instance = mRubyDLL.ValueToDataObject< CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );
			return mRubyDLL.mrb_fixnum_value ( ( int )instance.a );
		}

		public static mrb_value a_eql ( IntPtr mrb, mrb_value self ) {
			CustomClass instance = mRubyDLL.ValueToDataObject< CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );
			mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.a parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !mrb_value.IsFixnum ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.a parameter type mismatch: require Int32 but got {mRubyDLL.mrb_type ( args[ 0 ] )}." );
			}

			instance.a = ( System.Int32 )mRubyDLL.mrb_fixnum ( args[ 0 ] );
			return self;
		}

		public static mrb_value b ( IntPtr mrb, mrb_value self ) {
			CustomClass instance = mRubyDLL.ValueToDataObject< CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );
			return mRubyDLL.mrb_float_value ( mrb, ( double )instance.b );
		}

		public static mrb_value b_eql ( IntPtr mrb, mrb_value self ) {
			CustomClass instance = mRubyDLL.ValueToDataObject< CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );
			mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.b parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !mrb_value.IsFloat ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.b parameter type mismatch: require Single but got {mRubyDLL.mrb_type ( args[ 0 ] )}." );
			}

			instance.b = ( Single )mRubyDLL.mrb_float ( args[ 0 ] );
			return self;
		}

		public static mrb_value c ( IntPtr mrb, mrb_value self ) {
			CustomClass instance = mRubyDLL.ValueToDataObject< CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );
			return mrb_value.Create ( mrb, instance.c );
		}

		public static mrb_value c_eql ( IntPtr mrb, mrb_value self ) {
			CustomClass instance = mRubyDLL.ValueToDataObject< CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );
			mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.c parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !mrb_value.IsString ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.c parameter type mismatch: require String but got {mRubyDLL.mrb_type ( args[ 0 ] )}." );
			}

			instance.c = args[ 0 ].ToString ( state );
			return self;
		}

		public static mrb_value A ( IntPtr mrb, mrb_value self ) {
			CustomClass instance = mRubyDLL.ValueToDataObject< CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );
			return mRubyDLL.mrb_fixnum_value ( ( int )instance.A );
		}

		public static mrb_value A_eql ( IntPtr mrb, mrb_value self ) {
			CustomClass instance = mRubyDLL.ValueToDataObject< CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );
			mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.A parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !mrb_value.IsFixnum ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.A parameter type mismatch: require Int32 but got {mRubyDLL.mrb_type ( args[ 0 ] )}." );
			}

			instance.A = ( System.Int32 )mRubyDLL.mrb_fixnum ( args[ 0 ] );
			return self;
		}

		public static mrb_value C ( IntPtr mrb, mrb_value self ) {
			CustomClass instance = mRubyDLL.ValueToDataObject< CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );
			return mrb_value.Create ( mrb, instance.C );
		}

		public static mrb_value C_eql ( IntPtr mrb, mrb_value self ) {
			CustomClass instance = mRubyDLL.ValueToDataObject< CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );
			mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.C parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !mrb_value.IsString ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.C parameter type mismatch: require String but got {mRubyDLL.mrb_type ( args[ 0 ] )}." );
			}

			instance.C = args[ 0 ].ToString ( state );
			return self;
		}

		public static mrb_value FuncA ( IntPtr mrb, mrb_value self ) {
			CandyFramework.mRuby.CustomClass instance = mRubyDLL.ValueToDataObject< CandyFramework.mRuby.CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );

			instance.FuncA ();
			return self;
		}

		public static mrb_value FuncB ( IntPtr mrb, mrb_value self ) {
			CandyFramework.mRuby.CustomClass instance = mRubyDLL.ValueToDataObject< CandyFramework.mRuby.CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );

			mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.FuncB parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !mrb_value.IsFixnum ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncB parameter type mismatch: require Int32 but got {mRubyDLL.mrb_type ( args[ 0 ] )}." );
			}

			instance.FuncB ( ( System.Int32 )mRubyDLL.mrb_fixnum ( args[ 0 ] ) );
			return self;
		}

		public static mrb_value FuncC ( IntPtr mrb, mrb_value self ) {
			CandyFramework.mRuby.CustomClass instance = mRubyDLL.ValueToDataObject< CandyFramework.mRuby.CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );

			mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.FuncC parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !mrb_value.IsString ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncC parameter type mismatch: require String but got {mRubyDLL.mrb_type ( args[ 0 ] )}." );
			}

			var ret = instance.FuncC ( args[ 0 ].ToString ( state ) );
			return mRubyDLL.mrb_float_value ( mrb, ( double )ret );
		}

		public static mrb_value FuncD ( IntPtr mrb, mrb_value self ) {
			CandyFramework.mRuby.CustomClass instance = mRubyDLL.ValueToDataObject< CandyFramework.mRuby.CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );

			mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 5 ) {
				throw new ArgumentException ( $"CustomClass.FuncD parameter count mismatch: require 5 but got {args.Length}." );
			}

			if ( !mrb_value.IsFixnum ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncD parameter type mismatch: require Int32 but got {mRubyDLL.mrb_type ( args[ 0 ] )}." );
			}
			if ( !mrb_value.IsFloat ( args[ 1 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncD parameter type mismatch: require Single but got {mRubyDLL.mrb_type ( args[ 1 ] )}." );
			}
			if ( !mrb_value.IsBool ( args[ 2 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncD parameter type mismatch: require Boolean but got {mRubyDLL.mrb_type ( args[ 2 ] )}." );
			}
			if ( !mrb_value.IsString ( args[ 3 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncD parameter type mismatch: require String but got {mRubyDLL.mrb_type ( args[ 3 ] )}." );
			}
			if ( !mrb_value.IsData ( args[ 4 ] ) && !mrb_value.IsNil ( args[ 4 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncD parameter type mismatch: require CustomClass but got {mRubyDLL.mrb_type ( args[ 4 ] )}." );
			}

			var ret = instance.FuncD ( ( System.Int32 )mRubyDLL.mrb_fixnum ( args[ 0 ] ), ( Single )mRubyDLL.mrb_float ( args[ 1 ] ), mrb_value.Test ( args[ 2 ] ), args[ 3 ].ToString ( state ), mRubyDLL.ValueToDataObject< CandyFramework.mRuby.CustomClass > ( mrb, args[ 4 ], CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR ) );
			return mRubyDLL.DataObjectToValue ( mrb, CandyFramework.mRuby.CustomClass_Wrapper.@class, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR, ret );
		}

		public static mrb_value FuncG ( IntPtr mrb, mrb_value self ) {
			CandyFramework.mRuby.CustomClass instance = mRubyDLL.ValueToDataObject< CandyFramework.mRuby.CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );

			mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.FuncG parameter count mismatch: require 1 but got {args.Length}." );
			}


			var ret = instance.FuncG ( mRubyDLL.ValueToObject ( mrb, args[ 0 ] ) );
			// return mRubyDLL.DataObjectToValue ( mrb, System.Object_Wrapper.@class, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR, ret );
			return self;
		}

		public static mrb_value STATIC_FuncE ( IntPtr mrb, mrb_value self ) {

			mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.FuncE parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !mrb_value.IsData ( args[ 0 ] ) && !mrb_value.IsNil ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncE parameter type mismatch: require CustomClass but got {mRubyDLL.mrb_type ( args[ 0 ] )}." );
			}

			CustomClass.FuncE ( mRubyDLL.ValueToDataObject< CandyFramework.mRuby.CustomClass > ( mrb, args[ 0 ], CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR ) );
			return self;
		}

		public static mrb_value STATIC_FuncF ( IntPtr mrb, mrb_value self ) {

			mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 2 ) {
				throw new ArgumentException ( $"CustomClass.FuncF parameter count mismatch: require 2 but got {args.Length}." );
			}

			if ( !mrb_value.IsData ( args[ 0 ] ) && !mrb_value.IsNil ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncF parameter type mismatch: require CustomClass but got {mRubyDLL.mrb_type ( args[ 0 ] )}." );
			}
			if ( !mrb_value.IsFloat ( args[ 1 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncF parameter type mismatch: require Single but got {mRubyDLL.mrb_type ( args[ 1 ] )}." );
			}

			var ret = CustomClass.FuncF ( mRubyDLL.ValueToDataObject< CandyFramework.mRuby.CustomClass > ( mrb, args[ 0 ], CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR ), ( Single )mRubyDLL.mrb_float ( args[ 1 ] ) );
			return mRubyDLL.DataObjectToValue ( mrb, CandyFramework.mRuby.CustomClass_Wrapper.@class, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR, ret );
		}

		public static mrb_value _op_Addition ( IntPtr mrb, mrb_value self ) {
			CustomClass instance = mRubyDLL.ValueToDataObject< CustomClass > ( mrb, self, CandyFramework.mRuby.mRubyState.DATA_TYPE_PTR );

			mrb_value[] args = mRubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.op_Addition parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !mrb_value.IsFixnum ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.Int32 parameter type mismatch: require Int32 but got {mRubyDLL.mrb_type ( args[ 0 ] )}." );
			}


			var right = ( System.Int32 )mRubyDLL.mrb_fixnum ( args[ 0 ] );
			var ret = instance + right;
			return mRubyDLL.mrb_fixnum_value ( ( int )ret );
		}

		public static void __Register__ ( mRubyState state ) {
			CustomClass_Wrapper.state = state;
			CustomClass_Wrapper.@class = UserDataUtility.DefineCSharpClass ( state, typeof ( CandyFramework.mRuby.CustomClass ) );
			CustomClass_Wrapper.data_type_ptr = mRubyDLL.ObjectToInPtr ( data_type_ptr );

			mRubyDLL.mrb_define_method ( state, @class, "initialize", initialize, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "a", a, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "a=", a_eql, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "b", b, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "b=", b_eql, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "c", c, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "c=", c_eql, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "A", A, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "A=", A_eql, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "C", C, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "C=", C_eql, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "FuncA", FuncA, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "FuncB", FuncB, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "FuncC", FuncC, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "FuncD", FuncD, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "FuncG", FuncG, mrb_args.ANY () );
			mRubyDLL.mrb_define_method ( state, @class, "+", _op_Addition, mrb_args.ANY () );

			mRubyDLL.mrb_define_class_method ( state, @class, "FuncE", STATIC_FuncE, mrb_args.ANY () );
			mRubyDLL.mrb_define_class_method ( state, @class, "FuncF", STATIC_FuncF, mrb_args.ANY () );
		}
	}
}
