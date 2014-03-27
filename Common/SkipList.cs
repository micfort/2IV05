using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CG_2IV05.Common
{
	public class SkipList<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
		where TKey: IComparable<TKey>
	{
		private SkipListItem<TKey, TValue> _head;
		private Random rand = new Random();
		private const float probability = 0.5f;
		private const int MaxLevels = 32;

		public SkipList()
		{
			_head = new SkipListItem<TKey, TValue>(default(KeyValuePair<TKey, TValue>), MaxLevels);
		}

		public void Insert(TKey key, TValue value)
		{
			int levels = 1; //minimal 1 level (bottom level)
			while (rand.NextDouble() > probability && levels < MaxLevels)
			{
				levels++;
			}

			SkipListItem<TKey, TValue> newItem = new SkipListItem<TKey, TValue>(new KeyValuePair<TKey, TValue>(key, value),
				                                                                levels);
			var currentItem = _head;
			int curLevel = _head.LinksAfter.Count-1;
			while (curLevel >= 0)
			{
				if(currentItem.LinksAfter[curLevel].To == null || currentItem.LinksAfter[curLevel].To.Item.Key.CompareTo(key) >= 0)
				{
					if(curLevel < levels)
					{
						newItem.InstertAfterItem(currentItem, curLevel);
					}
					curLevel--;
				}
				else if (currentItem.LinksAfter[curLevel].To.Item.Key.CompareTo(key) < 0)
				{
					currentItem = currentItem.LinksAfter[curLevel].To;
				}
			}
		}

		public bool Remove(TKey key)
		{
			var item = FindItem(key);
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

		public void RemoveAll(Func<KeyValuePair<TKey, TValue>, bool> predicate)
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

		public bool Contains(TKey key)
		{
			var item = FindItem(key);
			if (item != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private SkipListItem<TKey, TValue> FindItem(TKey key)
		{
			var currentItem = _head;
			int curLevel = _head.LinksAfter.Count - 1;
			while (curLevel >= 0)
			{
				if(currentItem != _head && currentItem.Item.Key.CompareTo(key) == 0)
				{
					return currentItem;
				}
				else if (currentItem.LinksAfter[curLevel].To == null || currentItem.LinksAfter[curLevel].To.Item.Key.CompareTo(key) > 0)
				{
					curLevel--;
				}
				else if (currentItem.LinksAfter[curLevel].To.Item.Key.CompareTo(key) <= 0)
				{
					currentItem = currentItem.LinksAfter[curLevel].To;
				}
			}
			return curLevel >= 0 ? currentItem : null;
		}

		#region Implementation of IEnumerable

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return new SkipListEnumerator<TKey, TValue>(_head);
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

	public class SkipListItem<TKey, TValue>
		where TKey: IComparable<TKey>
	{
		public List<SkipListLink<TKey, TValue>> LinksBefore { get; set; }
		public List<SkipListLink<TKey, TValue>> LinksAfter { get; set; }
		public KeyValuePair<TKey, TValue> Item { get; set; }

		public bool Tail
		{
			get { return LinksAfter[0].To == null; }
		}

		public SkipListItem(KeyValuePair<TKey, TValue> item)
		{
			Item = item;
			LinksBefore = new List<SkipListLink<TKey, TValue>>();
			LinksAfter = new List<SkipListLink<TKey, TValue>>();
		}

		public SkipListItem(KeyValuePair<TKey, TValue> item, int levels)
		{
			Item = item;
			LinksBefore = new List<SkipListLink<TKey, TValue>>(levels);
			LinksAfter = new List<SkipListLink<TKey, TValue>>(levels);
			for (int i = 0; i < levels; i++)
			{
				LinksBefore.Add(new SkipListLink<TKey, TValue>(0, null, this));
				LinksAfter.Add(new SkipListLink<TKey, TValue>(0, this, null));
			}
		}

		public void InstertAfterItem(SkipListItem<TKey, TValue> beforeItem, int level)
		{
			//tempory hold reference
			SkipListItem<TKey, TValue> afterItem = beforeItem.LinksAfter[level].To;

			//update the link before
			beforeItem.LinksAfter[level].To = this;
			//update the link in the LinksBefore list
			LinksBefore[level] = beforeItem.LinksAfter[level];

			//create link for after this node
			LinksAfter[level] = new SkipListLink<TKey, TValue>(1, this, afterItem);
			if(afterItem != null) //update link in afternode if neccesary
				afterItem.LinksBefore[level] = LinksAfter[level];
		}

		public void RemoveFromList()
		{
			for (int i = 0; i < LinksBefore.Count; i++)
			{
				LinksBefore[i].From.LinksAfter[i].To = LinksAfter[i].To;
				if (LinksAfter[i].To != null) LinksAfter[i].To.LinksBefore[i].From = LinksBefore[i].From;
			}
		}
	}

	public class SkipListLink<TKey, TValue>
		where TKey : IComparable<TKey>
	{
		public int Distance { get; set; }
		public SkipListItem<TKey, TValue> From { get; set; }
		public SkipListItem<TKey, TValue> To { get; set; }

		public SkipListLink(int distance, SkipListItem<TKey, TValue> from, SkipListItem<TKey, TValue> to)
		{
			Distance = distance;
			From = @from;
			To = to;
		}
	}

	class SkipListEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
		where TKey : IComparable<TKey>
	{
		private readonly SkipListItem<TKey, TValue> _head;
		private SkipListItem<TKey, TValue> _currentItem;

		public SkipListEnumerator(SkipListItem<TKey, TValue> head)
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
		public KeyValuePair<TKey, TValue> Current
		{
			get { return _currentItem != null ? _currentItem.Item : default(KeyValuePair<TKey, TValue>); }
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

