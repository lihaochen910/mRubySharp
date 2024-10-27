PLUGINS_DIR = File.dirname(File.expand_path('bin', __FILE__))
MRUBY_ROOT =File.expand_path('../../mruby', __FILE__)

PLATFORMS = {
  'windows-x64' => 'dll',
  'macos-arm64' => 'dylib',
  'macos-x64' => 'dylib',
  'ios-arm64' => 'a',
  'ios-x64' => 'a',
  # 'tvos-arm64' => 'a',
  # 'tvos-x64' => 'a',
  'visionos-arm64' => 'a',
  'visionos-x64' => 'a',
  'linux-x64' => 'so',
  'linux-arm64' => 'so',
  'android-x64' => 'so',
  'android-arm64' => 'so',
  'wasm' => 'a',
}

task :build, ['target'] do |t, args|
  # build_config_path = File.expand_path("build_config/build_config.#{args.target}.rb", __FILE__)
  
  # Dir.chdir(MRUBY_ROOT) do
  #   sh "MRUBY_CONFIG=#{build_config_path} rake"
  # end
  build_config_path = File.expand_path("build_config/build_config.windows.rb", File.dirname(__FILE__))

  puts "MRUBY_ROOT: #{MRUBY_ROOT}"
  puts "build_config_path: #{build_config_path}"

  Dir.chdir(MRUBY_ROOT) do
    if args.target == 'windows'
      sh "SET MRUBY_CONFIG=#{build_config_path}"
      sh "rake"
    else
      sh "MRUBY_CONFIG=#{build_config_path} rake"
    end
  end
end

task :sync, ['build_dir'] do |t, args|
  build_dir = File.expand_path(args.build_dir)

  dylibs = []
  Dir.foreach(build_dir) do |dir|
    ext = PLATFORMS[dir]
    next if ext.nil?

    src = File.join(build_dir, dir, 'lib', "libmruby.#{ext}")
    dst = File.join(PLUGINS_DIR, dir, "mruby.#{ext}")
    FileUtils.cp src, dst, verbose: true

    if ext == 'dylib'
      sh %Q{codesign --sign - --force #{dst}}
      dylibs << dst
    end
  end

  if dylibs.any?
    universal_dylib = File.join(PLUGINS_DIR, 'macos-universal', "VitalRouter.MRUby.Native.dylib")
    sh %Q{lipo -create #{dylibs.join(' ')} -output #{universal_dylib}}
    sh %Q{codesign --sign - --force #{universal_dylib}}
  end
end
