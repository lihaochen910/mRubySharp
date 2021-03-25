#if MRUBY
using System;
using System.Collections.Generic;


namespace RubySharp {
    
    public class mRubyClass {
        
        private RubyState state;

        /// <summary>
        /// Class Name
        /// </summary>
        public string name { get; private set; }

        public IntPtr class_ptr   { get; private set; }
        public R_VAL class_value { get; private set; }

        /// <summary>
        /// 定义一个新Class，默认继承自mruby的Object
        /// </summary>
        public mRubyClass ( RubyState state, string name ) {
            mRubyClass.RegisteredClass.Add ( name, this );

            this.name = name;

            this.state       = state;
            this.class_ptr   = RubyDLL.r_define_class ( state, name, state.rb_object_class );
            this.class_value = R_VAL.CreateOBJ ( class_ptr );
        }

        public void DefineMethod ( string name, RubyDLL.RubyCSFunction receiver, rb_args aspec ) {
            // 防止被C#端GC
            state.MethodDelegates.Add ( receiver );

            RubyDLL.r_define_method ( state, class_ptr, name, receiver, aspec );
            //RubyDLL.mrb_define_module_function(state, class_ptr, name, receiver, aspec);

        }

        public void DefineStaticMethod ( string name, RubyDLL.RubyCSFunction receiver, rb_args aspec ) {
            // 防止被C#端GC
            state.MethodDelegates.Add ( receiver );

            RubyDLL.r_define_class_method ( state, class_ptr, name, receiver, aspec );
        }

        public R_VAL Call ( string funcName ) {
            return RubyDLL.r_funcall ( state.rb_state, class_value, funcName, 0 );
        }


        /// <summary>
        /// 统计C#在当前VM中定义的类
        /// </summary>
        static public Dictionary< string, mRubyClass > RegisteredClass { get; set; } = new Dictionary< string, mRubyClass > ();
    }
}
#endif