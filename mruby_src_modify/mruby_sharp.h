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
MRB_API mrb_value mrb_cptr_value_ex(struct mrb_state*, void*);

MRB_API void *mrb_ptr_ex(mrb_value o);
MRB_API void *mrb_cptr_ex(mrb_value o);
MRB_API mrb_float mrb_float_ex(mrb_value o);
MRB_API mrb_int mrb_fixnum_ex(mrb_value o);
MRB_API mrb_sym mrb_symbol_ex(mrb_value o);
MRB_API enum mrb_vtype mrb_type_ex(mrb_value o);

MRB_API mrb_bool mrb_immediate_p_ex(mrb_value o);
MRB_API mrb_bool mrb_fixnum_p_ex(mrb_value o);
MRB_API mrb_bool mrb_symbol_p_ex(mrb_value o);
MRB_API mrb_bool mrb_undef_p_ex(mrb_value o);
MRB_API mrb_bool mrb_nil_p_ex(mrb_value o);
MRB_API mrb_bool mrb_false_p_ex(mrb_value o);
MRB_API mrb_bool mrb_true_p_ex(mrb_value o);
MRB_API mrb_bool mrb_float_p_ex(mrb_value o);
MRB_API mrb_bool mrb_array_p_ex(mrb_value o);
MRB_API mrb_bool mrb_string_p_ex(mrb_value o);
MRB_API mrb_bool mrb_hash_p_ex(mrb_value o);
MRB_API mrb_bool mrb_cptr_p_ex(mrb_value o);
MRB_API mrb_bool mrb_exception_p_ex(mrb_value o);
MRB_API mrb_bool mrb_free_p_ex(mrb_value o);
MRB_API mrb_bool mrb_object_p_ex(mrb_value o);
MRB_API mrb_bool mrb_class_p_ex(mrb_value o);
MRB_API mrb_bool mrb_module_p_ex(mrb_value o);
MRB_API mrb_bool mrb_iclass_p_ex(mrb_value o);
MRB_API mrb_bool mrb_sclass_p_ex(mrb_value o);
MRB_API mrb_bool mrb_proc_p_ex(mrb_value o);
MRB_API mrb_bool mrb_range_p_ex(mrb_value o);
// MRB_API mrb_bool mrb_file_p_ex(mrb_value o);
MRB_API mrb_bool mrb_env_p_ex(mrb_value o);
MRB_API mrb_bool mrb_data_p_ex(mrb_value o);
MRB_API mrb_bool mrb_fiber_p_ex(mrb_value o);
MRB_API mrb_bool mrb_istruct_p_ex(mrb_value o);
MRB_API mrb_bool mrb_break_p_ex(mrb_value o);
MRB_API mrb_bool mrb_bool_ex(mrb_value o);
MRB_API mrb_bool mrb_test_ex(mrb_value o);

/*
 * data.h export
 */
MRB_API void mrb_data_init_ex(mrb_value v, void *ptr, const mrb_data_type *type);
MRB_API struct RData *mrb_data_wrap_struct(mrb_state *mrb, struct RClass *klass, const mrb_data_type *type, void *ptr);
MRB_API mrb_value mrb_data_wrap_struct_obj(mrb_state *mrb, struct RClass *klass, const mrb_data_type *type, void *ptr);

/*
 * class.h export
 */
MRB_API void mrb_set_instance_tt(struct RClass*, enum mrb_vtype);


/*
 * Exception
 */
MRB_API mrb_bool mrb_has_exc(mrb_state*);
MRB_API void mrb_exc_clear(mrb_state *);
MRB_API mrb_value mrb_exc_detail(mrb_state*);

/*
 * GC export
 */
MRB_API int mrb_gc_arena_save_ex(mrb_state *mrb);
MRB_API void mrb_gc_arena_restore_ex(mrb_state *mrb, int idx);

MRB_END_DECL

#endif /* MRUBY_SHARP_H */