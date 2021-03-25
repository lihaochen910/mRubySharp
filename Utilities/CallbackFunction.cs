#if MRUBY
namespace RubySharp {
	
	using System;
	
	/// <summary>
	/// This class wraps a CLR function 
	/// </summary>
	public sealed class CallbackFunction {

		/// <summary>
		/// Gets the name of the function
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Instance Function Target
		/// </summary>
		public object Target { get; private set; }

		/// <summary>
		/// Gets the call back.
		/// </summary>
		/// <value>
		/// The call back.
		/// </value>
		public Func< RubyState, object, CallbackArguments, R_VAL > ClrCallback { get; private set; }
		public RubyDLL.RubyCSFunction RbCallback { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CallbackFunction" /> class.
		/// </summary>
		/// <param name="callBack">The callback function to be called.</param>
		/// <param name="name">The callback name, used in stacktraces, debugger, etc..</param>
		public CallbackFunction ( Func< RubyState, object, CallbackArguments, R_VAL > callBack, string name = null ) {
			ClrCallback = callBack;
			Name = name;
		}

		/// <summary>
		/// Set target
		/// </summary>
		/// <param name="target"></param>
		public CallbackFunction SetCallbackTarget ( object target ) {
			Target = target;
			return this;
		}

#if MRUBY
		/// <summary>
		/// Invokes the callback function
		/// </summary>
		/// <param name="executionContext">The execution context.</param>
		/// <param name="args">The arguments.</param>
		/// <param name="isMethodCall">if set to <c>true</c> this is a method call.</param>
		/// <returns></returns>
		public R_VAL Invoke ( RubyState state, R_VAL self ) {
#if DEBUG
			try {
#endif
				return ClrCallback ( state, Target, new CallbackArguments ( RubyDLL.GetFunctionArgs ( state ) ) );
#if DEBUG
			}
			catch ( Exception e ) {
				Console.WriteLine ( $"Exception on CallbackFunction::Invoke() {Name}" );
				throw;
			}
#endif
		}
#else
		public R_VAL Invoke ( RubyState state, int argc, R_VAL[] argv, R_VAL self ) {
#if DEBUG
			try {
#endif
#if MRUBY
				return ClrCallback ( state, Target, new CallbackArguments ( RubyDLL.GetFunctionArgs ( state ) ) );
#else
				return ClrCallback ( state, Target, new CallbackArguments ( argv ) );
#endif
#if DEBUG
			}
			catch ( Exception e ) {
				Console.WriteLine ( $"Exception on CallbackFunction::Invoke() {Name}" );
				throw;
			}
#endif
		}
#endif
		
		/// <summary>
		/// Creates a CallbackFunction from a delegate.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="del">The delegate.</param>
		/// <param name="accessMode">The access mode.</param>
		/// <returns></returns>
		public static CallbackFunction FromDelegate ( Delegate del ) {

#if NETFX_CORE
			MethodMemberDescriptor descr = new MethodMemberDescriptor ( del.GetMethodInfo () );
#else
			MethodMemberDescriptor descr = new MethodMemberDescriptor ( del.Method );
#endif
			return descr.GetCallbackFunction ().SetCallbackTarget ( del.Target );
		}


		/// <summary>
		/// Creates a CallbackFunction from a MethodInfo relative to a function.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="mi">The MethodInfo object.</param>
		/// <param name="obj">The object to which the function applies, or null for static methods.</param>
		/// <param name="accessMode">The access mode.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">The method is not static.</exception>
		public static CallbackFunction FromMethodInfo ( System.Reflection.MethodBase mi, object obj = null ) {
			MethodMemberDescriptor descr = new MethodMemberDescriptor ( mi );
			return descr.GetCallbackFunction ().SetCallbackTarget ( obj );
		}
		
		
		public static CallbackFunction FromFieldInfo_Get ( System.Reflection.FieldInfo field ) {
			FieldDescriptor descr = new FieldDescriptor ( field );
			return descr.GetGetCallbackFunction ();
		}
		
		public static CallbackFunction FromFieldInfo_Set ( System.Reflection.FieldInfo field ) {
			FieldDescriptor descr = new FieldDescriptor ( field );
			return !field.IsInitOnly ? descr.GetSetCallbackFunction () : null;
		}
		
		public static CallbackFunction FromPropertyInfo_Get ( System.Reflection.PropertyInfo property ) {
			PropertyDescriptor descr = new PropertyDescriptor ( property );
			UserDataUtility.TestPropertyCanAccess ( property, out var canGet, out var canSet );
			return canGet ? descr.GetGetCallbackFunction () : null;
		}
		
		public static CallbackFunction FromPropertyInfo_Set ( System.Reflection.PropertyInfo property ) {
			PropertyDescriptor descr = new PropertyDescriptor ( property );
			UserDataUtility.TestPropertyCanAccess ( property, out var canGet, out var canSet );
			return canSet ? descr.GetSetCallbackFunction () : null;
		}


		/// <summary>
		/// Gets or sets an object used as additional data to the callback function (available in the execution context).
		/// </summary>
		public object AdditionalData { get; set; }


		/// <summary>
		/// Checks the callback signature of a method is compatible for callbacks
		/// </summary>
		public static bool CheckCallbackSignature ( System.Reflection.MethodInfo mi, bool requirePublicVisibility ) {
			System.Reflection.ParameterInfo[] pi = mi.GetParameters ();

			return ( pi.Length == 2 && pi[ 0 ].ParameterType == typeof ( IntPtr )
			                        && pi[ 1 ].ParameterType == typeof ( R_VAL ) &&
			                        mi.ReturnType == typeof ( R_VAL ) &&
			                        ( requirePublicVisibility || mi.IsPublic ) );
		}
		
	}
}
#endif