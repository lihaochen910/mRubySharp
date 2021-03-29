﻿namespace RubySharp {
	
	using System.Collections.Generic;

	/// <summary>
	/// This class is a container for arguments received by a CallbackFunction
	/// </summary>
	public class CallbackArguments {
		IList< R_VAL > m_Args;
		int m_Count;
		// bool m_LastIsTuple = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="CallbackArguments" /> class.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <param name="isMethodCall">if set to <c>true</c> [is method call].</param>
		public CallbackArguments ( IList< R_VAL > args ) {
			m_Args = args;

			if ( m_Args.Count > 0 ) {
				// var last = m_Args[ m_Args.Count - 1 ];
				//
				// if ( last.Type == DataType.Tuple ) {
				// 	m_Count       = last.Tuple.Length - 1 + m_Args.Count;
				// 	m_LastIsTuple = true;
				// }
				// else if ( last.Type == DataType.Void ) {
				// 	m_Count = m_Args.Count - 1;
				// }
				// else {
				// 	m_Count = m_Args.Count;
				// }
				
				m_Count = m_Args.Count;
			}
			else {
				m_Count = 0;
			}

			// IsMethodCall = isMethodCall;
		}

		/// <summary>
		/// Gets the count of arguments
		/// </summary>
		public int Count {
			get { return m_Count; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this is a method call.
		/// </summary>
		public bool IsMethodCall { get; private set; }


		/// <summary>
		/// Gets the <see cref="DynValue"/> at the specified index, or Void if not found 
		/// </summary>
		public R_VAL this [ int index ] {
			get { return RawGet ( index, true ); }
		}

		/// <summary>
		/// Gets the <see cref="DynValue" /> at the specified index, or null.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="translateVoids">if set to <c>true</c> all voids are translated to nils.</param>
		/// <returns></returns>
		public R_VAL RawGet ( int index, bool translateVoids ) {
			R_VAL v;

			if ( index >= m_Count ) {
				return R_VAL.NIL;
			}

			// if ( !m_LastIsTuple || index < m_Args.Count - 1 )
			// 	v = m_Args[ index ];
			// else
			// 	v = m_Args[ m_Args.Count - 1 ].Tuple[ index - ( m_Args.Count - 1 ) ];

			v = m_Args[ index ];

			// if ( v.Type == DataType.Tuple ) {
			// 	if ( v.Tuple.Length > 0 )
			// 		v = v.Tuple[ 0 ];
			// 	else
			// 		v = DynValue.Nil;
			// }

			// if ( translateVoids && v.Type == DataType.Void ) {
			// 	v = DynValue.Nil;
			// }

			return v;
		}


		/// <summary>
		/// Converts the arguments to an array
		/// </summary>
		/// <param name="skip">The number of elements to skip (default= 0).</param>
		/// <returns></returns>
		public R_VAL[] GetArray ( int skip = 0 ) {
			if ( skip >= m_Count )
				return new R_VAL[0];

			R_VAL[] vals = new R_VAL[m_Count - skip];

			for ( int i = skip; i < m_Count; i++ )
				vals[ i - skip ] = this[ i ];

			return vals;
		}

		/// <summary>
		/// Gets the specified argument as as an argument of the specified type. If not possible,
		/// an exception is raised.
		/// </summary>
		/// <param name="argIdx">The argument number.</param>
		/// <param name="funcName">Name of the function.</param>
		/// <param name="type">The type desired.</param>
		/// <param name="allowNil">if set to <c>true</c> nil values are allowed.</param>
		/// <returns></returns>
		// public R_VAL AsType ( int argIdx, string funcName, mrb_vtype type, bool allowNil = false ) {
		// 	return this[ argIdx ].CheckType ( funcName, type, argIdx,
		// 		allowNil
		// 			? TypeValidationFlags.AllowNil | TypeValidationFlags.AutoConvert
		// 			: TypeValidationFlags.AutoConvert );
		// }

		/// <summary>
		/// Gets the specified argument as as an argument of the specified user data type. If not possible,
		/// an exception is raised.
		/// </summary>
		/// <typeparam name="T">The desired userdata type</typeparam>
		/// <param name="argIdx">The argument number.</param>
		/// <param name="funcName">Name of the function.</param>
		/// <param name="allowNil">if set to <c>true</c> nil values are allowed.</param>
		/// <returns></returns>
		// public T AsUserData< T > ( int argIdx, string funcName, bool allowNil = false ) {
		// 	return this[ argIdx ].CheckUserDataType< T > ( funcName, argIdx,
		// 		allowNil ? TypeValidationFlags.AllowNil : TypeValidationFlags.None );
		// }

		/// <summary>
		/// Gets the specified argument as an integer
		/// </summary>
		/// <param name="argIdx">The argument number.</param>
		/// <param name="funcName">Name of the function.</param>
		/// <returns></returns>
		// public int AsInt ( int argIdx, string funcName ) {
		// 	R_VAL v = AsType ( argIdx, funcName, mrb_vtype.MRB_TT_FIXNUM, false );
		// 	return v.value.i;
		// }

		/// <summary>
		/// Gets the specified argument as a long integer
		/// </summary>
		/// <param name="argIdx">The argument number.</param>
		/// <param name="funcName">Name of the function.</param>
		/// <returns></returns>
		// public long AsLong ( int argIdx, string funcName ) {
		// 	R_VAL v = AsType ( argIdx, funcName, mrb_vtype.MRB_TT_FIXNUM, false );
		// 	return ( long )v.value.i;
		// }


		/// <summary>
		/// Gets the specified argument as a string, calling the __tostring metamethod if needed, in a NON
		/// yield-compatible way.
		/// </summary>
		/// <param name="executionContext">The execution context.</param>
		/// <param name="argNum">The argument number.</param>
		/// <param name="funcName">Name of the function.</param>
		/// <returns></returns>
		/// <exception cref="ScriptRuntimeException">'tostring' must return a string to '{0}'</exception>
		// public string AsStringUsingMeta ( ScriptExecutionContext executionContext, int argNum, string funcName ) {
		// 	if ( ( this[ argNum ].Type == DataType.Table ) && ( this[ argNum ].Table.MetaTable != null ) &&
		// 	     ( this[ argNum ].Table.MetaTable.RawGet ( "__tostring" ) != null ) ) {
		// 		var v = executionContext.GetScript ()
		// 		                        .Call ( this[ argNum ].Table.MetaTable.RawGet ( "__tostring" ),
		// 			                        this[ argNum ] );
		//
		// 		if ( v.Type != DataType.String )
		// 			throw new ScriptRuntimeException ( "'tostring' must return a string to '{0}'", funcName );
		//
		// 		return v.ToPrintString ();
		// 	}
		// 	else {
		// 		return ( this[ argNum ].ToPrintString () );
		// 	}
		// }


		/// <summary>
		/// Returns a copy of CallbackArguments where the first ("self") argument is skipped if this was a method call,
		/// otherwise returns itself.
		/// </summary>
		/// <returns></returns>
		public CallbackArguments SkipMethodCall () {
			if ( this.IsMethodCall ) {
				// Slice< DynValue > slice = new Slice< DynValue > ( m_Args, 1, m_Args.Count - 1, false );
				// return new CallbackArguments ( m_Args, false );
				return new CallbackArguments ( m_Args );
			}
			else return this;
		}
	}
}
