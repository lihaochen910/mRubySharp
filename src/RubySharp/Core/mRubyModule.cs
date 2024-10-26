#if MRUBY
using System;


namespace RubySharp {
    
    public class mRubyModule {
        
        /// <summary>
        /// Module Name
        /// </summary>
        public string Name { get; private set; }

        public IntPtr ModulePtr   { get; private set; }
        public R_VAL ModuleValue { get; private set; }


        private RubyState _state;

        public mRubyModule( RubyState state, string name ) {
            Name = name;
            _state = state;
            ModulePtr   = RubyDLL.r_define_module( state.rb_state, name );
            ModuleValue = R_VAL.CreateOBJ( ModulePtr );
        }

        public void DefineMethod( string name, RubyDLL.RubyCSFunction receiver, rb_args aspec ) {
            // 防止被C#端GC
            _state.MethodDelegates.Add( receiver );

            RubyDLL.r_define_module_function( _state.rb_state, ModulePtr, name, receiver, aspec );
        }

        public R_VAL Call( string funcName ) {
            return RubyDLL.r_funcall( _state.rb_state, ModuleValue, funcName, 0 );
        }
        
    }

}
#endif