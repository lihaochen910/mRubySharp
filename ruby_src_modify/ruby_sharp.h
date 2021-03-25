#ifndef RUBY_SHARP_H
#define RUBY_SHARP_H

#include "ruby/ruby.h"
#include "ruby/internal/dllexport.h"

/**
 * MRuby Value definition functions and macros.
 */
RBIMPL_SYMBOL_EXPORT_BEGIN()

/*
 * value.h export
 */
VALUE INT2FIX_ex(intptr_t);
VALUE INT2NUM_ex(int);
VALUE UINT2NUM_ex(unsigned int);
VALUE LL2NUM_ex(signed long);
VALUE ULL2NUM_ex(unsigned long);
VALUE DBL2NUM_ex(double);

// VALUE TO_PDT_ex(void);
intptr_t FIX2INT_ex(VALUE);
intptr_t NUM2INT_ex(VALUE);
uintptr_t NUM2UINT_ex(VALUE);
signed long NUM2LONG_ex(VALUE);
LONG_LONG NUM2LL_ex(VALUE);
unsigned LONG_LONG NUM2ULL_ex(VALUE);
double NUM2DBL_ex(VALUE);
char NUM2CHR_ex(VALUE);

bool r_test(VALUE);
VALUE r_char_to_sym(const char*);
enum ruby_value_type rb_type_ex(VALUE obj);

/*
 * data.h export
 */
#define r_data_wrap_struct(name, data)  Data_Wrap_Struct(rb_cObject, NULL, (free_##name), data)
#define r_data_get_struct(self, var, mrb_type, rb_type, data)  Data_Get_Struct(r_iv_get(self, var), rb_type, data)

    /*
 * class.h export
 */
    /**
 * Function pointer type for a function callable by mruby.
 *
 * The arguments to the function are stored on the mrb_state. To get them see mrb_get_args
 *
 * @param mrb The mruby state
 * @param self The self object
 * @return [mrb_value] The function's return value
 */
typedef VALUE (*rb_func_t)(int argc, VALUE *argv, VALUE self);
typedef VALUE (*rb_alloc_func_t)(VALUE klass);

void rb_define_method_ex(VALUE klass, const char *name, rb_func_t func, int argc);
void rb_define_module_function_ex(VALUE module, const char *name, rb_func_t func, int argc);
void rb_define_singleton_method_ex(VALUE obj, const char *name, rb_func_t func, int argc);
void rb_define_alloc_func_ex(VALUE klass, rb_alloc_func_t func);

/*
 * Exception
 */
// mrb_bool mrb_has_exc(mrb_state*);
// void mrb_exc_clear(mrb_state *);
// VALUE mrb_exc_detail(mrb_state*);

RBIMPL_SYMBOL_EXPORT_END()

#endif /* RUBY_SHARP_H */
