using System;
using System.IO;
using System.Runtime.InteropServices;


namespace RubySharp {
	
    /// <summary>
    /// Rubyのポーティング。
    /// </summary>
    /// <remarks>
    /// 行き当たりばったりで機能を追加しています。
    /// </remarks>
    public static partial class RubyDLL {
		
        /// <summary>
        /// 使用平台特定API加载动态库
        /// </summary>
        [Obsolete]
		private static IntPtr RubyLibrary = IntPtr.Zero;

		[Obsolete]
		private static IntPtr GetNativeLibrary() {
			
			IntPtr ret = IntPtr.Zero;

			string rubyDLLStr =
#if MRUBY
				__DllName;
#else
				RubyDll;
#endif

			// Load bundled library
			string assemblyLocation = Path.GetDirectoryName( typeof( RubyDLL ).Assembly.Location );
			if ( CurrentPlatform.OS == OS.Windows && Environment.Is64BitProcess )
				ret = FuncLoader.LoadLibrary( Path.Combine( assemblyLocation, $"lib/windows/{rubyDLLStr}.dll" ) );
			else if ( CurrentPlatform.OS == OS.Windows && !Environment.Is64BitProcess )
				ret = FuncLoader.LoadLibrary( Path.Combine( assemblyLocation, $"lib/windows/{rubyDLLStr}.dll" ) );
			else if ( CurrentPlatform.OS == OS.Linux && Environment.Is64BitProcess )
				ret = FuncLoader.LoadLibrary( Path.Combine( assemblyLocation, $"lib/linux/x64/{rubyDLLStr}.so.0" ) );
			else if ( CurrentPlatform.OS == OS.Linux && !Environment.Is64BitProcess )
				ret = FuncLoader.LoadLibrary( Path.Combine( assemblyLocation, $"lib/linux/x86/{rubyDLLStr}.so.0" ) );
			else if ( CurrentPlatform.OS == OS.MacOSX )
				ret = FuncLoader.LoadLibrary( Path.Combine( assemblyLocation, $"lib/osx/{rubyDLLStr}.dylib" ) );

			// Load system library
			if ( ret == IntPtr.Zero ) {
				if ( CurrentPlatform.OS == OS.Windows )
					ret = FuncLoader.LoadLibrary( $"{rubyDLLStr}.dll" );
				else if ( CurrentPlatform.OS == OS.Linux )
					ret = FuncLoader.LoadLibrary( $"{rubyDLLStr}.so.0" );
				else
					ret = FuncLoader.LoadLibrary( $"{rubyDLLStr}.dylib" );
			}

			// Welp, all failed, PANIC!!!
			if ( ret == IntPtr.Zero ) {
				throw new Exception( "Failed to load ruby dynamic library." );
			}

			return ret;
		}
		
		/// <summary>
		/// Ruby スクリプトを解釈するときのエンコーディングを取得または設定します。
		/// </summary>
		public static System.Text.Encoding Encoding { get; set; } = System.Text.Encoding.UTF8;

		
		/// <summary>
		/// C 用に文字列を byte の配列に変換します。
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static byte[] ToCBytes( string str ) {
			return Encoding.GetBytes( str + '\0' );
		}
		
    }


    internal class FuncLoader {
		
	    private class Windows {
			
		    [DllImport( "kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true )]
		    public static extern IntPtr GetProcAddress( IntPtr hModule, string procName );

		    [DllImport( "kernel32", SetLastError = true, CharSet = CharSet.Unicode )]
		    public static extern IntPtr LoadLibraryW( string lpszLib );
			
	    }

	    private class Linux {
			
		    [DllImport( "libdl.so.2" )]
		    public static extern IntPtr dlopen( string path, int flags );

		    [DllImport( "libdl.so.2" )]
		    public static extern IntPtr dlsym( IntPtr handle, string symbol );
			
	    }

	    private class OSX {
			
		    [DllImport( "/usr/lib/libSystem.dylib" )]
		    public static extern IntPtr dlopen( string path, int flags );

		    [DllImport( "/usr/lib/libSystem.dylib" )]
		    public static extern IntPtr dlsym( IntPtr handle, string symbol );
			
	    }
		
		private class Android {

			[DllImport( "dl" )]
			public static extern IntPtr dlopen( string path, int flags );

			[DllImport( "dl" )]
			public static extern IntPtr dlsym( IntPtr handle, string symbol );

		}

	    private const int RTLD_LAZY = 0x0001;

	    public static IntPtr LoadLibrary( string libname ) {
		    if ( CurrentPlatform.OS == OS.Windows )
			    return Windows.LoadLibraryW( libname );

		    if ( CurrentPlatform.OS == OS.MacOSX )
			    return OSX.dlopen( libname, RTLD_LAZY );

		    return Linux.dlopen( libname, RTLD_LAZY );
	    }

	    public static T LoadFunction< T >( IntPtr library, string function, bool throwIfNotFound = true ) {
		    var ret = IntPtr.Zero;

		    if ( CurrentPlatform.OS == OS.Windows )
			    ret = Windows.GetProcAddress( library, function );
		    else if ( CurrentPlatform.OS == OS.MacOSX )
			    ret = OSX.dlsym( library, function );
		    else
			    ret = Linux.dlsym( library, function );

		    if ( ret == IntPtr.Zero ) {
				if ( throwIfNotFound ) {
					throw new EntryPointNotFoundException( function );
				}

			    return default( T );
		    }

#if NETSTANDARD
			return Marshal.GetDelegateForFunctionPointer< T >( ret );
#else
		    return ( T )( object )Marshal.GetDelegateForFunctionPointer( ret, typeof ( T ) );
#endif
	    }
    }


    internal enum OS {
		Windows,
		Linux,
		MacOSX,
		Unknown
	}

	internal static class CurrentPlatform {
		
		private static bool init = false;
		private static OS os;

		[DllImport ( "libc" )]
		static extern int uname( IntPtr buf );

		private static void Init() {
			if ( !init ) {
				PlatformID pid = Environment.OSVersion.Platform;

				switch ( pid ) {
					case PlatformID.Win32NT:
					case PlatformID.Win32S:
					case PlatformID.Win32Windows:
					case PlatformID.WinCE:
						os = OS.Windows;
						break;
					case PlatformID.MacOSX:
						os = OS.MacOSX;
						break;
					case PlatformID.Unix:
						os = OS.MacOSX;

						// Mac can return a value of Unix sometimes, We need to double check it.
						IntPtr buf = IntPtr.Zero;
						try {
							buf = Marshal.AllocHGlobal( 8192 );

							if ( uname( buf ) == 0 && Marshal.PtrToStringAnsi( buf ) == "Linux" ) {
								os = OS.Linux;
							}
						}
						catch {}
						finally {
							if ( buf != IntPtr.Zero ) {
								Marshal.FreeHGlobal( buf );
							}
						}
						
						break;
					default:
						os = OS.Unknown;
						break;
				}

				init = true;
			}
		}

		public static OS OS {
			get {
				Init();
				return os;
			}
		}


		public static string Rid {
			get {
				if ( CurrentPlatform.OS == OS.Windows && Environment.Is64BitProcess )
					return "win-x64";
				else if ( CurrentPlatform.OS == OS.Windows && !Environment.Is64BitProcess )
					return "win-x86";
				else if ( CurrentPlatform.OS == OS.Linux )
					return "linux-x64";
				else if ( CurrentPlatform.OS == OS.MacOSX )
					return "osx";
				else
					return "unknown";
			}
		}
		
	}
	
}
