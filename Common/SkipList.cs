using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CG_2IV05.Common
{
	public class SkipList<T> : IEnumerable<T>
		where T: IComparable<T>
	{
		private SkipListItem<T> _head;
		private Random rand = new Random();
		private const float probability = 0.5f;
		private const int MaxLevels = 32;

		public SkipList()
		{
			_head = new SkipListItem<T>(default(T), MaxLevels);
		}

		public SkipListItem<T> Insert(T value)
		{
			int levels = 1; //minimal 1 level (bottom level)
			while (rand.NextDouble() > probability && levels < MaxLevels)
			{
				levels++;
			}

			SkipListItem<T> newItem = new SkipListItem<T>(value, levels);
			var currentItem = _head;
			int curLevel = _head.LinksAfter.Count-1;
			while (curLevel >= 0)
			{
				if (currentItem.LinksAfter[curLevel].To == null || currentItem.LinksAfter[curLevel].To.Item.CompareTo(value) >= 0)
				{
					if(curLevel < levels)
					{
						newItem.InstertAfterItem(currentItem, curLevel);
					}
					curLevel--;
				}
				else if (currentItem.LinksAfter[curLevel].To.Item.CompareTo(value) < 0)
				{
					currentItem = currentItem.LinksAfter[curLevel].To;
				}
			}
			newItem.InSkipList = true;
			return newItem;
		}

		public bool Remove(T value)
		{
			var item = FindItem(value);
			if (item != null)
			{
				item.RemoveFromList();
				return true;
			}
			else
			{
				return false;
			}
		}

		public void RemoveAll(Func<T, bool> predicate)
		{
			var currentItem = _head;
			var nextItem = currentItem.LinksAfter[0].To;
			while (!currentItem.Tail)
			{
				currentItem = nextItem;
				nextItem = currentItem.LinksAfter[0].To;
				if(predicate(currentItem.Item))
				{
					currentItem.RemoveFromList();
				}
			}
		}

		public bool Contains(T value)
		{
			var item = FindItem(value);
			if (item != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private SkipListItem<T> FindItem(T value)
		{
			var currentItem = _head;
			int curLevel = _head.LinksAfter.Count - 1;
			while (curLevel >= 0)
			{
				if(currentItem != _head && currentItem.Item.CompareTo(value) == 0)
				{
					return currentItem;
				}
				else if (currentItem.LinksAfter[curLevel].To == null || currentItem.LinksAfter[curLevel].To.Item.CompareTo(value) > 0)
				{
					curLevel--;
				}
				else if (currentItem.LinksAfter[curLevel].To.Item.CompareTo(value) <= 0)
				{
					currentItem = currentItem.LinksAfter[curLevel].To;
				}
			}
			return curLevel >= 0 ? currentItem : null;
		}

		public List<T> List { get { return new List<T>(this); } } 

		#region Implementation of IEnumerable

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<T> GetEnumerator()
		{
			return new SkipListEnumerator<T>(_head);
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}

	public class SkipListItem<T>
		where T: IComparable<T>
	{
		private bool _InSkipList = false;
		public bool InSkipList
		{
			get { return _InSkipList; }
			set { _InSkipList = value; }
		}

		public List<SkipListLink<T>> LinksBefore { get; set; }
		public List<SkipListLink<T>> LinksAfter { get; set; }
		public T Item { get; set; }

		public bool Tail
		{
			get { return LinksAfter[0].To == null; }
		}

		public SkipListItem(T item)
		{
			Item = item;
			LinksBefore = new List<SkipListLink<T>>();
			LinksAfter = new List<SkipListLink<T>>();
		}

		public SkipListItem(T item, int levels)
		{
			Item = item;
			LinksBefore = new List<SkipListLink<T>>(levels);
			LinksAfter = new List<SkipListLink<T>>(levels);
			for (int i = 0; i < levels; i++)
			{
				LinksBefore.Add(new SkipListLink<T>(0, null, this));
				LinksAfter.Add(new SkipListLink<T>(0, this, null));
			}
		}

		public void InstertAfterItem(SkipListItem<T> beforeItem, int level)
		{
			//tempory hold reference
			SkipListItem<T> afterItem = beforeItem.LinksAfter[level].To;

			//update the link before
			beforeItem.LinksAfter[level].To = this;
			//update the link in the LinksBefore list
			LinksBefore[level] = beforeItem.LinksAfter[level];

			//create link for after this node
			LinksAfter[level] = new SkipListLink<T>(1, this, afterItem);
			if(afterItem != null) //update link in afternode if neccesary
				afterItem.LinksBefore[level] = LinksAfter[level];
		}

		public void RemoveFromList()
		{
			if(_InSkipList)
			{
				for (int i = 0; i < LinksBefore.Count; i++)
				{
					LinksBefore[i].From.LinksAfter[i].To = LinksAfter[i].To;
					if (LinksAfter[i].To != null) LinksAfter[i].To.LinksBefore[i].From = LinksBefore[i].From;
				}
				_InSkipList = false;
			}
			else
			{
				throw new InvalidOperationException("item isn't in a list");
			}
		}
	}

	public class SkipListLink<T>
		where T : IComparable<T>
	{
		public int Distance { get; set; }
		public SkipListItem<T> From { get; set; }
		public SkipListItem<T> To { get; set; }

		public SkipListLink(int distance, SkipListItem<T> from, SkipListItem<T> to)
		{
			Distance = distance;
			From = @from;
			To = to;
		}
	}

	class SkipListEnumerator<T> : IEnumerator<T>
		where T : IComparable<T>
	{
		private readonly SkipListItem<T> _head;
		private SkipListItem<T> _currentItem;

		public SkipListEnumerator(SkipListItem<T> head)
		{
			_head = head;
			_currentItem = head;
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
		}

		#endregion

		#region Implementation of IEnumerator

		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
		public bool MoveNext()
		{
			_currentItem = _currentItem.LinksAfter[0].To;
			return _currentItem != null;
		}

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
		public void Reset()
		{
			_currentItem = _head;
		}

		/// <summary>
		/// Gets the element in the collection at the current position of the enumerator.
		/// </summary>
		/// <returns>
		/// The element in the collection at the current position of the enumerator.
		/// </returns>
		public T Current
		{
			get { return _currentItem != null ? _currentItem.Item : default(T); }
		}

		/// <summary>
		/// Gets the current element in the collection.
		/// </summary>
		/// <returns>
		/// The current element in the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		object IEnumerator.Current
		{
			get { return Current; }
		}

		#endregion
	}
}

