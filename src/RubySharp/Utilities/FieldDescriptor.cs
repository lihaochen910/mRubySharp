#if MRUBY
using System;
using System.Reflection;


namespace RubySharp {
	
	/// <summary>
	/// Class providing easier marshalling of get / set CLR fields
	/// </summary>
	public class FieldDescriptor {
		
		/// <summary>
		/// Gets the method information (can be a MethodInfo or ConstructorInfo)
		/// </summary>
		public FieldInfo FieldInfo { get; private set; }
		

		/// <summary>
		/// Gets a value indicating whether the described method is static.
		/// </summary>
		public bool IsStatic { get; private set; }

		/// <summary>
		/// Gets the name of the described method
		/// </summary>
		public string Name { get; private set; }

		public IntPtr DataTypePtr { get; set; }
		
		public FieldDescriptor( FieldInfo fieldInfo ) {
			this.FieldInfo = fieldInfo;
			IsStatic = fieldInfo.IsStatic;
			Name = fieldInfo.Name;
		}

		/// <summary>
		/// Gets a callback function as a delegate
		/// </summary>
		/// <param name="obj">The object (null for static).</param>
		/// <returns></returns>
		public Func< RubyState, object, CallbackArguments, R_VAL > GetGetCallback() {
			return ExecuteGet;
		}
		
		/// <summary>
		/// Gets a callback function as a delegate
		/// </summary>
		/// <param name="script">The script for which the callback must be generated.</param>
		/// <param name="obj">The object (null for static).</param>
		/// <returns></returns>
		public Func< RubyState, object, CallbackArguments, R_VAL > GetSetCallback() {
			return ExecuteSet;
		}


		/// <summary>
		/// Gets the callback function.
		/// </summary>
		/// <param name="script">The script for which the callback must be generated.</param>
		/// <param name="obj">The object (null for static).</param>
		/// <returns></returns>
		public CallbackFunction GetGetCallbackFunction() {
			return new CallbackFunction( GetGetCallback(), Name );
		}
		
		
		/// <summary>
		/// Gets the callback function.
		/// </summary>
		/// <param name="obj">The object (null for static).</param>
		/// <returns></returns>
		public CallbackFunction GetSetCallbackFunction() {
			return new CallbackFunction( GetSetCallback(), Name );
		}

		
		/// <summary>
		/// Builds the return value of a call
		/// </summary>
		/// <param name="outParams">The out parameters indices, or null. See <see cref="BuildArgumentList" />.</param>
		/// <param name="pars">The parameters passed to the function.</param>
		/// <param name="retv">The return value from the function. Use DynValue.Void if the function returned no value.</param>
		/// <returns>A DynValue to be returned to scripts</returns>
		protected static R_VAL BuildReturnValue( RubyState state, object retv ) {
			return RubyState.ObjectToValue( state, retv );
		}

		/// <summary>
		/// The internal callback which actually executes the method
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public R_VAL ExecuteGet( RubyState state, object obj, CallbackArguments args ) {
			return RubyState.ObjectToValue( state, FieldInfo.GetValue( obj ) );
		}
		
		public R_VAL ExecuteSet( RubyState state, object obj, CallbackArguments args ) {
			var arg = args.RawGet( 0, false );
			FieldInfo.SetValue( obj, RubyState.ValueToObjectOfType( state, arg, DataTypePtr, FieldInfo.FieldType, FieldInfo.FieldType, false ) );
			return R_VAL.NIL;
		}
		
	}
	
}
#endif