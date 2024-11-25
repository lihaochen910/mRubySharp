using System.Text;
using NUnit.Framework;


namespace RubySharp.Test;

public unsafe class RubyVMTests {
	
	private IntPtr _mrb;
	private RubyState _rubyState;

	[SetUp]
	public void Setup() {
		// 初始化mruby虚拟机
		_mrb = ( IntPtr )RubyDLL.mrb_open();
		_rubyState = new RubyState( _mrb );
		Assert.That( IntPtr.Zero, Is.Not.EqualTo( _mrb ), "Failed to initialize mruby state." );
	}

	[TearDown]
	public void Teardown() {
		// 关闭并释放mruby虚拟机
		if ( _mrb != IntPtr.Zero ) {
			RubyDLL.mrb_close( _mrb );
		}
		_rubyState?.Dispose();
	}

	[Test]
	public void TestIntRubyValueInteger() {
		R_VAL val = R_VAL.Create( _mrb, 2333 );
		var castVar = ( int )val;
		Assert.That( val.tt, Is.EqualTo( rb_vtype.RUBY_T_INTEGER ) );
		Assert.That( 2333, Is.EqualTo( castVar ) );
	}
	
	[Test]
	public void TestFloatRubyValueInteger() {
		R_VAL val = R_VAL.Create( _mrb, 3.1415926535897932f );
		var castVar = ( float )val;
		Assert.That( val.tt, Is.EqualTo( rb_vtype.RUBY_T_FLOAT ) );
		Assert.That( 3.1415926535897932f, Is.EqualTo( castVar ) );
	}
	
	[Test]
	public void TestFalseRubyValueInteger() {
		R_VAL val = R_VAL.FALSE;
		var castVar = ( bool )val;
		Assert.That( val.tt, Is.EqualTo( rb_vtype.RUBY_T_FALSE ) );
		Assert.That( R_VAL.IsFalse( val ) );
		Assert.That( false, Is.EqualTo( castVar ) );
	}
	
	[Test]
	public void TestTrueRubyValueInteger() {
		R_VAL val = R_VAL.TRUE;
		var castVar = ( bool )val;
		Assert.That( val.tt, Is.EqualTo( rb_vtype.RUBY_T_TRUE ) );
		Assert.That( R_VAL.IsTrue( val ) );
		Assert.That( true, Is.EqualTo( castVar ) );
	}
	
	[Test]
	public void TestNilRubyValueInteger() {
		R_VAL val = R_VAL.NIL;
		Assert.That( R_VAL.IsNil( val ) );
	}
	
	[Test]
	public void TestDoStringException() {
		int arena = RubyDLL.mrb_gc_arena_save( _mrb );
		IntPtr mrbc_context = RubyDLL.mrbc_context_new( _mrb );
		RubyDLL.mrbc_filename( _mrb, mrbc_context, "*interactive*" );
		R_VAL val = RubyDLL.mrb_load_string_cxt( _mrb, RubyDLL.ToCBytes( "balabala" ), mrbc_context );
		RubyDLL.mrbc_context_free( _mrb, mrbc_context );
		
		Assert.That( RubyDLL.mrb_has_exc( _mrb ) );
		R_VAL exc = RubyDLL.mrb_get_exc_value( _mrb );
		Assert.That( exc.tt, Is.EqualTo( rb_vtype.RUBY_T_EXCEPTION ) );
		Assert.That( R_VAL.IsNil( val ) );
		
		RubyDLL.mrb_exc_clear( _mrb );
		RubyDLL.mrb_gc_arena_restore( _mrb, arena );
	}

	[Test]
	public void TestDefineMethod_01() {
		bool methodCalled = false;

		R_VAL AMethod( IntPtr mrb, R_VAL context ) {
			methodCalled = true;
			return R_VAL.TRUE;
		}
		
		_rubyState.DefineMethod( "a_method", AMethod, rb_args.NONE() );
		R_VAL methodReturn = _rubyState.DoString( "a_method" );
		Assert.That( R_VAL.IsTrue( methodReturn ) );
		Assert.That( methodCalled );
		_rubyState.UndefineMethod( "a_method", AMethod );
	}

	[Test]
	public void TestDefineMethod_02() {
		bool methodCalled = false;

		R_VAL AMethod( IntPtr mrb, R_VAL context ) {
			methodCalled = true;
			var args = RubyDLL.GetFunctionArgs( mrb );
			Assert.That( 3, Is.EqualTo( ( int )args[ 0 ] ) );
			Assert.That( 2.0f, Is.EqualTo( ( float )args[ 1 ] ) );
			Assert.That( R_VAL.IsTrue( args[ 2 ] ) );
			return R_VAL.Create( mrb, 0 );
		}
		
		_rubyState.DefineMethod( "b_method", AMethod, rb_args.ANY() );
		R_VAL methodReturn = _rubyState.DoString( "b_method(3, 2.0, true)" );
		Assert.That( 0, Is.EqualTo( ( int )methodReturn ) );
		Assert.That( methodCalled );
		_rubyState.UndefineMethod( "b_method", AMethod );
	}

	[Test]
	public void TestMRubyGV() {
		const string ScriptDefineGV = "$var = 9";
		_rubyState.DoString( ScriptDefineGV );

		var symVal = RubyDLL.mrb_intern_cstr( _rubyState, "$var"u8.ToArray() );
		var mrbVal = RubyDLL.mrb_gv_get( _rubyState, symVal );
		TestContext.WriteLine( $"var is {( int )mrbVal}" );
		Assert.That( 9, Is.EqualTo( ( int )mrbVal ) );
		
		RubyDLL.mrb_gv_set( _rubyState, symVal, R_VAL.Create( _rubyState, 10 ) );
		
		mrbVal = RubyDLL.mrb_gv_get( _rubyState, symVal );
		TestContext.WriteLine( "after mrb_gv_set..." );
		TestContext.WriteLine( $"var is {( int )mrbVal}" );
		Assert.That( 10, Is.EqualTo( ( int )mrbVal ) );
	}

	[Test]
	public void TestMRubyInternalGC() {
		TestContext.WriteLine( $"GC已启用: {( bool )_rubyState.DoString( "MRuby.gc_enabled?" )}" );
		TestContext.WriteLine( $"当前对象数: {( int )_rubyState.DoString( "MRuby.object_count" )}" );

		const string ScriptDefineGV = "$var = 9";
		_rubyState.DoString( ScriptDefineGV );

		var mrbVal = RubyDLL.mrb_gv_get( _rubyState, RubyDLL.mrb_intern_cstr( _rubyState, Encoding.ASCII.GetBytes( "$var" ) ) );
		Assert.That( 9, Is.EqualTo( ( int )mrbVal ) );
		
		const string Script = """
							  # 打印当前的GC状态
							  def print_gc_status
							    puts "当前GC状态:"
							    # puts "  GC已启用: #{MRuby.gc_enabled?}"
							    # puts "  当前对象数: #{MRuby.object_count}"
							  end
							  
							  # 打印初始状态
							  print_gc_status
							  
							  # 创建大量对象以测试GC
							  100_000.times do |i|
							    obj = "对象 #{i}"
							  end
							  
							  # 强制进行垃圾回收
							  GC.start
							  
							  # 打印GC后的状态
							  print_gc_status
							  
							  # 创建更多对象
							  100_000.times do |i|
							    obj = "新对象 #{i}"
							  end
							  
							  # 再次强制进行垃圾回收
							  GC.start
							  
							  # 打印最终状态
							  print_gc_status
							  """;

		_rubyState.DoString( Script );
		Assert.Pass( "测试通过" );
	}

}
