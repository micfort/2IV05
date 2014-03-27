using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG_2IV05.Common;
using NUnit.Framework;

namespace CG_2IV05.Test
{
	[TestFixture]
	class SkipListTest
	{
		[Test]
		[Repeat(100)]
		public void testInsert()
		{
			SkipList<int, int> skipList = new SkipList<int, int>();
			skipList.Insert(10, 0);
			skipList.Insert(11, 0);
			skipList.Insert(12, 0);

			var enumerator = skipList.GetEnumerator();
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(10, enumerator.Current.Key);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(11, enumerator.Current.Key);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(12, enumerator.Current.Key);
		}

		[Test]
		[Repeat(100)]
		public void testInsertAddedLower()
		{
			SkipList<int, int> skipList = new SkipList<int, int>();
			skipList.Insert(10, 0);
			skipList.Insert(12, 0);
			skipList.Insert(13, 0);
			skipList.Insert(11, 0);

			var enumerator = skipList.GetEnumerator();
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(10, enumerator.Current.Key);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(11, enumerator.Current.Key);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(12, enumerator.Current.Key);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(13, enumerator.Current.Key);
		}

		[Test]
		[Repeat(100)]
		public void testInsertAddedLowerEqual()
		{
			SkipList<int, int> skipList = new SkipList<int, int>();
			skipList.Insert(10, 0);
			skipList.Insert(11, 0);
			skipList.Insert(12, 0);
			skipList.Insert(11, 0);

			var enumerator = skipList.GetEnumerator();
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(10, enumerator.Current.Key);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(11, enumerator.Current.Key);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(11, enumerator.Current.Key);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(12, enumerator.Current.Key);
		}

		[Test]
		[Repeat(100)]
		public void testContains()
		{
			SkipList<int, int> skipList = new SkipList<int, int>();
			skipList.Insert(10, 0);
			skipList.Insert(11, 0);
			skipList.Insert(12, 0);
			skipList.Insert(11, 0);

			Assert.AreEqual(true, skipList.Contains(12));
			Assert.AreEqual(false, skipList.Contains(13));
		}

		[Test]
		[Repeat(100)]
		public void testRemove()
		{
			SkipList<int, int> skipList = new SkipList<int, int>();
			skipList.Insert(10, 0);
			skipList.Insert(11, 0);
			skipList.Insert(12, 0);
			skipList.Insert(11, 0);

			skipList.Remove(12);

			Assert.AreEqual(true, skipList.Contains(11));
			Assert.AreEqual(false, skipList.Contains(12));
		}
	}
}
