MRuby::Gem::Specification.new('mruby-csharp') do |spec|
  spec.license = 'MIT'
  spec.authors = 'Kanbaru'
end

MRuby.each_target do
  next unless name.match(/^(windows|macOS|android)/i)

  sharedlib_ext =
    if RUBY_PLATFORM.match(/darwin/i)
      'dylib'
    elsif ENV['OS'] == 'Windows_NT'
      'dll'
    else
      'so'
    end
  
  mruby_sharedlib = "#{build_dir}/lib/libmruby.#{sharedlib_ext}"

  products << mruby_sharedlib

  file shared_lib: mruby_sharedlib

  task mruby_sharedlib => libmruby_static do |t|
    is_vc = primary_toolchain == 'visualcpp'
    is_mingw = ENV['OS'] == 'Windows_NT' && cc.command.start_with?('gcc')

    deffile = "#{File.dirname(__FILE__)}/mruby-sharp.def"

    flags = []
    flags_after_libraries = []
    
    if is_vc
      flags << '/DLL'
      flags << "/DEF:#{deffile}"
    else
      flags << '-shared'
      flags << '-fpic'
      if sharedlib_ext == 'dylib'
        flags << '-Wl,-force_load'
        # flags << '-install_name @rpath/libmruby.dylib'
      elsif is_mingw
        flags << deffile          
      else
        flags << '-Wl,--whole-archive'
        flags_after_libraries << '-Wl,--no-whole-archive'
      end
    end

    flags << "/MACHINE:#{ENV['Platform']}" if is_vc && ENV.include?('Platform')
    flags << libmruby_static
    flags += flags_after_libraries

    linker.run mruby_sharedlib, [], [], [], flags

    # tools = File.exapnd_path('../tools.rb', __FILE__)
    # sh "ruby #{tools} copy_to_uity #{build_dir}"
  end
end
