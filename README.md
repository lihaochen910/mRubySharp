# mRubySharp

旨在实现C#与mruby的绑定，

因为随着游戏项目的增大，C#的编译速度太慢了(请忽略C++..XD)，
所以想在游戏中使用Ruby语言作为我的主要脚本开发语言


### C#当前已经绑定的接口:
* 初始化mrb_state
* 关闭mrb_state
* 执行mruby字符串代码
* 执行mrbc字节码
* 创建mrb_value
	* 创建Fixnum
	* 创建Float
	* 创建Boolean
	* 创建String
	* 创建引用
	* mrb_value转换到C#类型
* 创建符号
* 定义Class、获取Class
	定义Class方法
* 定义Module、获取Module
	定义Module方法
* 将C#类作为Data传递给mruby
* 调用mruby内的方法
* 从mruby调用C#方法时，取得传递过来的参数
* 导入C#类到mruby, 以及自动生成C#包装类代码


### Usage
Code:
```csharp
mRubyState state = new mRubyState();

mrb_value v1 = mRubyDLL.mrb_fixnum_value_ex(2333);
mrb_value v2 = mRubyDLL.mrb_float_value_ex(state, 65.5f);
mrb_value v3 = mrb_value.FALSE;
mrb_value v4 = mrb_value.TRUE;
mrb_value v5 = mrb_value.NIL;

mRubyClass klass = new mRubyClass(state, "CSharpClass");

klass.DefineMethod("write", WriteLine, mrb_args.ANY());
```

Gen Wrapper Code:
```csharp
UserDataUtility.GenCSharpClass ( state, typeof ( CustomClass ) );
UserDataUtility.GenByAssembly ( state, typeof ( UnityEngine.Object ).Assembly );
```

### So, how to build?
- mruby-sharedlib
(https://github.com/mattn/mruby-sharedlib)
- mruby
(https://github.com/mruby/mruby)
- 使用这个仓库中的mruby_src_modify\mruby.def覆盖mruby-sharedlib中对应的文件
- 使用这个仓库中mruby_src_modify目录下的源代码覆盖mruby中对应的文件
- 在Developer Command Prompt for VS 20XX中构建mruby
- 拷贝mruby.dll || mruby.dylib

### TODO
* C#与mruby交流时的引用管理
* 生成C#类的绑定代码，方便mruby调用
  计划在Unity游戏引擎/MonoGame中使用mruby作为游戏脚本语言
* 生成C#类的绑定类支持C#数组参数，支持枚举类，支持嵌套类，泛型类??
* 尝试在IOS/Android/Nintendo Switch/PS4上构建mruby
  (这应该很轻松，因为mruby的目标就是实现轻量级和嵌入性)
