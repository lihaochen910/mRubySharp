#include "ruby/ruby.h"
#include "ruby_sharp.h"

//===================================
RUBY_FUNC_EXPORTED VALUE INT2FIX_ex(intptr_t i)
{
  return INT2FIX(i);
}

RUBY_FUNC_EXPORTED VALUE INT2NUM_ex(int i)
{
  return INT2NUM(i);
}

RUBY_FUNC_EXPORTED VALUE UINT2NUM_ex(unsigned int u)
{
  return UINT2NUM(u);
}

RUBY_FUNC_EXPORTED VALUE LL2NUM_ex(signed long ll)
{
  return LL2NUM(ll);
}

RUBY_FUNC_EXPORTED VALUE ULL2NUM_ex(unsigned long ull)
{
  return ULL2NUM(ull);
}

RUBY_FUNC_EXPORTED VALUE DBL2NUM_ex(double d)
{
  return DBL2NUM(d);
}

RUBY_FUNC_EXPORTED intptr_t FIX2INT_ex(VALUE o)
{
  return FIX2INT(o);
}

RUBY_FUNC_EXPORTED intptr_t NUM2INT_ex(VALUE o)
{
  return NUM2INT(o);
}

RUBY_FUNC_EXPORTED uintptr_t NUM2UINT_ex(VALUE o)
{
  return NUM2UINT(o);
}

RUBY_FUNC_EXPORTED signed long NUM2LONG_ex(VALUE o)
{
  return NUM2LONG(o);
}

RUBY_FUNC_EXPORTED LONG_LONG NUM2LL_ex(VALUE o)
{
  return NUM2LL(o);
}

RUBY_FUNC_EXPORTED unsigned LONG_LONG NUM2ULL_ex(VALUE o)
{
  return NUM2ULL(o);
}

RUBY_FUNC_EXPORTED double NUM2DBL_ex(VALUE o)
{
  return NUM2DBL(o);
}

RUBY_FUNC_EXPORTED char NUM2CHR_ex(VALUE o)
{
  return NUM2CHR(o);
}

RUBY_FUNC_EXPORTED VALUE r_char_to_sym(const char* str)
{
  return ID2SYM(rb_intern(str));
}

RUBY_FUNC_EXPORTED enum ruby_value_type rb_type_ex(VALUE obj)
{
  return rb_type(obj);
}

RUBY_FUNC_EXPORTED bool r_test(VALUE val)
{
  return (val != Qfalse && val != Qnil);
}

RUBY_FUNC_EXPORTED void rb_define_method_ex(VALUE klass, const char *name, rb_func_t func, int argc)
{
  rb_define_method(klass, name, func, argc);
}

RUBY_FUNC_EXPORTED void rb_define_module_function_ex(VALUE module, const char *name, rb_func_t func, int argc)
{
  rb_define_module_function(module, name, func, argc);
}

RUBY_FUNC_EXPORTED void rb_define_singleton_method_ex(VALUE obj, const char *name, rb_func_t func, int argc)
{
  rb_define_singleton_method(obj, name, func, argc);
}

RUBY_FUNC_EXPORTED void rb_define_alloc_func_ex(VALUE klass, rb_alloc_func_t func)
{
  rb_define_alloc_func(klass, func);
}
