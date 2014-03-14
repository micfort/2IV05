using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace CG_2IV05.Visualize
{
	public class OnDemand<T>
		where T:new()
	{
		private static ConcurrentQueue<T> _unused = new ConcurrentQueue<T>();
		private static int _maxUnused = 0;
		private static bool TIsIDisposable = false;

		public static int MaxUnused
		{
			get { return _maxUnused; }
			set { _maxUnused = value; }
		}

		static OnDemand()
		{
			TIsIDisposable = Array.Exists(typeof (T).GetInterfaces(), type => type == typeof (IDisposable));
		}

		public static T Create()
		{
			T item;
			if (_unused.TryDequeue(out item))
			{
				return item;
			}
			else
			{
				return new T();
			}
		}

		public static void Release(T o)
		{
			if(_unused.Count >= _maxUnused)
			{
				if(TIsIDisposable)
				{
					((IDisposable)o).Dispose();
				}
			}
			else
			{
				_unused.Enqueue(o);
			}
		}
	}
}
