using System;
using System.Runtime.InteropServices;

using mrb_float = System.Double;
using mrb_int = System.Int32;
using mrb_sym = System.UInt32;
using mrb_bool = System.Byte;


namespace CandyFramework.mRuby {
    
    /**
	 *  MrbValue Wrapper
	 * 
	 *  Create Helper to convert MrbValue to C# value types
	 */
    [StructLayout ( LayoutKind.Sequential )]
    public struct mrb_value {
        
        [StructLayout ( LayoutKind.Explicit )]
        public struct Value {
            [FieldOffset ( 0 )] public mrb_float  f;
            [FieldOffset ( 0 )] public IntPtr    p;
            [FieldOffset ( 0 )] public mrb_int   i;
            [FieldOffset ( 0 )] public mrb_sym   sym;
        }

        public Value     value;
        public mrb_vtype tt;

        public double    mrb_float  => value.f;
        public Int64     mrb_fixnum => value.i;
        public uint      mrb_symbol => value.sym;
        public mrb_vtype mrb_type   => tt;

        public static readonly mrb_value DEFAULT = new mrb_value () { tt = mrb_vtype.MRB_TT_UNDEF };
        public static readonly mrb_value TRUE    = mRubyDLL.mrb_true_value ();
        public static readonly mrb_value FALSE   = mRubyDLL.mrb_false_value ();
        public static readonly mrb_value NIL     = mRubyDLL.mrb_nil_value ();

        static public mrb_value Create ( int i ) {
            return mRubyDLL.mrb_fixnum_value ( i );
        }

        static public mrb_value Create ( IntPtr mrb_state, float i ) {
            return mRubyDLL.mrb_float_value ( mrb_state, i );
        }

        static public mrb_value Create ( IntPtr mrb_state, double dbl ) {
            return mRubyDLL.mrb_float_value ( mrb_state, dbl );
        }

        static public mrb_value Create ( bool b ) {
            return b ? mRubyDLL.mrb_true_value () : mRubyDLL.mrb_false_value ();
        }

        static public mrb_value Create ( IntPtr mrb_state, string str ) {
            var cbytes = mRubyDLL.ToCBytes ( str );
            return mRubyDLL.mrb_str_new_static ( mrb_state, cbytes, cbytes.Length );
        }

        static public mrb_value CreateNIL () {
            return mRubyDLL.mrb_nil_value ();
        }

        static public mrb_value CreateUNDEF () {
            return mRubyDLL.mrb_undef_value ();
        }

        static public mrb_value CreateOBJ ( IntPtr p ) {
            return mRubyDLL.mrb_obj_value ( p );
        }

        static public mrb_value CreateOBJ ( object obj ) {
            if ( obj == null )
                return mrb_value.CreateNIL ();

            return mrb_value.CreateOBJ ( GCHandle.ToIntPtr ( GCHandle.Alloc ( obj ) ) );
        }


        //static public implicit operator string(mrb_value value)
        //{
        //    int length = 0;
        //    IntPtr ptr = mRubyDLL.mrb_string_value_cstr(mRubyDLL.mrb_state, ref value);
        //    unsafe
        //    {
        //        byte* p = (byte*)ptr;
        //        while (*p != 0)
        //        {
        //            length++;
        //            p++;
        //        }
        //    }
        //    byte[] bytes = new byte[length];
        //    Marshal.Copy(ptr, bytes, 0, length);
        //    return mRubyDLL.Encoding.GetString(bytes);

        //    //IntPtr stringPtr = IntPtr.Zero;

        //    //if (value.tt == mrb_vtype.MRB_TT_STRING)
        //    //{
        //    //    stringPtr = MRUBY.mrb_string_value_ptr(MRUBY.mrb_state, value);
        //    //}
        //    //else
        //    //{
        //    //    stringPtr = MRUBY.mrb_string_value_ptr(MRUBY.mrb_state, MRUBY.mrb_obj_as_string(MRUBY.mrb_state, value));
        //    //}
        //    //return Marshal.PtrToStringAuto(stringPtr);
        //}

        static public implicit operator int ( mrb_value value ) {
            if ( value.tt == mrb_vtype.MRB_TT_FIXNUM ) {
                return value.value.i;
            }

            if ( value.tt == mrb_vtype.MRB_TT_FLOAT ) {
                return ( int )value.value.f;
            }

            return 0;
        }
        
        static public implicit operator float ( mrb_value value ) {
            if ( value.tt == mrb_vtype.MRB_TT_FLOAT ) {
                return ( float )value.value.f;
            }

            if ( value.tt == mrb_vtype.MRB_TT_FIXNUM ) {
                return ( float )value.value.i;
            }

            return 0f;
        }

        static public implicit operator double ( mrb_value value ) {
            if ( value.tt == mrb_vtype.MRB_TT_FLOAT ) {
                return value.value.f;
            }

            if ( value.tt == mrb_vtype.MRB_TT_FIXNUM ) {
                return value.value.i;
            }

            return 0f;
        }

        static public implicit operator bool ( mrb_value value ) {
            switch ( value.tt ) {
                case mrb_vtype.MRB_TT_FALSE:
                case mrb_vtype.MRB_TT_EXCEPTION:
                case mrb_vtype.MRB_TT_UNDEF:
                    return false;
            }

            return true;
        }

        static public implicit operator IntPtr ( mrb_value value ) {
            return value.value.p;
        }
        
        static public bool IsImmediate ( mrb_value value ) {
            return mRubyDLL.mrb_immediate_p ( value ) == 1;
        }
        
        static public bool IsFixnum ( mrb_value value ) {
            return mRubyDLL.mrb_fixnum_p ( value ) == 1;
        }
        
        static public bool IsSymbol ( mrb_value value ) {
            return mRubyDLL.mrb_symbol_p ( value ) == 1;
        }
        
        static public bool IsUndef ( mrb_value value ) {
            return mRubyDLL.mrb_undef_p ( value ) == 1;
        }
        
        static public bool IsNil ( mrb_value value ) {
            return mRubyDLL.mrb_nil_p ( value ) == 1;
        }
        
        static public bool IsFalse ( mrb_value value ) {
            return mRubyDLL.mrb_false_p ( value ) == 1;
        }
        
        static public bool IsTrue ( mrb_value value ) {
            return mRubyDLL.mrb_true_p ( value ) == 1;
        }
        
        static public bool IsFloat ( mrb_value value ) {
            return mRubyDLL.mrb_float_p ( value ) == 1;
        }
        
        static public bool IsArray ( mrb_value value ) {
            return mRubyDLL.mrb_array_p ( value ) == 1;
        }
        
        static public bool IsString ( mrb_value value ) {
            return mRubyDLL.mrb_string_p ( value ) == 1;
        }
        
        static public bool IsHash ( mrb_value value ) {
            return mRubyDLL.mrb_hash_p ( value ) == 1;
        }
        
        static public bool IsCPtr ( mrb_value value ) {
            return mRubyDLL.mrb_cptr_p ( value ) == 1;
        }
        
        static public bool IsException ( mrb_value value ) {
            return mRubyDLL.mrb_exception_p ( value ) == 1;
        }
        
        static public bool IsFree ( mrb_value value ) {
            return mRubyDLL.mrb_free_p ( value ) == 1;
        }
        
        static public bool IsObject ( mrb_value value ) {
            return mRubyDLL.mrb_object_p ( value ) == 1;
        }
        
        static public bool IsClass ( mrb_value value ) {
            return mRubyDLL.mrb_class_p ( value ) == 1;
        }
        
        static public bool IsModule ( mrb_value value ) {
            return mRubyDLL.mrb_module_p ( value ) == 1;
        }
        
        static public bool IsIClass ( mrb_value value ) {
            return mRubyDLL.mrb_iclass_p ( value ) == 1;
        }
        
        static public bool IsSClass ( mrb_value value ) {
            return mRubyDLL.mrb_sclass_p ( value ) == 1;
        }
        
        static public bool IsProc ( mrb_value value ) {
            return mRubyDLL.mrb_proc_p ( value ) == 1;
        }
        
        static public bool IsRange ( mrb_value value ) {
            return mRubyDLL.mrb_range_p ( value ) == 1;
        }
        
        static public bool IsFile ( mrb_value value ) {
            return mRubyDLL.mrb_file_p ( value ) == 1;
        }
        
        static public bool IsEnv ( mrb_value value ) {
            return mRubyDLL.mrb_env_p ( value ) == 1;
        }
        
        static public bool IsData ( mrb_value value ) {
            return mRubyDLL.mrb_data_p ( value ) == 1;
        }
        
        static public bool IsFiber ( mrb_value value ) {
            return mRubyDLL.mrb_fiber_p ( value ) == 1;
        }
        
        static public bool IsIStruct ( mrb_value value ) {
            return mRubyDLL.mrb_istruct_p ( value ) == 1;
        }
        
        static public bool IsBreak ( mrb_value value ) {
            return mRubyDLL.mrb_break_p ( value ) == 1;
        }
        
        static public bool IsBool ( mrb_value value ) {
            return mRubyDLL.mrb_true_p ( value ) == 1 || mRubyDLL.mrb_false_p ( value ) == 1;
        }
        
        static public bool Test ( mrb_value value ) {
            if ( mRubyDLL.mrb_true_p ( value ) == 1 ) {
                return true;
            }
            if ( mRubyDLL.mrb_false_p ( value ) == 1 ) {
                return false;
            }
            return mRubyDLL.mrb_test ( value ) == 1;
        }

        //public override string ToString()
        //{
        //    //IntPtr stringPtr = IntPtr.Zero;

        //    //if (tt == mrb_vtype.MRB_TT_STRING)
        //    //{
        //    //    stringPtr = MRUBY.mrb_string_value_ptr(MRUBY.mrb_state, this);
        //    //}
        //    //else
        //    //{
        //    //    stringPtr = MRUBY.mrb_string_value_ptr(MRUBY.mrb_state, MRUBY.mrb_obj_as_string(MRUBY.mrb_state, this));
        //    //}
        //    //return Marshal.PtrToStringAuto(stringPtr);

        //    var str = mRubyDLL.mrb_obj_as_string(mRubyDLL.mrb_state, this);
        //    int length = 0;
        //    IntPtr ptr = mRubyDLL.mrb_string_value_cstr(mRubyDLL.mrb_state, ref str);
        //    unsafe
        //    {
        //        byte* p = (byte*)ptr;
        //        while (*p != 0)
        //        {
        //            length++;
        //            p++;
        //        }
        //    }
        //    byte[] bytes = new byte[length];
        //    Marshal.Copy(ptr, bytes, 0, length);
        //    return mRubyDLL.Encoding.GetString(bytes);
        //}

        public string ToString ( IntPtr mrb_state ) {
            var    str    = mRubyDLL.mrb_obj_as_string ( mrb_state, this );
            int    length = 0;
            IntPtr ptr    = mRubyDLL.mrb_string_value_cstr ( mrb_state, ref str );
            unsafe {
                byte * p = ( byte * )ptr;
                while ( *p != 0 ) {
                    length++;
                    p++;
                }
            }

            byte[] bytes = new byte[length];
            Marshal.Copy ( ptr, bytes, 0, length );
            return mRubyDLL.Encoding.GetString ( bytes );
        }
    }
}
