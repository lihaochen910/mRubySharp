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

}
