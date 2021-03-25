set PATH=%PATH%;D:\Github\vcpkg\installed\x64-windows\bin
set INCLUDE=%INCLUDE%;D:\Github\vcpkg\installed\x64-windows\include
set LIB=%LIB%;D:\Github\vcpkg\installed\x64-windows\lib

d:
cd D:\ruby-3.0.0
win32\configure.bat --prefix=d:\ruby-3.0.0 --enable-shared --disable-win95 --disable-install-doc --disable-rubygems