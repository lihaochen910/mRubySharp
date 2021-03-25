using System;

namespace CandyFramework.mRuby
{
	internal static class ValueToClrConversions
	{
		internal const int WEIGHT_MAX_VALUE = 100;
		internal const int WEIGHT_CUSTOM_CONVERTER_MATCH = 100;
		internal const int WEIGHT_EXACT_MATCH = 100;
		internal const int WEIGHT_STRING_TO_STRINGBUILDER = 99;
		internal const int WEIGHT_STRING_TO_CHAR = 98;
		internal const int WEIGHT_NIL_TO_NULLABLE = 100;
		internal const int WEIGHT_NIL_TO_REFTYPE = 100;
		internal const int WEIGHT_VOID_WITH_DEFAULT = 50;
		internal const int WEIGHT_VOID_WITHOUT_DEFAULT = 25;
		internal const int WEIGHT_NIL_WITH_DEFAULT = 25;
		internal const int WEIGHT_BOOL_TO_STRING = 5;
		internal const int WEIGHT_NUMBER_TO_STRING = 50;
		internal const int WEIGHT_NUMBER_TO_ENUM = 90;
		internal const int WEIGHT_USERDATA_TO_STRING = 5;
		internal const int WEIGHT_TABLE_CONVERSION = 90;
		internal const int WEIGHT_NUMBER_DOWNCAST = 99;
		internal const int WEIGHT_NO_MATCH = 0;
		internal const int WEIGHT_NO_EXTRA_PARAMS_BONUS = 100;
		internal const int WEIGHT_EXTRA_PARAMS_MALUS = 2;
		internal const int WEIGHT_BYREF_BONUSMALUS = -10;
		internal const int WEIGHT_VARARGS_MALUS = 1;
		internal const int WEIGHT_VARARGS_EMPTY = 40;

		/// <summary>
		/// Converts a R_VAL to a CLR object [simple conversion]
		/// </summary>
		internal static object ValueToObject ( RubyState state, R_VAL value ) {
			if ( R_VAL.IsNil ( value ) ) {
				return null;
			}

			switch ( value.tt ) {
				case mrb_vtype.MRB_TT_FALSE:
					return false;
				case mrb_vtype.MRB_TT_TRUE:
					return true;
				case mrb_vtype.MRB_TT_FIXNUM:
					return ( int )RubyDLL.mrb_fixnum ( value );
				case mrb_vtype.MRB_TT_SYMBOL:
					return RubyDLL.mrb_symbol ( value );
				case mrb_vtype.MRB_TT_UNDEF:
					return null;
				case mrb_vtype.MRB_TT_FLOAT:
					return ( float )RubyDLL.mrb_float ( value );
				case mrb_vtype.MRB_TT_STRING:
					return value.ToString ( state );
				case mrb_vtype.MRB_TT_CPTR:
					return RubyDLL.mrb_cptr ( value );
				case mrb_vtype.MRB_TT_OBJECT:
				case mrb_vtype.MRB_TT_CLASS:
				case mrb_vtype.MRB_TT_MODULE:
				case mrb_vtype.MRB_TT_ICLASS:
				case mrb_vtype.MRB_TT_SCLASS:
				case mrb_vtype.MRB_TT_PROC:
				case mrb_vtype.MRB_TT_RANGE:
				case mrb_vtype.MRB_TT_EXCEPTION:
				case mrb_vtype.MRB_TT_FILE:
				case mrb_vtype.MRB_TT_ENV:
				case mrb_vtype.MRB_TT_FIBER:
				case mrb_vtype.MRB_TT_ISTRUCT:
				case mrb_vtype.MRB_TT_BREAK:
				case mrb_vtype.MRB_TT_MAXDEFINE:
					return RubyDLL.mrb_ptr ( value );
				case mrb_vtype.MRB_TT_ARRAY:
				case mrb_vtype.MRB_TT_HASH:
					// TODO:
					return RubyDLL.mrb_ptr ( value );
				case mrb_vtype.MRB_TT_DATA:
					return RubyDLL.IntPtrToObject ( RubyDLL.mrb_data_get_ptr ( state, value,
						RubyState.DATA_TYPE_PTR ) );
				default:
					return null;
			}
		}


		/// <summary>
		/// Converts a R_VAL to a CLR object of a specific type
		/// </summary>
		internal static object ValueToObjectOfType ( RubyState state,        R_VAL value, Type desiredType,
		                                             object     defaultValue, bool      isOptional ) {
			if ( desiredType.IsByRef ) {
				desiredType = desiredType.GetElementType ();
			}

			if ( desiredType == typeof ( R_VAL ) ) {
				return value;
			}

			if ( desiredType == typeof ( object ) ) {
				return ValueToObject ( state, value );
			}

			StringConversions.StringSubtype stringSubType = StringConversions.GetStringSubtype ( desiredType );
			string                          str           = null;

			Type nt           = Nullable.GetUnderlyingType ( desiredType );
			Type nullableType = null;

			if ( nt != null ) {
				nullableType = desiredType;
				desiredType  = nt;
			}

			switch ( value.tt ) {
				case DataType.Void:
					if ( isOptional )
						return defaultValue;
					else if ( ( !desiredType.IsValueType ) || ( nullableType != null ) )
						return null;
					break;
				case DataType.Nil:
					if ( Framework.Do.IsValueType ( desiredType ) ) {
						if ( nullableType != null )
							return null;

						if ( isOptional )
							return defaultValue;
					}
					else {
						return null;
					}

					break;
				case DataType.Boolean:
					if ( desiredType == typeof ( bool ) )
						return value.Boolean;
					if ( stringSubType != StringConversions.StringSubtype.None )
						str = value.Boolean.ToString ();
					break;
				case DataType.Number:
					if ( Framework.Do.IsEnum ( desiredType ) ) {
						// number to enum conv
						Type underType = Enum.GetUnderlyingType ( desiredType );
						return NumericConversions.DoubleToType ( underType, value.Number );
					}

					if ( NumericConversions.NumericTypes.Contains ( desiredType ) )
						return NumericConversions.DoubleToType ( desiredType, value.Number );
					if ( stringSubType != StringConversions.StringSubtype.None )
						str = value.Number.ToString ();
					break;
				case DataType.String:
					if ( stringSubType != StringConversions.StringSubtype.None )
						str = value.String;
					break;
				case DataType.Function:
					if ( desiredType == typeof ( Closure ) ) return value.Function;
					else if ( desiredType == typeof ( ScriptFunctionDelegate ) ) return value.Function.GetDelegate ();
					break;
				case DataType.ClrFunction:
					if ( desiredType == typeof ( CallbackFunction ) ) return value.Callback;
					else if ( desiredType == typeof ( Func< ScriptExecutionContext, CallbackArguments, R_VAL > ) )
						return value.Callback.ClrCallback;
					break;
				case DataType.UserData:
					if ( value.UserData.Object != null ) {
						var udObj  = value.UserData.Object;
						var udDesc = value.UserData.Descriptor;

						if ( udDesc.IsTypeCompatible ( desiredType, udObj ) )
							return udObj;

						if ( stringSubType != StringConversions.StringSubtype.None )
							str = udDesc.AsString ( udObj );
					}

					break;
				case DataType.Table:
					if ( desiredType == typeof ( Table ) ||
					     Framework.Do.IsAssignableFrom ( desiredType, typeof ( Table ) ) )
						return value.Table;
					else {
						object o = TableConversions.ConvertTableToType ( value.Table, desiredType );
						if ( o != null )
							return o;
					}

					break;
				case DataType.Tuple:
					break;
			}

			if ( stringSubType != StringConversions.StringSubtype.None && str != null )
				return StringConversions.ConvertString ( stringSubType, str, desiredType, value.Type );

			throw ScriptRuntimeException.ConvertObjectFailed ( value.Type, desiredType );
		}

	}
}
