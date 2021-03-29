# (m)RubySharp

旨在实现C#与mruby的绑定，

因为随着游戏项目的增大，C#的编译速度太慢了(请忽略C++..XD)，
所以想在游戏中使用Ruby语言作为我的主要脚本开发语言


### C#当前已经绑定的接口:
* 初始化mrb_state
* 关闭mrb_state
* 执行mruby字符串代码
* 执行mrbc字节码
* 创建R_VAL - ( mruby: mrb_value || ruby: VALUE )
	* 创建Fixnum
	* 创建Float
	* 创建Boolean
	* 创建String
	* 创建引用
	* R_VAL转换到C#类型
* 创建符号
* 定义Class、获取Class
	定义Class方法
* 定义Module、获取Module
	定义Module方法
* 获取发生异常时的调用堆栈字符串
* 将C#类作为Data传递给mruby
* 调用mruby内的方法
* 从mruby调用C#方法时，取得传递过来的参数
* 导入C#类到mruby, 以及自动生成C#包装类代码
	(字段的访问与写入)
	(属性的访问与写入)
	(枚举类导入为Module,枚举值会导入为Module中的Fixnum类型常量)
	(运算符重载)
* 内置dofile方法加载rb源文件

### Usage
Code:
```csharp
RubyState state = new RubyState ();

R_VAL v1 = RubyDLL.mrb_fixnum_value ( state, 2333 );
R_VAL v2 = RubyDLL.mrb_float_value ( state, 65.5f );
R_VAL v3 = R_VAL.FALSE;
R_VAL v4 = R_VAL.TRUE;
R_VAL v5 = R_VAL.NIL;

mRubyClass klass = new mRubyClass ( state, "CSharpClass" );

klass.DefineMethod ( "write", WriteLine, mrb_args.ANY () );
```

Binding by Gen Wrapper Code:
```csharp
// 这将会在当前工作目录下生成CustomClass_Wrapper.cs文件
WrapperUtility.GenCSharpClass ( state, typeof ( CustomClass ) );

// 这将会在当前工作目录下生成Assembly中所有支持类的包装文件
WrapperUtility.GenByAssembly ( state, typeof ( UnityEngine.Object ).Assembly );

// 注册CustomClass类到mruby虚拟机中
CustomClass_Wrapper.__Register__ ( state );
```

Binding by C# Reflection:
```csharp
// 反射绑定CustomClass类
UserDataUtility.RegisterType< CustomClass >( state );

// 直接定义CustomClass枚举
UserDataUtility.RegisterType< CustomEnum >( state );
```

### So, how to build?
- mruby-sharedlib
(https://github.com/mattn/mruby-sharedlib)
- mruby
(https://github.com/mruby/mruby)
- 使用这个仓库中的mruby_src_modify\mruby.def覆盖mruby-sharedlib中对应的文件
- 使用这个仓库中mruby_src_modify目录下的源代码覆盖mruby中对应的文件
- cc.defines = %w(MRB_USE_FLOAT32 MRB_INT32 MRB_NO_BOXING MRB_UTF8_STRING MRB_MAIN_PROFILE)
- 在Developer Command Prompt for VS 20XX中构建mruby
- 拷贝mruby.dll || mruby.dylib

### Note
- mruby 3.0.0
- ruby 3.0.0
- C#运行时使用mono, 在windows平台测试使用.Net Framework会出现问题, 貌似mono的PInvoke实现兼容性更好
- 当前mrb_value使用无装箱结构(No Boxing)
- float使用32位, int使用32位
- 目前可以正常管理在ruby中实例化绑定类, 也可以处理没在ruby中实例化的已绑定C#实例(即只有C#实例但ruby端没有对应的ruby类实例)
- DFree正常调用, 但是同一引用有时候可能会多次DFree, C#需要实现管理引用传递次数???
- 如何在ruby中处理C#类的继承关系???
  当前绑定不包含父类方法
  尝试: 默认绑定System.Object类, 后续所有绑定都继承System.Object类, 不清楚这样是否在ruby实例化类的时候会同时创建两个实例(System.Object类实例和绑定类实例)
- 委托传递处理
- 结构体传递处理

### TODO
* C#与mruby交流时的引用管理
	(目前C#引用实例传递给mruby时会使用GCHandle.Alloc方法转换为IntPtr)
* 生成C#类的绑定代码，方便mruby调用
  计划在Unity游戏引擎/MonoGame中使用mruby作为游戏脚本语言
* 生成C#类的绑定类支持C#数组参数，支持嵌套类，泛型类??
* 尝试在IOS/Android/Nintendo Switch/PS4上构建mruby
  (这应该很轻松，因为mruby的目标就是实现轻量级和嵌入性)
* 在ruby中实现与mruby等效的交互
