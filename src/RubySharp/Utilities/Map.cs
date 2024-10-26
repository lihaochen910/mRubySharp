using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace RubySharp.Utilities {
	
	/// <summary>
	/// https://stackoverflow.com/questions/10966331/two-way-bidirectional-dictionary-in-c
	/// dict["SomeWord"] -> 123
	/// dict[123] -> "SomeWord"
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	public class Map< T1, T2 > : IEnumerable< KeyValuePair< T1, T2 > > {
		
		private readonly Dictionary< T1, T2 > _forward = new ();
		private readonly Dictionary< T2, T1 > _reverse = new ();


		public Map() {
			Forward = new Indexer< T1, T2 >( _forward );
			Reverse = new Indexer< T2, T1 >( _reverse );
		}


		public Indexer< T1, T2 > Forward { get; private set; }
		public Indexer< T2, T1 > Reverse { get; private set; }


		public void Add( T1 t1, T2 t2 ) {
			_forward.Add( t1, t2 );
			_reverse.Add( t2, t1 );
		}


		public void Remove( T1 t1 ) {
			T2 revKey = Forward[ t1 ];
			_forward.Remove( t1 );
			_reverse.Remove( revKey );
		}


		public void Remove( T2 t2 ) {
			T1 forwardKey = Reverse[ t2 ];
			_reverse.Remove( t2 );
			_forward.Remove( forwardKey );
		}


		public void Clear() {
			Forward.Clear();
			Reverse.Clear();
		}


		IEnumerator IEnumerable.GetEnumerator() {
			return GetForwardEnumerator();
		}

		
		public IEnumerator< KeyValuePair< T1, T2 > > GetEnumerator() {
			return _forward.GetEnumerator();
		}
		

		public IEnumerator< KeyValuePair< T1, T2 > > GetForwardEnumerator() {
			return _forward.GetEnumerator();
		}
		
		
		public IEnumerator< KeyValuePair< T2, T1 > > GetReverseEnumerator() {
			return _reverse.GetEnumerator();
		}


		public class Indexer< T3, T4 > {
			
			private readonly Dictionary< T3, T4 > _dictionary;
			
			public Indexer( Dictionary< T3, T4 > dictionary ) {
				_dictionary = dictionary;
			}


			public T4 this[ T3 index ] {
				[MethodImpl( MethodImplOptions.AggressiveInlining )]
				get => _dictionary[ index ];
				[MethodImpl( MethodImplOptions.AggressiveInlining )]
				set => _dictionary[ index ] = value;
			}


			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			public bool Contains( T3 key ) {
				return _dictionary.ContainsKey( key );
			}


			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			public bool TryGetValue( T3 key, out T4 value ) {
				return _dictionary.TryGetValue( key, out value );
			}


			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			public void Clear() {
				_dictionary.Clear();
			}
			
		}
		
	}

}
