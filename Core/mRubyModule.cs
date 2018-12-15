using System;

namespace CandyFramework.mRuby
{
    public class mRubyModule
    {
        /// <summary>
        /// Module Name
        /// </summary>
        public string name { get; private set; }
        public IntPtr module_ptr { get; private set; }
        public mrb_value module_value { get; private set; }
        

        private mRubyState state;

        public mRubyModule(mRubyState state, string name)
        {
            this.name = name;

            this.state = state;
            this.module_ptr = mRubyDLL.mrb_define_module(state.mrb_state, name);
            this.module_value = mrb_value.CreateOBJ(module_ptr);
        }

        public void DefineMethod(string name, mRubyDLL.MRubyCSFunction receiver, mrb_args aspec)
        {
            // 防止被C#端GC
            state.MethodDelegates.Add(receiver);

            mRubyDLL.mrb_define_module_function(state.mrb_state, module_ptr, name, receiver, aspec);
        }

        public mrb_value Call(string funcName)
        {
            return mRubyDLL.mrb_funcall(state.mrb_state, module_value, funcName, 0);
        }
    }
}
