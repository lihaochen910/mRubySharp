/*
Copyright (c) 2015-2017 topameng(topameng@qq.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace RubySharp {
	
	public class ObjectTranslator : IDisposable {
		
		// private class DelayGC {
		// 	public DelayGC ( int id, UnityEngine.Object obj, float time ) {
		// 		this.id = id;
		// 		this.time = time;
		// 		this.obj = obj;
		// 	}
		//
		//
		// 	public int id;
		// 	public UnityEngine.Object obj;
		// 	public float time;
		// }


		private class CompareObject : IEqualityComparer< object > {
			
			public new bool Equals( object x, object y ) {
				return object.ReferenceEquals( x, y );
			}
			
			public int GetHashCode( object obj ) {
				return RuntimeHelpers.GetHashCode( obj );
			}
			
		}


		public bool LogGC { get; set; } = true;

		
		public readonly Dictionary< object, int > objectsBackMap = new ( 257, new CompareObject () );

		public readonly RubyObjectPool objects = new ();
		// private List< DelayGC > gcList = new List< DelayGC > ();
		private Action< object, int > removeInvalidObject;

#if !MULTI_STATE
		private static ObjectTranslator _translator = null;
#endif


		public ObjectTranslator() {
			// LogGC = false;
#if !MULTI_STATE
			_translator = this;
#endif
			removeInvalidObject = RemoveObject;
		}


		public int AddObject( object obj ) {
			int index = objects.Add( obj );

			if ( !IsValueType( obj.GetType() ) ) {
				objectsBackMap[ obj ] = index;
			}

			return index;
		}


		public static ObjectTranslator Get( IntPtr mrbState ) {
#if !MULTI_STATE
			return _translator;
#else
            return RubyState.GetTranslator( mrbState );
#endif
		}


		//fixed 枚举唯一性问题（对象唯一，没有实现__eq操作符）
		void RemoveObject( object o, int udata ) {
			int index = -1;

			if ( objectsBackMap.TryGetValue( o, out index ) && index == udata ) {
				objectsBackMap.Remove( o );
			}
		}


		//lua gc一个对象(lua 库不再引用，但不代表c#没使用)
		public void RemoveObject( int udata ) {
			//只有lua gc才能移除
			object o = objects.Remove( udata );

			if ( o != null ) {
				if ( !IsValueType( o.GetType() ) ) {
					RemoveObject( o, udata );
				}

				if ( LogGC ) {
					Console.WriteLine( "gc object {0}, id {1}", o, udata );
				}
			}
		}


		public object GetObject( int udata ) {
			return objects.TryGetValue( udata );
		}


		//预删除，但不移除一个lua对象(移除id只能由gc完成)
		public void Destroy( int udata ) {
			object o = objects.Destroy( udata );

			if ( o != null ) {
				if ( !IsValueType( o.GetType() ) ) {
					RemoveObject( o, udata );
				}

				if ( LogGC ) {
					Console.WriteLine( "destroy object {0}, id {1}", o, udata );
				}
			}
		}


		//Unity Object 延迟删除
		// public void DelayDestroy ( int id, float time ) {
		// 	UnityEngine.Object obj = ( UnityEngine.Object )GetObject ( id );
		//
		// 	if ( obj != null ) {
		// 		gcList.Add ( new DelayGC ( id, obj, time ) );
		// 	}
		// }


		public bool Getudata( object o, out int index ) {
			index = -1;
			return objectsBackMap.TryGetValue( o, out index );
		}


		public void Destroyudata( int udata ) {
			objects.Destroy( udata );
		}


		public void SetBack( int index, object o ) {
			objects.Replace( index, o );
		}


		// bool RemoveFromGCList ( int id ) {
		// 	int index = gcList.FindIndex ( ( p ) => { return p.id == id; } );
		//
		// 	if ( index >= 0 ) {
		// 		gcList.RemoveAt ( index );
		// 		return true;
		// 	}
		//
		// 	return false;
		// }


		//延迟删除处理
		// void DestroyUnityObject ( int udata, UnityEngine.Object obj ) {
		// 	object o = objects.TryGetValue ( udata );
		//
		// 	if ( object.ReferenceEquals ( o, obj ) ) {
		// 		RemoveObject ( o, udata );
		//
		// 		//一定不能Remove, 因为GC还可能再来一次
		// 		objects.Destroy ( udata );
		//
		// 		if ( LogGC ) {
		// 			Console.WriteLine ( "destroy object {0}, id {1}", o, udata );
		// 		}
		// 	}
		//
		// 	UnityEngine.Object.Destroy ( obj );
		// }


		public void Collect() {
			// if ( gcList.Count == 0 ) {
			// 	return;
			// }
			//
			// float delta = Time.deltaTime;
			//
			// for ( int i = gcList.Count - 1; i >= 0; i-- ) {
			// 	float time = gcList[ i ].time - delta;
			//
			// 	if ( time <= 0 ) {
			// 		DestroyUnityObject ( gcList[ i ].id, gcList[ i ].obj );
			// 		gcList.RemoveAt ( i );
			// 	}
			// 	else {
			// 		gcList[ i ].time = time;
			// 	}
			// }
		}


		public void StepCollect() {
			objects.StepCollect( removeInvalidObject );
		}


		public void Dispose() {
			objectsBackMap.Clear();
			objects.Clear();

#if !MULTI_STATE
			_translator = null;
#endif
		}
		
		
		private static bool IsValueType( Type t ) {
			return !t.IsEnum && t.IsValueType;
		}
	}


	public class RubyObjectPool {
		
		class PoolNode {
			
			public int index;
			public object obj;
			
			public PoolNode( int index, object obj ) {
				this.index = index;
				this.obj = obj;
			}
		}


		private List< PoolNode > list;

		//同lua_ref策略，0作为一个回收链表头，不使用这个位置
		private PoolNode head = null;
		private int count = 0;
		private int collectStep = 2;
		private int collectedIndex = -1;


		public RubyObjectPool() {
			list = new List< PoolNode >( 512 );
			head = new PoolNode( 0, null );
			list.Add( head );
			// list.Add ( new PoolNode ( 1, null ) );
			count = list.Count;
		}


		public object this[ int i ] {
			get {
				if ( i > 0 && i < count ) {
					return list[ i ].obj;
				}

				return null;
			}
		}


		public void Clear() {
			list.Clear();
			head = null;
			count = 0;
		}


		public int Add( object obj ) {
			int pos = -1;

			if ( head.index != 0 ) {
				pos = head.index;
				list[ pos ].obj = obj;
				head.index = list[ pos ].index;
			}
			else {
				pos = list.Count;
				list.Add( new PoolNode( pos, obj ) );
				count = pos + 1;
			}

			return pos;
		}


		public object TryGetValue( int index ) {
			if ( index > 0 && index < count ) {
				return list[ index ].obj;
			}

			return null;
		}


		public object Remove( int pos ) {
			if ( pos > 0 && pos < count ) {
				object o = list[ pos ].obj;
				list[ pos ].obj = null;
				list[ pos ].index = head.index;
				head.index = pos;

				return o;
			}

			return null;
		}


		public object Destroy( int pos ) {
			if ( pos > 0 && pos < count ) {
				object o = list[ pos ].obj;
				list[ pos ].obj = null;
				return o;
			}

			return null;
		}


		public void StepCollect( Action< object, int > collectListener ) {
			++collectedIndex;
			for ( int i = 0; i < collectStep; ++i ) {
				collectedIndex += i;
				if ( collectedIndex >= count ) {
					collectedIndex = -1;
					return;
				}

				PoolNode node = list[ collectedIndex ];
				object o = node.obj;
				if ( o != null && o.Equals( null ) ) {
					node.obj = null;
					if ( collectListener != null ) {
						collectListener( o, collectedIndex );
					}
				}
			}
		}


		public object Replace( int pos, object o ) {
			if ( pos > 0 && pos < count ) {
				object obj = list[ pos ].obj;
				list[ pos ].obj = o;
				return obj;
			}

			return null;
		}
		
	}

}
