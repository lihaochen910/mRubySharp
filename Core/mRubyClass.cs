using System;
using System.Collections.Generic;

namespace CandyFramework.mRuby
{
    public class mRubyClass
    {
        private mRubyState state;

        /// <summary>
        /// Class Name
        /// </summary>
        public string name { get; private set; }
        public IntPtr class_ptr { get; private set; }
        public mrb_value class_value { get; private set; }

        /// <summary>
        /// 定义一个新Class，默认继承自mruby的Object
        /// </summary>
        public mRubyClass(mRubyState state, string name)
        {
            mRubyClass.RegisteredClass.Add(name, this);

            this.name = name;

            this.state = state;
            this.class_ptr = mRubyDLL.mrb_define_class(state, name, state.mrb_object_class);
            this.class_value = mrb_value.CreateOBJ(class_ptr);
        }

        public void DefineMethod(string name, mRubyDLL.MRubyCSFunction receiver, mrb_args aspec)
        {
            // 防止被C#端GC
            state.MethodDelegates.Add(receiver);

            mRubyDLL.mrb_define_method(state, class_ptr, name, receiver, aspec);
            //mRubyDLL.mrb_define_module_function(state, class_ptr, name, receiver, aspec);

        }

        public void DefineStaticMethod(string name, mRubyDLL.MRubyCSFunction receiver, mrb_args aspec)
        {
            // 防止被C#端GC
            state.MethodDelegates.Add(receiver);

            mRubyDLL.mrb_define_class_method(state, class_ptr, name, receiver, aspec);
        }

        public mrb_value Call(string funcName)
        {
            return mRubyDLL.mrb_funcall(state.mrb_state, class_value, funcName, 0);
        }


        /// <summary>
        /// 统计C#在当前VM中定义的类
        /// </summary>
        static public Dictionary<string, mRubyClass> RegisteredClass { get; set; } = new Dictionary<string, mRubyClass>();
    }
}
