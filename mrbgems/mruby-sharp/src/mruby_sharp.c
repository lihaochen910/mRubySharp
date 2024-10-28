#include <mruby.h>
#include "mruby_sharp.h"

//===================================
MRB_API mrb_value mrb_float_value_ex(mrb_state *mrb, mrb_float f)
{
  mrb_value v;
  (void) mrb;
  SET_FLOAT_VALUE(mrb, v, f);
  return v;
}

MRB_API mrb_value mrb_fixnum_value_ex(mrb_state *mrb, mrb_int i)
{
  mrb_value v;
  SET_INT_VALUE(mrb, v, i);
  return v;
}

MRB_API mrb_value mrb_nil_value_ex(void)
{
  mrb_value v;
  SET_NIL_VALUE(v);
  return v;
}

MRB_API mrb_value mrb_false_value_ex(void)
{
  mrb_value v;
  SET_FALSE_VALUE(v);
  return v;
}

MRB_API mrb_value mrb_true_value_ex(void)
{
  mrb_value v;
  SET_TRUE_VALUE(v);
  return v;
}

MRB_API mrb_value mrb_bool_value_ex(mrb_bool boolean)
{
  mrb_value v;
  SET_BOOL_VALUE(v, boolean);
  return v;
}

MRB_API mrb_value mrb_undef_value_ex(void)
{
  mrb_value v;
  SET_UNDEF_VALUE(v);
  return v;
}

MRB_API mrb_value mrb_obj_value_ex(void *p)
{
  return mrb_obj_value(p);
}

MRB_API mrb_value mrb_cptr_value_ex(mrb_state *mrb, void *p)
{
  return mrb_cptr_value(mrb, p);
}

MRB_API void *mrb_ptr_ex(mrb_value o)
{
  return mrb_ptr(o);
}

MRB_API mrb_float mrb_float_ex(mrb_value o)
{
  return mrb_float(o);
}

MRB_API void *mrb_cptr_ex(mrb_value o)
{
  return mrb_cptr(o);
}

MRB_API mrb_int mrb_fixnum_ex(mrb_value o)
{
  return mrb_fixnum(o);
}

MRB_API mrb_sym mrb_symbol_ex(mrb_value o)
{
  return mrb_symbol(o);
}

MRB_API enum mrb_vtype mrb_type_ex(mrb_value o)
{
  return mrb_type(o);
}

MRB_API mrb_bool mrb_immediate_p_ex(mrb_value o)
{
  return mrb_immediate_p(o);
}

MRB_API mrb_bool mrb_fixnum_p_ex(mrb_value o)
{
  return mrb_fixnum_p(o);
}

MRB_API mrb_bool mrb_symbol_p_ex(mrb_value o)
{
  return mrb_symbol_p(o);
}

MRB_API mrb_bool mrb_undef_p_ex(mrb_value o)
{
  return mrb_undef_p(o);
}

MRB_API mrb_bool mrb_nil_p_ex(mrb_value o)
{
  return mrb_nil_p(o);
}

MRB_API mrb_bool mrb_false_p_ex(mrb_value o)
{
  return mrb_false_p(o);
}

MRB_API mrb_bool mrb_true_p_ex(mrb_value o)
{
  return mrb_true_p(o);
}

MRB_API mrb_bool mrb_float_p_ex(mrb_value o)
{
  return mrb_float_p(o);
}

MRB_API mrb_bool mrb_array_p_ex(mrb_value o)
{
  return mrb_array_p(o);
}

MRB_API mrb_bool mrb_string_p_ex(mrb_value o)
{
  return mrb_string_p(o);
}

MRB_API mrb_bool mrb_hash_p_ex(mrb_value o)
{
  return mrb_hash_p(o);
}

MRB_API mrb_bool mrb_cptr_p_ex(mrb_value o)
{
  return mrb_cptr_p(o);
}

MRB_API mrb_bool mrb_exception_p_ex(mrb_value o)
{
  return mrb_exception_p(o);
}

MRB_API mrb_bool mrb_free_p_ex(mrb_value o)
{
  return mrb_free_p(o);
}

MRB_API mrb_bool mrb_object_p_ex(mrb_value o)
{
  return mrb_object_p(o);
}

MRB_API mrb_bool mrb_class_p_ex(mrb_value o)
{
  return mrb_class_p(o);
}

MRB_API mrb_bool mrb_module_p_ex(mrb_value o)
{
  return mrb_module_p(o);
}

MRB_API mrb_bool mrb_iclass_p_ex(mrb_value o)
{
  return mrb_iclass_p(o);
}

MRB_API mrb_bool mrb_sclass_p_ex(mrb_value o)
{
  return mrb_sclass_p(o);
}

MRB_API mrb_bool mrb_proc_p_ex(mrb_value o)
{
  return mrb_proc_p(o);
}

MRB_API mrb_bool mrb_range_p_ex(mrb_value o)
{
  return mrb_range_p(o);
}

// MRB_API mrb_bool mrb_file_p_ex(mrb_value o)
// {
//   return mrb_file_p(o);
// }

MRB_API mrb_bool mrb_env_p_ex(mrb_value o)
{
  return mrb_env_p(o);
}

MRB_API mrb_bool mrb_data_p_ex(mrb_value o)
{
  return mrb_data_p(o);
}

MRB_API mrb_bool mrb_fiber_p_ex(mrb_value o)
{
  return mrb_fiber_p(o);
}

MRB_API mrb_bool mrb_istruct_p_ex(mrb_value o)
{
  return mrb_istruct_p(o);
}

MRB_API mrb_bool mrb_break_p_ex(mrb_value o)
{
  return mrb_break_p(o);
}

MRB_API mrb_bool mrb_bool_ex(mrb_value o)
{
  return mrb_bool(o);
}

MRB_API mrb_bool mrb_test_ex(mrb_value o)
{
  return mrb_test(o);
}

MRB_API void mrb_data_init_ex(mrb_value v, void *ptr, const mrb_data_type *type)
{
  mrb_data_init(v, ptr, type);
}

MRB_API struct RData *mrb_data_wrap_struct(mrb_state *mrb, struct RClass *klass, const mrb_data_type *type, void *ptr)
{
  return Data_Wrap_Struct(mrb, klass, type, ptr);
}

MRB_API mrb_value mrb_data_wrap_struct_obj(mrb_state *mrb, struct RClass *klass, const mrb_data_type *type, void *ptr)
{
  return mrb_obj_value(Data_Wrap_Struct(mrb, klass, type, ptr));
}

MRB_API void mrb_set_data_type(mrb_state *mrb, mrb_value v, const mrb_data_type *type)
{
  DATA_TYPE(v) = type;
}

MRB_API void mrb_set_data_ptr(mrb_state *mrb, mrb_value v, void *ptr)
{
  DATA_PTR(v) = ptr;
}

MRB_API void mrb_set_instance_tt(struct RClass *c, enum mrb_vtype tt)
{
  MRB_SET_INSTANCE_TT(c, tt);
}

MRB_API mrb_bool mrb_has_exc(mrb_state *mrb)
{
  if (mrb->exc)
  {
      return TRUE;
  }
  return FALSE;
}

MRB_API void mrb_exc_clear(mrb_state *mrb)
{
  if (mrb->exc)
  {
    mrb->exc = 0;
  }
}

MRB_API mrb_value mrb_get_exc_value(struct mrb_state *mrb)
{
  if (mrb->exc) {
    return mrb_obj_value(mrb->exc);
  }
  return mrb_nil_value();
}

MRB_API mrb_value mrb_exc_detail(struct mrb_state *mrb)
{
  if (!mrb->exc) {
    return mrb_nil_value();
  }

  mrb_value detail = mrb_funcall(mrb, mrb_obj_value(mrb->exc), "inspect", 0);
  return detail;
}

MRB_API int mrb_gc_arena_save_ex(mrb_state *mrb)
{
  return mrb_gc_arena_save(mrb);
}

MRB_API void mrb_gc_arena_restore_ex(mrb_state *mrb, int idx)
{
  mrb_gc_arena_restore(mrb, idx);
}

void
mrb_mruby_csharp_gem_init(mrb_state *mrb)
{
}

void
mrb_mruby_csharp_gem_final(mrb_state *mrb)
{
}
