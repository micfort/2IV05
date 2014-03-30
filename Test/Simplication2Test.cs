using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CG_2IV05.Common;
using CG_2IV05.Common.Element;
using NUnit.Framework;
using micfort.GHL.Math2;

namespace CG_2IV05.Test
{
	class TestElementFactory: IElementFactory
	{
		#region Implementation of IElementFactory

		public IElement ReadFromStream(Stream stream)
		{
			return new TestElement();
		}

		public int FactoryID { get { return FactoryIDs.TestID; } }
		public IElement Merge(List<IElement> elements)
		{
			return new TestElement()
				       {
					       _referencePoint = elements.Aggregate(new HyperPoint<float>(3), (point, element) => point + element.ReferencePoint) * (1f/elements.Count)
				       };
		}

		public bool CanMerge(List<IElement> elements)
		{
			return true;
		}

		public bool RemoveItem(IElement element, int height)
		{
			return false;
		}

		#endregion
	}
	class TestElement: IElement
	{
		public override string ToString()
		{
			return _referencePoint.X.ToString();
		}

		#region Implementation of IElement

		public bool _finalElement;

		public int _triangleCount = 1;

		public ScoreKey _score;

		public HyperPoint<float> _min;

		public HyperPoint<float> _max;

		public HyperPoint<float> _referencePoint;

		public bool FinalElement
		{
			get { return _finalElement; }
		}

		public int TriangleCount
		{
			get { return _triangleCount; }
		}

		public ScoreKey Score
		{
			get { return _score; }
		}

		public HyperPoint<float> Min
		{
			get { return _min; }
		}

		public HyperPoint<float> Max
		{
			get { return _max; }
		}

		public HyperPoint<float> ReferencePoint
		{
			get { return _referencePoint; }
		}

		public NodeData CreateData(HyperPoint<float> centerDataSet, TextureInfo textureInfo)
		{
			return new NodeData()
				       {
					       Indexes = new int[0],
						   Normals = new HyperPoint<float>[0],
						   Vertices = new HyperPoint<float>[0],
						   TextCoord = new HyperPoint<float>[0],
				       };
		}

		public IElement GetSimplifiedVersion(int height)
		{
			return this;
		}

		public void SaveToStream(Stream stream)
		{
			return;
		}

		public int FactoryID { get { return FactoryIDs.TestID; } }

		#endregion
	}

	[TestFixture]
	class Simplication2Test
	{
		private Simplification2 simplification2;
		private Simplification2.Item item1;
		private Simplification2.Item item2;
		private Simplification2.Item item3;
		private Simplification2.Item item4;
		private Simplification2.Item newItem;
		private List<Simplification2.Item> items;
		private SkipList<Simplification2.Combination> closestElementsList;
		private Dictionary<int, List<Simplification2.Item>> itemsLists;

		[SetUp]
		public void setup()
		{
			
			simplification2 = new Simplification2();
			TestElement element1 = new TestElement()
			{
				_referencePoint = new HyperPoint<float>(0, 0, 0)
			};
			TestElement element2 = new TestElement()
			{
				_referencePoint = new HyperPoint<float>(10, 0, 0)
			};
			TestElement element3 = new TestElement()
			{
				_referencePoint = new HyperPoint<float>(30, 0, 0)
			};
			TestElement element4 = new TestElement()
			{
				_referencePoint = new HyperPoint<float>(60, 0, 0)
			};
			item1 = new Simplification2.Item(element1);
			item2 = new Simplification2.Item(element2);
			item3 = new Simplification2.Item(element3);
			item4 = new Simplification2.Item(element4);
			items = new List<Simplification2.Item>()
				        {
					        item1,
					        item2,
					        item3,
					        item4
				        };
			items.ForEach(x => x.referenced = new List<Simplification2.Combination>());
			closestElementsList = new SkipList<Simplification2.Combination>();

			itemsLists = new Dictionary<int, List<Simplification2.Item>>()
				             {
					             {FactoryIDs.ElementListID, new List<Simplification2.Item>()},
					             {FactoryIDs.BuildingID, new List<Simplification2.Item>()},
					             {FactoryIDs.LandUseID, new List<Simplification2.Item>()},
					             {FactoryIDs.Road2ID, new List<Simplification2.Item>()},
					             {FactoryIDs.RoadID, new List<Simplification2.Item>()},
					             {FactoryIDs.TestID, new List<Simplification2.Item>(items)}
				             };
			TestElement newElement = new TestElement()
			{
				_referencePoint = new HyperPoint<float>(05, 0, 0)
			};
			newItem = new Simplification2.Item(newElement);
			newItem.referenced = new List<Simplification2.Combination>();

			FactoryIDs.AddElementFactory(new TestElementFactory());
		}
		
		[Test]
		public void DistanceSquared()
		{
			float distance = Simplification2.CalcDistanceSquared(item1, item2);
			Assert.AreEqual(100, distance);
		}

		[Test]
		public void ConnectionExist()
		{
			bool result;
			TestElement element1 = new TestElement();
			TestElement element2 = new TestElement();
			Simplification2.Item item1 = new Simplification2.Item(element1);
			Simplification2.Item item2 = new Simplification2.Item(element2);
			item1.referenced = new List<Simplification2.Combination>();
			item2.referenced = new List<Simplification2.Combination>();
			result = simplification2.ConnectionExist(item1, item2);
			Assert.AreEqual(false, result);
		}

		[Test]
		public void ConnectionExist2()
		{
			bool result;
			TestElement element1 = new TestElement();
			TestElement element2 = new TestElement();
			Simplification2.Item item1 = new Simplification2.Item(element1);
			Simplification2.Item item2 = new Simplification2.Item(element2);
			item1.referenced = new List<Simplification2.Combination>();
			item2.referenced = new List<Simplification2.Combination>();

			Simplification2.Combination combination = new Simplification2.Combination();
			combination.first = item1;
			combination.second = item2;
			item1.referenced.Add(combination);
			item2.referenced.Add(combination);
			
			result = simplification2.ConnectionExist(item1, item2);
			Assert.AreEqual(true, result);
		}

		[Test]
		public void ConnectionExist3()
		{
			bool result;
			TestElement element1 = new TestElement();
			TestElement element2 = new TestElement();
			Simplification2.Item item1 = new Simplification2.Item(element1);
			Simplification2.Item item2 = new Simplification2.Item(element2);
			item1.referenced = new List<Simplification2.Combination>();
			item2.referenced = new List<Simplification2.Combination>();

			Simplification2.Combination combination = new Simplification2.Combination();
			combination.first = item2;
			combination.second = item1;
			item1.referenced.Add(combination);
			item2.referenced.Add(combination);

			result = simplification2.ConnectionExist(item1, item2);
			Assert.AreEqual(true, result);
		}
		
		[Test]
		public void CreateCombination()
		{
			simplification2.CreateCombination(closestElementsList, item1, item2);
			Assert.AreEqual(1, item1.referenced.Count);
			Assert.AreEqual(1, item2.referenced.Count);
			Assert.AreSame(item1.referenced[0], item2.referenced[0]);
			Simplification2.Combination combination = closestElementsList.First();
			Assert.AreSame(combination, item1.referenced[0]);
			Assert.AreSame(combination, item2.referenced[0]);

			Assert.AreSame(item1, combination.first);
			Assert.AreSame(item2, combination.second);
		}

		[Test]
		public void CreateCombination2()
		{
			simplification2.CreateCombination(closestElementsList, item1, item2);
			simplification2.CreateCombination(closestElementsList, item1, item2);
			simplification2.CreateCombination(closestElementsList, item2, item1);
			Assert.AreEqual(1, item1.referenced.Count);
			Assert.AreEqual(1, item2.referenced.Count);
			Assert.AreSame(item1.referenced[0], item2.referenced[0]);
			Simplification2.Combination combination = closestElementsList.First();
			Assert.AreSame(combination, item1.referenced[0]);
			Assert.AreSame(combination, item2.referenced[0]);

			Assert.AreSame(item1, combination.first);
			Assert.AreSame(item2, combination.second);
		}

		[Test]
		public void FindClosestItem()
		{
			Simplification2.Item minItem = simplification2.FindClosestItem(items, item1);
			Assert.AreSame(item2, minItem);
		}
		[Test]
		public void FindClosestItem2()
		{
			Simplification2.Item minItem = simplification2.FindClosestItem(items, item3);
			Assert.AreSame(item2, minItem);
		}

		[Test]
		public void CheckAndFindClosest()
		{
			simplification2.CheckAndFindClosest(closestElementsList, items, item1);
			Assert.AreEqual(1, item1.referenced.Count);
			Assert.AreEqual(1, item2.referenced.Count);
			Assert.AreSame(item1.referenced[0], item2.referenced[0]);
			Simplification2.Combination combination = closestElementsList.First();
			Assert.AreSame(combination, item1.referenced[0]);
			Assert.AreSame(combination, item2.referenced[0]);

			Assert.AreSame(item1, combination.first);
			Assert.AreSame(item2, combination.second);
		}

		[Test]
		public void CheckAndFindClosest2()
		{
			simplification2.CheckAndFindClosest(closestElementsList, items, item1);
			simplification2.CheckAndFindClosest(closestElementsList, items, item3);
			
			Assert.AreEqual(1, item1.referenced.Count);
			Assert.AreEqual(2, item2.referenced.Count);
			Assert.AreEqual(1, item1.referenced.Count);

			Assert.AreSame(item1.referenced[0], item2.referenced[0]);
			Assert.AreSame(item2.referenced[1], item3.referenced[0]);

			var enumarator = closestElementsList.GetEnumerator();
			
			enumarator.MoveNext();
			Simplification2.Combination combination = enumarator.Current;
			Assert.AreSame(combination, item1.referenced[0]);
			Assert.AreSame(combination, item2.referenced[0]);

			Assert.AreSame(item1, combination.first);
			Assert.AreSame(item2, combination.second);

			enumarator.MoveNext();
			Simplification2.Combination combination2 = enumarator.Current;
			Assert.AreSame(combination2, item3.referenced[0]);
			Assert.AreSame(combination2, item2.referenced[1]);

			Assert.AreSame(item3, combination2.first);
			Assert.AreSame(item2, combination2.second);
		}

		[Test]
		public void RemoveCombinationFromItem()
		{
			simplification2.CheckAndFindClosest(closestElementsList, items, item1);
			simplification2.CheckAndFindClosest(closestElementsList, items, item3);

			items.Remove(item1);
			items.Remove(item2);
			items.Remove(item4);

			simplification2.RemoveCombinationFromItem(closestElementsList, items, item3,
			                                          item2.referenced[1]);

			Assert.AreEqual(0, item3.referenced.Count);
		}

		[Test]
		public void RemoveCombinationFromItem2()
		{
			simplification2.CheckAndFindClosest(closestElementsList, items, item1);
			simplification2.CheckAndFindClosest(closestElementsList, items, item3);

			items.Remove(item1);
			items.Remove(item2);

			simplification2.RemoveCombinationFromItem(closestElementsList, items, item3,
													  item2.referenced[1]);

			Assert.AreEqual(1, item3.referenced.Count);
			Assert.AreEqual(1, item4.referenced.Count);
			Assert.AreSame(item3.referenced[0], item4.referenced[0]);
		}

		[Test]
		public void UpdateList()
		{
			//tested in InitilizeClosestElement
			simplification2.InitilizeClosestElement(closestElementsList, FactoryIDs.GetFactoryIDs(), itemsLists);

			simplification2.UpdateLists(closestElementsList, closestElementsList.First(), newItem, itemsLists);

			Assert.AreEqual(1, newItem.referenced.Count);
			Assert.AreEqual(2, item3.referenced.Count);
			Assert.AreEqual(1, item4.referenced.Count);
			Assert.AreSame(newItem.referenced[0], item3.referenced[1]);
			Assert.AreSame(item3.referenced[0], item4.referenced[0]);
			Assert.AreEqual(2, closestElementsList.Count());

			var enumerator = closestElementsList.GetEnumerator();

			Assert.AreEqual(true, enumerator.MoveNext());
			Assert.AreSame(enumerator.Current, newItem.referenced[0]);
			Assert.AreSame(enumerator.Current, item3.referenced[1]);

			Assert.AreSame(newItem, enumerator.Current.first);
			Assert.AreSame(item3, enumerator.Current.second);

			Assert.AreEqual(true, enumerator.MoveNext());
			Assert.AreSame(enumerator.Current, item3.referenced[0]);
			Assert.AreSame(enumerator.Current, item4.referenced[0]);

			Assert.AreSame(item4, enumerator.Current.first);
			Assert.AreSame(item3, enumerator.Current.second);
			Assert.AreEqual(false, enumerator.MoveNext());
		}

		[Test]
		public void InitilizeClosestElement()
		{
			simplification2.InitilizeClosestElement(closestElementsList, FactoryIDs.GetFactoryIDs(), itemsLists);

			Assert.AreEqual(1, item1.referenced.Count);
			Assert.AreEqual(2, item2.referenced.Count);
			Assert.AreEqual(2, item3.referenced.Count);
			Assert.AreEqual(1, item4.referenced.Count);
			Assert.AreSame(item1.referenced[0], item2.referenced[0]);
			Assert.AreSame(item2.referenced[1], item3.referenced[0]);
			Assert.AreSame(item3.referenced[1], item4.referenced[0]);
			Assert.AreEqual(3, closestElementsList.Count());

			var enumerator = closestElementsList.GetEnumerator();
			
			Assert.AreEqual(true, enumerator.MoveNext());
			Assert.AreSame(enumerator.Current, item1.referenced[0]);
			Assert.AreSame(enumerator.Current, item2.referenced[0]);

			Assert.AreSame(item1, enumerator.Current.first);
			Assert.AreSame(item2, enumerator.Current.second);

			Assert.AreEqual(true, enumerator.MoveNext());
			Assert.AreSame(enumerator.Current, item2.referenced[1]);
			Assert.AreSame(enumerator.Current, item3.referenced[0]);

			Assert.AreSame(item3, enumerator.Current.first);
			Assert.AreSame(item2, enumerator.Current.second);

			Assert.AreEqual(true, enumerator.MoveNext());
			Assert.AreSame(enumerator.Current, item3.referenced[1]);
			Assert.AreSame(enumerator.Current, item4.referenced[0]);

			Assert.AreSame(item4, enumerator.Current.first);
			Assert.AreSame(item3, enumerator.Current.second);
			Assert.AreEqual(false, enumerator.MoveNext());
		}

		[Test]
		public void SimplifyData()
		{
			simplification2.SimplifyData(itemsLists, FactoryIDs.GetFactoryIDs(), 4, 3);

			Assert.AreEqual(3, itemsLists[FactoryIDs.TestID].Count);

			Simplification2.Item newItem = itemsLists[FactoryIDs.TestID][2];
			Assert.AreEqual(1, newItem.referenced.Count);
			Assert.AreEqual(2, item3.referenced.Count);
			Assert.AreEqual(1, item4.referenced.Count);
			Assert.AreSame(newItem.referenced[0], item3.referenced[1]);
			Assert.AreSame(item3.referenced[0], item4.referenced[0]);

			Assert.AreEqual(30, itemsLists[FactoryIDs.TestID][0].element.ReferencePoint.X);
			Assert.AreEqual(60, itemsLists[FactoryIDs.TestID][1].element.ReferencePoint.X);
			Assert.AreEqual(05, itemsLists[FactoryIDs.TestID][2].element.ReferencePoint.X);

		}

		[Test]
		public void SimplifyData2()
		{
			simplification2.SimplifyData(itemsLists, FactoryIDs.GetFactoryIDs(), 4, 2);

			Assert.AreEqual(2, itemsLists[FactoryIDs.TestID].Count);

			Assert.AreEqual(60, itemsLists[FactoryIDs.TestID][0].element.ReferencePoint.X);
			Assert.AreEqual((30+5)/2f, itemsLists[FactoryIDs.TestID][1].element.ReferencePoint.X);
		}



		[Test]
		public void SimplifyData3()
		{
			items = new List<Simplification2.Item>();
			int count = 50*2;
			for (int i = 0; i < count; i++)
			{
				if(i % 2 == 0)
				{
					items.Add(new Simplification2.Item(new TestElement(){_referencePoint = new HyperPoint<float>(i*2, 0, 0)}));
				}
				else
				{
					items.Add(new Simplification2.Item(new TestElement() { _referencePoint = new HyperPoint<float>(i*2-1, 0, 0) }));
				}
			}
			items.ForEach(x => x.referenced = new List<Simplification2.Combination>());
			closestElementsList = new SkipList<Simplification2.Combination>();

			itemsLists = new Dictionary<int, List<Simplification2.Item>>()
				             {
					             {FactoryIDs.ElementListID, new List<Simplification2.Item>()},
					             {FactoryIDs.BuildingID, new List<Simplification2.Item>()},
					             {FactoryIDs.LandUseID, new List<Simplification2.Item>()},
					             {FactoryIDs.Road2ID, new List<Simplification2.Item>()},
					             {FactoryIDs.RoadID, new List<Simplification2.Item>()},
					             {FactoryIDs.TestID, new List<Simplification2.Item>(items)}
				             };

			simplification2.SimplifyData(itemsLists, FactoryIDs.GetFactoryIDs(), count, count/2);

			Assert.AreEqual(count/2, itemsLists[FactoryIDs.TestID].Count);

			for (int i = 0; i < count/2; i++)
			{
				int i1 = i*2;
				int i2 = i*2 + 1;
				float x = (i1 * 2 + i2 * 2 - 1) / 2f;
				if(!itemsLists[FactoryIDs.TestID].Any(y => y.element.ReferencePoint.X == x))
				{
					Assert.True(false);
				}
			}
		}

		[TestCase(10)]
		[TestCase(20)]
		[TestCase(30)]
		[TestCase(40)]
		[TestCase(50)]
		[TestCase(60)]
		[TestCase(70)]
		[TestCase(80)]
		[TestCase(90)]
		[TestCase(100)]
		public void SimplifyDataFib(int count)
		{
			List<float> fib = new List<float>();
			items = new List<Simplification2.Item>();
			float outputX = 2;
			fib.Add(1);
			fib.Add(1);
			fib.Add(2);
			for (int i = 1; i < count; i++)
			{
				fib.Add(fib[i] + fib[i+1]);
				outputX = (outputX + fib[i + 2])/2f;
			}
			fib.RemoveAt(0);
			fib.RemoveAt(0);


			for (int i = 0; i < count; i++)
			{
				items.Add(new Simplification2.Item(new TestElement() { _referencePoint = new HyperPoint<float>(fib[i], 0, 0) }));
			}
			items.ForEach(x => x.referenced = new List<Simplification2.Combination>());

			itemsLists = new Dictionary<int, List<Simplification2.Item>>()
				             {
					             {FactoryIDs.ElementListID, new List<Simplification2.Item>()},
					             {FactoryIDs.BuildingID, new List<Simplification2.Item>()},
					             {FactoryIDs.LandUseID, new List<Simplification2.Item>()},
					             {FactoryIDs.Road2ID, new List<Simplification2.Item>()},
					             {FactoryIDs.RoadID, new List<Simplification2.Item>()},
					             {FactoryIDs.TestID, new List<Simplification2.Item>(items)}
				             };

			simplification2.SimplifyData(itemsLists, FactoryIDs.GetFactoryIDs(), count, 1);

			Assert.AreEqual(1, itemsLists[FactoryIDs.TestID].Count);
			Assert.AreEqual(outputX, itemsLists[FactoryIDs.TestID][0].element.ReferencePoint[0]);
		}

		[Test]
		public void SimplifyData5(
			[Range(1, 100)]
			int count)
		{
			Random rand = new Random();
			List<HyperPoint<float>> referencesPoints = new List<HyperPoint<float>>(); 
			HyperPoint<float> result = new HyperPoint<float>(0,0,0);
			items = new List<Simplification2.Item>();
			for (int i = 0; i < count; i++)
			{
				referencesPoints.Add(new HyperPoint<double>(rand.NextDouble(), rand.NextDouble(), rand.NextDouble()).ConvertTo<float>());
			}
			result = referencesPoints.Aggregate(result, (point, hyperPoint) => point + hyperPoint)*(1f/count);

			for (int i = 0; i < count; i++)
			{
				items.Add(new Simplification2.Item(new TestElement() { _referencePoint = referencesPoints[i] }));
			}
			items.ForEach(x => x.referenced = new List<Simplification2.Combination>());

			itemsLists = new Dictionary<int, List<Simplification2.Item>>()
				             {
					             {FactoryIDs.ElementListID, new List<Simplification2.Item>()},
					             {FactoryIDs.BuildingID, new List<Simplification2.Item>()},
					             {FactoryIDs.LandUseID, new List<Simplification2.Item>()},
					             {FactoryIDs.Road2ID, new List<Simplification2.Item>()},
					             {FactoryIDs.RoadID, new List<Simplification2.Item>()},
					             {FactoryIDs.TestID, new List<Simplification2.Item>(items)}
				             };

			try
			{
				simplification2.SimplifyData(itemsLists, FactoryIDs.GetFactoryIDs(), count, 1);
			}
			catch (Exception e)
			{
				foreach (HyperPoint<float> referencesPoint in referencesPoints)
				{
					Console.Out.WriteLine(referencesPoint);
				}
				throw;
			}
			Assert.AreEqual(1, itemsLists[FactoryIDs.TestID].Count);
		}

		[Test]
		public void SimplifyData6()
		{
			#region dataset
			List<HyperPoint<float>> referencesPoints = new List<HyperPoint<float>>();
			referencesPoints.Add(new HyperPoint<float>(0.7120432f, 0.4791095f, 0.8116302f));
			referencesPoints.Add(new HyperPoint<float>(0.8385785f, 0.7880369f, 0.01598505f));
			referencesPoints.Add(new HyperPoint<float>(0.8980438f, 0.8699656f, 0.7010358f));
			#endregion

			#region init
			items = new List<Simplification2.Item>();
			for (int i = 0; i < referencesPoints.Count; i++)
			{
				items.Add(new Simplification2.Item(new TestElement() { _referencePoint = referencesPoints[i] }));
			}
			items.ForEach(x => x.referenced = new List<Simplification2.Combination>());

			itemsLists = new Dictionary<int, List<Simplification2.Item>>()
				             {
					             {FactoryIDs.ElementListID, new List<Simplification2.Item>()},
					             {FactoryIDs.BuildingID, new List<Simplification2.Item>()},
					             {FactoryIDs.LandUseID, new List<Simplification2.Item>()},
					             {FactoryIDs.Road2ID, new List<Simplification2.Item>()},
					             {FactoryIDs.RoadID, new List<Simplification2.Item>()},
					             {FactoryIDs.TestID, new List<Simplification2.Item>(items)}
				             };
			#endregion

			simplification2.SimplifyData(itemsLists, FactoryIDs.GetFactoryIDs(), referencesPoints.Count, 1);
			Assert.AreEqual(1, itemsLists[FactoryIDs.TestID].Count);
		}
	}
}
