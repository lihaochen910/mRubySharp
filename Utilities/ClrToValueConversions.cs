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
		internal static R_VAL TryObjectToSimpleValue ( RubyState state, object obj ) {

			if ( obj == null ) {
				return R_VAL.NIL;
			}

			if ( obj is R_VAL ) {
				return ( R_VAL )obj;
			}
			
			Type t = obj.GetType ();

			if ( obj is bool )
				return R_VAL.NewBoolean ( ( bool )obj );

			if ( obj is string || obj is StringBuilder || obj is char )
				return R_VAL.NewString ( obj.ToString () );

			if ( obj is Closure )
				return R_VAL.NewClosure ( ( Closure )obj );

			if ( NumericConversions.NumericTypes.Contains ( t ) )
				return R_VAL.NewNumber ( NumericConversions.TypeToDouble ( t, obj ) );

			if ( obj is Table )
				return R_VAL.NewTable ( ( Table )obj );

			if ( obj is CallbackFunction )
				return R_VAL.NewCallback ( ( CallbackFunction )obj );

			if ( obj is Delegate ) {
				Delegate d = ( Delegate )obj;


#if NETFX_CORE
				MethodInfo mi = d.GetMethodInfo();
#else
				MethodInfo mi = d.Method;
#endif

				if ( CallbackFunction.CheckCallbackSignature ( mi, false ) )
					return R_VAL.NewCallback ( ( Func< ScriptExecutionContext, CallbackArguments, R_VAL > )d );
			}

			return R_VAL.NIL;
		}


		/// <summary>
		/// Tries to convert a CLR object to a MoonSharp value, using more in-depth analysis
		/// </summary>
		internal static R_VAL ObjectToValue ( RubyState state, object obj ) {
			R_VAL v = TryObjectToSimpleValue ( script, obj );

			if ( v != null ) return v;

			v = UserData.Create ( obj );
			if ( v != null ) return v;

			if ( obj is Type )
				v = UserData.CreateStatic ( obj as Type );

			// unregistered enums go as integers
			if ( obj is Enum )
				return R_VAL.NewNumber (
					NumericConversions.TypeToDouble ( Enum.GetUnderlyingType ( obj.GetType () ), obj ) );

			if ( v != null ) return v;

			if ( obj is Delegate )
				return R_VAL.NewCallback ( CallbackFunction.FromDelegate ( script, ( Delegate )obj ) );

			if ( obj is MethodInfo ) {
				MethodInfo mi = ( MethodInfo )obj;

				if ( mi.IsStatic ) {
					return R_VAL.NewCallback ( CallbackFunction.FromMethodInfo ( script, mi ) );
				}
			}

			if ( obj is System.Collections.IList ) {
				Table t = TableConversions.ConvertIListToTable ( script, ( System.Collections.IList )obj );
				return R_VAL.NewTable ( t );
			}

			if ( obj is System.Collections.IDictionary ) {
				Table t = TableConversions.ConvertIDictionaryToTable ( script, ( System.Collections.IDictionary )obj );
				return R_VAL.NewTable ( t );
			}

			var enumerator = EnumerationToValue ( script, obj );
			if ( enumerator != null ) return enumerator;


			throw ScriptRuntimeException.ConvertObjectFailed ( obj );
		}

		/// <summary>
		/// Converts an IEnumerable or IEnumerator to a R_VAL
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		// public static R_VAL EnumerationToValue ( RubyState state, object obj ) {
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
