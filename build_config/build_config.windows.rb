# After creating libmruby.a, convert it to a shared library for Unity. See `mrbgem.rake`.
MRuby::CrossBuild.new("windows") do |conf|
  conf.toolchain
  conf.gembox File.expand_path("../mruby-sharp", File.dirname(__FILE__))
  conf.disable_presym
  cc.defines = %w(MRB_NO_BOXING MRB_NO_PRESYM)
  cc.defines << %w(MRB_INT32)
  cc.defines << %w(MRB_USE_FLOAT32)
  cc.defines << %w(MRB_UTF8_STRING)
  cc.defines << %w(MRB_MAIN_PROFILE)
  # cc.defines << %w(MRB_NO_STDIO)
  cc.defines << %w(MRB_USE_DEBUG_HOOK)
  cc.flags << '-Os'

  # Turn on `enable_debug` for better debugging
  conf.enable_debug
end
