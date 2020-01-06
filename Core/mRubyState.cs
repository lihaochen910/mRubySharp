using System;
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

        public mRubyState () {
            mrb_state = mRubyDLL.mrb_open ();

            mrb_object_class  = DoString ( "Object" );
            mrb_kernel_module = DoString ( "Kernel" );
        }

        public mrb_value DoString ( string str ) {
            return mRubyDLL.mrb_load_string ( mrb_state, mRubyDLL.ToCBytes ( str ) );
        }

        public mrb_value DoStringCxt ( string str ) {
            IntPtr mrbc_context = mRubyDLL.mrbc_context_new ( mrb_state );
            var    ret          = mRubyDLL.mrb_load_string_cxt ( mrb_state, mRubyDLL.ToCBytes ( str ), mrbc_context );
            mRubyDLL.mrbc_context_free ( mrb_state, mrbc_context );
            return ret;
        }

        public mrb_value DoByteCode ( byte[] bytecode ) {
            return mRubyDLL.mrb_load_irep ( mrb_state, bytecode );
        }

        public mrb_value Call ( string funcName ) {
            return mRubyDLL.mrb_funcall ( mrb_state, mRubyDLL.mrb_top_self ( mrb_state ), funcName, 0 );
        }

        public mrb_value Call ( string funcName, mrb_value arg ) {
            return mRubyDLL.mrb_funcall_1 ( mrb_state, mRubyDLL.mrb_top_self ( mrb_state ), funcName, 1, arg );
        }

        /// <summary>
        /// 获取当前调用堆栈信息
        /// https://qiita.com/seanchas_t/items/ca293f9dd4454cd6cb6d
        /// </summary>
        public string GetCurrentBackTrace () {
            return mRubyDLL.mrb_inspect ( mrb_state, mRubyDLL.mrb_get_backtrace ( mrb_state ) ).ToString ( mrb_state );
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



        static public implicit operator IntPtr ( mRubyState state ) {
            return state.mrb_state;
        }
    }
}
