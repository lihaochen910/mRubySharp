#if MRUBY
using System;
using System.Runtime.InteropServices;


namespace RubySharp {
	
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
			Console.WriteLine ( $"CustomClass::FuncF() {first} {second}" );
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
		public static RubyState state;
		
		public static void __Register__ ( RubyState state ) {
			CustomEnum_Wrapper.state         = state;
			CustomEnum_Wrapper.@module       = UserDataUtility.DefineCSharpEnum ( state, typeof ( RubySharp.CustomEnum ) );
			CustomEnum_Wrapper.data_type_ptr = RubyDLL.ObjectToInPtr ( data_type );
			
			RubyDLL.r_define_const ( state, @module, "A", RubyDLL.INT2FIX ( ( int )RubySharp.CustomEnum.A ) );
			RubyDLL.r_define_const ( state, @module, "B", RubyDLL.INT2FIX ( ( int )RubySharp.CustomEnum.B ) );
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
		public static RubyState state;

		public static R_VAL initialize ( IntPtr mrb, R_VAL self ) {
			CustomClass instance = new CustomClass ();
			RubyDLL.mrb_data_init ( self, RubyDLL.ObjectToInPtr ( instance ), RubySharp.RubyState.DATA_TYPE_PTR );
			return self;
		}

		public static R_VAL a ( IntPtr mrb, R_VAL self ) {
			CustomClass instance = RubyDLL.ValueToDataObject< CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );
			return RubyDLL.INT2FIX ( ( int )instance.a );
		}

		public static R_VAL a_eql ( IntPtr mrb, R_VAL self ) {
			CustomClass instance = RubyDLL.ValueToDataObject< CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );
			R_VAL[] args = RubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.a parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !R_VAL.IsFixnum ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.a parameter type mismatch: require Int32 but got {RubyDLL.r_type ( args[ 0 ] )}." );
			}

			instance.a = ( System.Int32 )RubyDLL.FIX2INT ( args[ 0 ] );
			return self;
		}

		public static R_VAL b ( IntPtr mrb, R_VAL self ) {
			CustomClass instance = RubyDLL.ValueToDataObject< CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );
			return RubyDLL.DBL2NUM ( mrb, ( double )instance.b );
		}

		public static R_VAL b_eql ( IntPtr mrb, R_VAL self ) {
			CustomClass instance = RubyDLL.ValueToDataObject< CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );
			R_VAL[] args = RubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.b parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !R_VAL.IsFloat ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.b parameter type mismatch: require Single but got {RubyDLL.r_type ( args[ 0 ] )}." );
			}

			instance.b = ( Single )RubyDLL.mrb_float ( args[ 0 ] );
			return self;
		}

		public static R_VAL c ( IntPtr mrb, R_VAL self ) {
			CustomClass instance = RubyDLL.ValueToDataObject< CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );
			return R_VAL.Create ( mrb, instance.c );
		}

		public static R_VAL c_eql ( IntPtr mrb, R_VAL self ) {
			CustomClass instance = RubyDLL.ValueToDataObject< CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );
			R_VAL[] args = RubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.c parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !R_VAL.IsString ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.c parameter type mismatch: require String but got {RubyDLL.r_type ( args[ 0 ] )}." );
			}

			instance.c = args[ 0 ].ToString ( state );
			return self;
		}

		public static R_VAL A ( IntPtr mrb, R_VAL self ) {
			CustomClass instance = RubyDLL.ValueToDataObject< CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );
			return RubyDLL.INT2FIX ( ( int )instance.A );
		}

		public static R_VAL A_eql ( IntPtr mrb, R_VAL self ) {
			CustomClass instance = RubyDLL.ValueToDataObject< CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );
			R_VAL[] args = RubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.A parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !R_VAL.IsFixnum ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.A parameter type mismatch: require Int32 but got {RubyDLL.r_type ( args[ 0 ] )}." );
			}

			instance.A = ( System.Int32 )RubyDLL.FIX2INT ( args[ 0 ] );
			return self;
		}

		public static R_VAL C ( IntPtr mrb, R_VAL self ) {
			CustomClass instance = RubyDLL.ValueToDataObject< CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );
			return R_VAL.Create ( mrb, instance.C );
		}

		public static R_VAL C_eql ( IntPtr mrb, R_VAL self ) {
			CustomClass instance = RubyDLL.ValueToDataObject< CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );
			R_VAL[] args = RubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.C parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !R_VAL.IsString ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.C parameter type mismatch: require String but got {RubyDLL.r_type ( args[ 0 ] )}." );
			}

			instance.C = args[ 0 ].ToString ( state );
			return self;
		}

		public static R_VAL FuncA ( IntPtr mrb, R_VAL self ) {
			RubySharp.CustomClass instance = RubyDLL.ValueToDataObject< RubySharp.CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );

			instance.FuncA ();
			return self;
		}

		public static R_VAL FuncB ( IntPtr mrb, R_VAL self ) {
			RubySharp.CustomClass instance = RubyDLL.ValueToDataObject< RubySharp.CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );

			R_VAL[] args = RubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.FuncB parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !R_VAL.IsFixnum ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncB parameter type mismatch: require Int32 but got {RubyDLL.r_type ( args[ 0 ] )}." );
			}

			instance.FuncB ( ( System.Int32 )RubyDLL.FIX2INT ( args[ 0 ] ) );
			return self;
		}

		public static R_VAL FuncC ( IntPtr mrb, R_VAL self ) {
			RubySharp.CustomClass instance = RubyDLL.ValueToDataObject< RubySharp.CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );

			R_VAL[] args = RubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.FuncC parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !R_VAL.IsString ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncC parameter type mismatch: require String but got {RubyDLL.r_type ( args[ 0 ] )}." );
			}

			var ret = instance.FuncC ( args[ 0 ].ToString ( state ) );
			return RubyDLL.DBL2NUM ( mrb, ( double )ret );
		}

		public static R_VAL FuncD ( IntPtr mrb, R_VAL self ) {
			RubySharp.CustomClass instance = RubyDLL.ValueToDataObject< RubySharp.CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );

			R_VAL[] args = RubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 5 ) {
				throw new ArgumentException ( $"CustomClass.FuncD parameter count mismatch: require 5 but got {args.Length}." );
			}

			if ( !R_VAL.IsFixnum ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncD parameter type mismatch: require Int32 but got {RubyDLL.r_type ( args[ 0 ] )}." );
			}
			if ( !R_VAL.IsFloat ( args[ 1 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncD parameter type mismatch: require Single but got {RubyDLL.r_type ( args[ 1 ] )}." );
			}
			if ( !R_VAL.IsBool ( args[ 2 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncD parameter type mismatch: require Boolean but got {RubyDLL.r_type ( args[ 2 ] )}." );
			}
			if ( !R_VAL.IsString ( args[ 3 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncD parameter type mismatch: require String but got {RubyDLL.r_type ( args[ 3 ] )}." );
			}
			if ( !R_VAL.IsData ( args[ 4 ] ) && !R_VAL.IsNil ( args[ 4 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncD parameter type mismatch: require CustomClass but got {RubyDLL.r_type ( args[ 4 ] )}." );
			}

			var ret = instance.FuncD ( ( System.Int32 )RubyDLL.FIX2INT ( args[ 0 ] ), ( Single )RubyDLL.mrb_float ( args[ 1 ] ), R_VAL.Test ( args[ 2 ] ), args[ 3 ].ToString ( state ), RubyDLL.ValueToDataObject< RubySharp.CustomClass > ( mrb, args[ 4 ], RubySharp.RubyState.DATA_TYPE_PTR ) );
			return RubyDLL.DataObjectToValue ( mrb, RubySharp.CustomClass_Wrapper.@class, RubySharp.RubyState.DATA_TYPE_PTR, ret );
		}

		public static R_VAL FuncG ( IntPtr mrb, R_VAL self ) {
			RubySharp.CustomClass instance = RubyDLL.ValueToDataObject< RubySharp.CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );

			R_VAL[] args = RubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.FuncG parameter count mismatch: require 1 but got {args.Length}." );
			}


			var ret = instance.FuncG ( RubyDLL.ValueToObject ( mrb, args[ 0 ] ) );
			// return mRubyDLL.DataObjectToValue ( mrb, System.Object_Wrapper.@class, RubySharp.mRubyState.DATA_TYPE_PTR, ret );
			return self;
		}

		public static R_VAL STATIC_FuncE ( IntPtr mrb, R_VAL self ) {

			R_VAL[] args = RubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.FuncE parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !R_VAL.IsData ( args[ 0 ] ) && !R_VAL.IsNil ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncE parameter type mismatch: require CustomClass but got {RubyDLL.r_type ( args[ 0 ] )}." );
			}

			CustomClass.FuncE ( RubyDLL.ValueToDataObject< RubySharp.CustomClass > ( mrb, args[ 0 ], RubySharp.RubyState.DATA_TYPE_PTR ) );
			return self;
		}

		public static R_VAL STATIC_FuncF ( IntPtr mrb, R_VAL self ) {

			R_VAL[] args = RubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 2 ) {
				throw new ArgumentException ( $"CustomClass.FuncF parameter count mismatch: require 2 but got {args.Length}." );
			}

			if ( !R_VAL.IsData ( args[ 0 ] ) && !R_VAL.IsNil ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncF parameter type mismatch: require CustomClass but got {RubyDLL.r_type ( args[ 0 ] )}." );
			}
			if ( !R_VAL.IsFloat ( args[ 1 ] ) ) {
				throw new ArgumentException ( $"CustomClass.FuncF parameter type mismatch: require Single but got {RubyDLL.r_type ( args[ 1 ] )}." );
			}

			var ret = CustomClass.FuncF ( RubyDLL.ValueToDataObject< RubySharp.CustomClass > ( mrb, args[ 0 ], RubySharp.RubyState.DATA_TYPE_PTR ), ( Single )RubyDLL.mrb_float ( args[ 1 ] ) );
			return RubyDLL.DataObjectToValue ( mrb, RubySharp.CustomClass_Wrapper.@class, RubySharp.RubyState.DATA_TYPE_PTR, ret );
		}

		public static R_VAL _op_Addition ( IntPtr mrb, R_VAL self ) {
			CustomClass instance = RubyDLL.ValueToDataObject< CustomClass > ( mrb, self, RubySharp.RubyState.DATA_TYPE_PTR );

			R_VAL[] args = RubyDLL.GetFunctionArgs ( mrb );

			if ( args.Length != 1 ) {
				throw new ArgumentException ( $"CustomClass.op_Addition parameter count mismatch: require 1 but got {args.Length}." );
			}

			if ( !R_VAL.IsFixnum ( args[ 0 ] ) ) {
				throw new ArgumentException ( $"CustomClass.Int32 parameter type mismatch: require Int32 but got {RubyDLL.r_type ( args[ 0 ] )}." );
			}


			var right = ( System.Int32 )RubyDLL.FIX2INT ( args[ 0 ] );
			var ret = instance + right;
			return RubyDLL.INT2FIX ( ( int )ret );
		}

		public static void __Register__ ( RubyState state ) {
			CustomClass_Wrapper.state = state;
			CustomClass_Wrapper.@class = UserDataUtility.DefineCSharpClass ( state, typeof ( RubySharp.CustomClass ) );
			CustomClass_Wrapper.data_type_ptr = RubyDLL.ObjectToInPtr ( data_type_ptr );

			RubyDLL.r_define_method ( state, @class, "initialize", initialize, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "a", a, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "a=", a_eql, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "b", b, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "b=", b_eql, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "c", c, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "c=", c_eql, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "A", A, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "A=", A_eql, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "C", C, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "C=", C_eql, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "FuncA", FuncA, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "FuncB", FuncB, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "FuncC", FuncC, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "FuncD", FuncD, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "FuncG", FuncG, rb_args.ANY () );
			RubyDLL.r_define_method ( state, @class, "+", _op_Addition, rb_args.ANY () );

			RubyDLL.r_define_class_method ( state, @class, "FuncE", STATIC_FuncE, rb_args.ANY () );
			RubyDLL.r_define_class_method ( state, @class, "FuncF", STATIC_FuncF, rb_args.ANY () );
		}
	}
}
#endif