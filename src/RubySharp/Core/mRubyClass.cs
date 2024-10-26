#if MRUBY
using System;
using System.Collections.Generic;


namespace RubySharp {
    
    public class mRubyClass {
        
        /// <summary>
        /// 统计C#在当前VM中定义的类
        /// </summary>
        public static Dictionary< string, mRubyClass > RegisteredClass { get; } = new ();
        
        /// <summary>
        /// Class Name
        /// </summary>
        public string Name { get; private set; }

        public IntPtr ClassPtr   { get; private set; }
        public R_VAL ClassValue { get; private set; }
        
        private RubyState _state;

        /// <summary>
        /// 定义一个新Class，默认继承自mruby的Object
        /// </summary>
        public mRubyClass( RubyState state, string name ) {
            RegisteredClass.Add( name, this );

            Name = name;

            _state = state;
            ClassPtr = RubyDLL.r_define_class( state, name, state.rb_object_class );
            ClassValue = R_VAL.CreateOBJ( ClassPtr );
        }

        public void DefineMethod( string name, RubyDLL.RubyCSFunction receiver, rb_args aspec ) {
            // 防止被C#端GC
            _state.MethodDelegates.Add( receiver );

            RubyDLL.r_define_method( _state, ClassPtr, name, receiver, aspec );
            //RubyDLL.mrb_define_module_function(state, class_ptr, name, receiver, aspec);

        }

        public void DefineStaticMethod( string name, RubyDLL.RubyCSFunction receiver, rb_args aspec ) {
            // 防止被C#端GC
            _state.MethodDelegates.Add( receiver );

            RubyDLL.r_define_class_method( _state, ClassPtr, name, receiver, aspec );
        }

        public R_VAL Call( string funcName ) {
            return RubyDLL.r_funcall( _state.rb_state, ClassValue, funcName, 0 );
        }
        
    }
    
}
#endif