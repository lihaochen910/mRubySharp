#if MRUBY
using System;


namespace RubySharp {
    
    public class mRubyModule {
        
        /// <summary>
        /// Module Name
        /// </summary>
        public string name { get; private set; }

        public IntPtr module_ptr   { get; private set; }
        public R_VAL module_value { get; private set; }


        private RubyState state;

        public mRubyModule ( RubyState state, string name ) {
            this.name         = name;
            this.state        = state;
            this.module_ptr   = RubyDLL.r_define_module ( state.rb_state, name );
            this.module_value = R_VAL.CreateOBJ ( module_ptr );
        }

        public void DefineMethod ( string name, RubyDLL.RubyCSFunction receiver, rb_args aspec ) {
            // 防止被C#端GC
            state.MethodDelegates.Add ( receiver );

            RubyDLL.r_define_module_function ( state.rb_state, module_ptr, name, receiver, aspec );
        }

        public R_VAL Call ( string funcName ) {
            return RubyDLL.r_funcall ( state.rb_state, module_value, funcName, 0 );
        }
    }
}
#endif