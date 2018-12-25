#ifndef MRUBY_SHARP_H
#define MRUBY_SHARP_H

#include "mruby/value.h"
#include "mruby/data.h"
#include "mruby/class.h"

/**
 * MRuby Value definition functions and macros.
 */
MRB_BEGIN_DECL

/*
 * value.h export
 */
MRB_API mrb_value mrb_float_value_ex(struct mrb_state*, mrb_float);
MRB_API mrb_value mrb_fixnum_value_ex(mrb_int);
MRB_API mrb_value mrb_nil_value_ex(void);
MRB_API mrb_value mrb_false_value_ex(void);
MRB_API mrb_value mrb_true_value_ex(void);
MRB_API mrb_value mrb_bool_value_ex(mrb_bool);
MRB_API mrb_value mrb_undef_value_ex(void);
MRB_API mrb_value mrb_obj_value_ex(void*);

/*
 * data.h export
 */
MRB_API void mrb_data_init_ex(mrb_value v, void *ptr, const mrb_data_type *type);

/*
 * class.h export
 */
MRB_API void mrb_set_instance_tt(struct RClass*, enum mrb_vtype);


/*
 * Exception
 */
MRB_API mrb_bool mrb_has_exc(struct mrb_state*);

MRB_API mrb_value mrb_exc_detail(struct mrb_state*);



MRB_END_DECL

#endif /* MRUBY_SHARP_H */