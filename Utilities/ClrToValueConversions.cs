using System;
using System.Reflection;
using System.Text;
using MoonSharp.Interpreter.Interop.RegistrationPolicies;


namespace CandyFramework.mRuby {
	internal static class ClrToValueConversions {

		/// <summary>
		/// Tries to convert a CLR object to a MoonSharp value, using "simple" logic.
		/// Does NOT throw on failure.
		/// </summary>
		internal static mrb_value TryObjectToSimpleValue ( mRubyState state, object obj ) {

			if ( obj == null ) {
				return mrb_value.NIL;
			}

			if ( obj is mrb_value ) {
				return ( mrb_value )obj;
			}
			
			Type t = obj.GetType ();

			if ( obj is bool )
				return mrb_value.NewBoolean ( ( bool )obj );

			if ( obj is string || obj is StringBuilder || obj is char )
				return mrb_value.NewString ( obj.ToString () );

			if ( obj is Closure )
				return mrb_value.NewClosure ( ( Closure )obj );

			if ( NumericConversions.NumericTypes.Contains ( t ) )
				return mrb_value.NewNumber ( NumericConversions.TypeToDouble ( t, obj ) );

			if ( obj is Table )
				return mrb_value.NewTable ( ( Table )obj );

			if ( obj is CallbackFunction )
				return mrb_value.NewCallback ( ( CallbackFunction )obj );

			if ( obj is Delegate ) {
				Delegate d = ( Delegate )obj;


#if NETFX_CORE
				MethodInfo mi = d.GetMethodInfo();
#else
				MethodInfo mi = d.Method;
#endif

				if ( CallbackFunction.CheckCallbackSignature ( mi, false ) )
					return mrb_value.NewCallback ( ( Func< ScriptExecutionContext, CallbackArguments, mrb_value > )d );
			}

			return mrb_value.NIL;
		}


		/// <summary>
		/// Tries to convert a CLR object to a MoonSharp value, using more in-depth analysis
		/// </summary>
		internal static mrb_value ObjectToValue ( mRubyState state, object obj ) {
			mrb_value v = TryObjectToSimpleValue ( script, obj );

			if ( v != null ) return v;

			v = UserData.Create ( obj );
			if ( v != null ) return v;

			if ( obj is Type )
				v = UserData.CreateStatic ( obj as Type );

			// unregistered enums go as integers
			if ( obj is Enum )
				return mrb_value.NewNumber (
					NumericConversions.TypeToDouble ( Enum.GetUnderlyingType ( obj.GetType () ), obj ) );

			if ( v != null ) return v;

			if ( obj is Delegate )
				return mrb_value.NewCallback ( CallbackFunction.FromDelegate ( script, ( Delegate )obj ) );

			if ( obj is MethodInfo ) {
				MethodInfo mi = ( MethodInfo )obj;

				if ( mi.IsStatic ) {
					return mrb_value.NewCallback ( CallbackFunction.FromMethodInfo ( script, mi ) );
				}
			}

			if ( obj is System.Collections.IList ) {
				Table t = TableConversions.ConvertIListToTable ( script, ( System.Collections.IList )obj );
				return mrb_value.NewTable ( t );
			}

			if ( obj is System.Collections.IDictionary ) {
				Table t = TableConversions.ConvertIDictionaryToTable ( script, ( System.Collections.IDictionary )obj );
				return mrb_value.NewTable ( t );
			}

			var enumerator = EnumerationToValue ( script, obj );
			if ( enumerator != null ) return enumerator;


			throw ScriptRuntimeException.ConvertObjectFailed ( obj );
		}

		/// <summary>
		/// Converts an IEnumerable or IEnumerator to a mrb_value
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		// public static mrb_value EnumerationToValue ( mRubyState state, object obj ) {
		// 	if ( obj is System.Collections.IEnumerable ) {
		// 		var enumer = ( System.Collections.IEnumerable )obj;
		// 		return EnumerableWrapper.ConvertIterator ( script, enumer.GetEnumerator () );
		// 	}
		//
		// 	if ( obj is System.Collections.IEnumerator ) {
		// 		var enumer = ( System.Collections.IEnumerator )obj;
		// 		return EnumerableWrapper.ConvertIterator ( script, enumer );
		// 	}
		//
		// 	return null;
		// }
	}
}
