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
			SkipList<int> skipList = new SkipList<int>();
			skipList.Insert(10);
			skipList.Insert(11);
			skipList.Insert(12);

			var enumerator = skipList.GetEnumerator();
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(10, enumerator.Current);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(11, enumerator.Current);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(12, enumerator.Current);
		}

		[Test]
		[Repeat(100)]
		public void testInsertAddedLower()
		{
			SkipList<int> skipList = new SkipList<int>();
			skipList.Insert(10);
			skipList.Insert(12);
			skipList.Insert(13);
			skipList.Insert(11);

			var enumerator = skipList.GetEnumerator();
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(10, enumerator.Current);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(11, enumerator.Current);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(12, enumerator.Current);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(13, enumerator.Current);
		}

		[Test]
		[Repeat(100)]
		public void testInsertAddedLowerEqual()
		{
			SkipList<int> skipList = new SkipList<int>();
			skipList.Insert(10);
			skipList.Insert(11);
			skipList.Insert(12);
			skipList.Insert(11);

			var enumerator = skipList.GetEnumerator();
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(10, enumerator.Current);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(11, enumerator.Current);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(11, enumerator.Current);
			Assert.AreEqual(true, enumerator.MoveNext()); Assert.AreEqual(12, enumerator.Current);
		}

		[Test]
		[Repeat(100)]
		public void testContains()
		{
			SkipList<int> skipList = new SkipList<int>();
			skipList.Insert(10);
			skipList.Insert(11);
			skipList.Insert(12);
			skipList.Insert(11);

			Assert.AreEqual(true, skipList.Contains(12));
			Assert.AreEqual(false, skipList.Contains(13));
		}

		[Test]
		[Repeat(100)]
		public void testRemove()
		{
			SkipList<int> skipList = new SkipList<int>();
			skipList.Insert(10);
			skipList.Insert(11);
			skipList.Insert(12);
			skipList.Insert(11);

			skipList.Remove(12);

			Assert.AreEqual(true, skipList.Contains(11));
			Assert.AreEqual(false, skipList.Contains(12));
		}
	}
}
