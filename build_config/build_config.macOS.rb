# After creating libmruby.a, convert it to a shared library for Unity. See `mrbgem.rake`.

MRuby::CrossBuild.new("macos-arm64") do |conf|
  conf.toolchain :clang
  conf.gembox File.expand_path("../mruby-sharp", File.dirname(__FILE__))

  conf.disable_presym
  cc.defines = %w(MRB_NO_BOXING MRB_NO_PRESYM)
  cc.defines << %w(MRB_INT32)
  cc.defines << %w(MRB_USE_FLOAT32)
  cc.defines << %w(MRB_UTF8_STRING)
  cc.defines << %w(MRB_MAIN_PROFILE)
  # cc.defines << %w(MRB_NO_STDIO)
  cc.defines << %w(MRB_USE_DEBUG_HOOK)
  conf.cc.flags << '-arch arm64'
  conf.cc.flags << '-Os'
  conf.linker.flags << '-arch arm64'
end

MRuby::CrossBuild.new("macos-x64") do |conf|
  conf.toolchain :clang
  conf.gembox File.expand_path("../mruby-sharp", File.dirname(__FILE__))

  conf.disable_presym
  cc.defines = %w(MRB_NO_BOXING MRB_NO_PRESYM)
  cc.defines << %w(MRB_INT32)
  cc.defines << %w(MRB_USE_FLOAT32)
  cc.defines << %w(MRB_UTF8_STRING)
  cc.defines << %w(MRB_MAIN_PROFILE)
  # cc.defines << %w(MRB_NO_STDIO)
  cc.defines << %w(MRB_USE_DEBUG_HOOK)
  conf.cc.flags << '-arch x86_64'
  conf.linker.flags << '-arch x86_64'
end
