#include <mruby.h>
#include <mruby/mruby_sharp.h>

//===================================
MRB_API mrb_value mrb_float_value_ex(mrb_state *mrb, mrb_float f)
{
  mrb_value v;
  (void) mrb;
  SET_FLOAT_VALUE(mrb, v, f);
  return v;
}

MRB_API mrb_value mrb_fixnum_value_ex(mrb_int i)
{
  mrb_value v;
  SET_INT_VALUE(v, i);
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

MRB_API void mrb_data_init_ex(mrb_value v, void *ptr, const mrb_data_type *type)
{
  mrb_data_init(v, ptr, type);
}

MRB_API void mrb_set_instance_tt(struct RClass *c, enum mrb_vtype tt)
{
  MRB_SET_INSTANCE_TT(c, tt);
}

MRB_API mrb_bool mrb_has_exc(struct mrb_state *mrb)
{
  if (mrb->exc)
  {
      return TRUE;
  }
  return FALSE;
}

MRB_API mrb_value mrb_exc_detail(struct mrb_state *mrb)
{
  if (!mrb->exc) {
    return mrb_nil_value();
  }

  mrb_value detail = mrb_funcall(mrb, mrb_obj_value(mrb->exc), "inspect", 0);
  return detail;
}
