using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG_2IV05.Common.Element;

namespace CG_2IV05.Common
{
	class Simplification2
	{
		class Item
		{
			public IElement element;
			public List<Combination> referenced;

			public Item(IElement element)
			{
				this.element = element;
			}
		}
		class Combination: IComparable<Combination>
		{
			public Item first;
			public Item second;
			public SkipListItem<Combination> skipListItem; 

			public float DistanceSquared
			{
				get { return CalcDistanceSquared(first, second); }
			}

			public Item Merge()
			{
				var factory = FactoryIDs.GetFactory(first.element.FactoryID);
				IElement element = factory.Merge(new List<IElement>(2){first.element, second.element});
				return new Item(element);
			}

			#region Implementation of IComparable<in Combination>

			/// <summary>
			/// Compares the current object with another object of the same type.
			/// </summary>
			/// <returns>
			/// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
			/// </returns>
			/// <param name="other">An object to compare with this object.</param>
			public int CompareTo(Combination other)
			{
				return DistanceSquared.CompareTo(other.DistanceSquared);
			}

			#endregion
		}

		public float DetermineError(FileElementList OriginalList, FileElementList UsedList, int currentDepth)
		{
			if(currentDepth >= TreeBuildingSettings.MinCurrentDepthForData)
			{
				return OriginalList.TriangleCount - UsedList.TriangleCount;
			}
			else
			{
				return float.PositiveInfinity;
			}
		}

		public FileElementList CreateDataFromChildren(List<FileElementList> children, string filename, List<int> heights, int currentDepth, out float error)
		{
			if (currentDepth >= TreeBuildingSettings.MinCurrentDepthForData)
			{
				error = 0;
				using (FileElementListWriter writer = new FileElementListWriter(filename))
				{
					if (heights.Exists(x => x == 0))
					{
						#region First step (convex hulls)

						for (int i = 0; i < children.Count; i++)
						{
							if (heights[i] == 0)
							{
								foreach (IElement element in children[i])
								{
									error += element.TriangleCount;
									writer.WriteElement(element.GetSimplifiedVersion());
									error -= element.TriangleCount;
								}
							}
							else
							{
								foreach (IElement element in children[i])
								{
									writer.WriteElement(element);
								}
							}
						}

						#endregion
					}
					else
					{
						int triangleCount = children.Aggregate(0, (i, list) => i + list.TriangleCount);
						List<ElementList> lists = children.ConvertAll(x => x.ToElementList());
						List<IElement> elements = lists.Aggregate(new List<IElement>(),
																  (list, elementList) =>
																  {
																	  list.AddRange(elementList.Elements);
																	  return list;
																  });
						List<int> factorys = FactoryIDs.GetFactoryIDs();
						Dictionary<int, List<Item>> itemLists = new Dictionary<int, List<Item>>(factorys.Count);
						foreach (int factoryID in factorys)
						{
							itemLists[factoryID] = new List<Item>();
						}
						elements.ForEach(x => itemLists[x.FactoryID].Add(new Item(x)));
						factorys.ForEach(x => itemLists[x].ForEach(y => y.referenced = new List<Combination>()));
						SkipList<Combination> closestElementsList = new SkipList<Combination>();

						InitilizeClosestElement(closestElementsList, factorys, itemLists);

						while (triangleCount > TreeBuildingSettings.MaxTriangleCount && closestElementsList.Any())
						{
							//get lowest distance
							Combination combination = closestElementsList.First();

							//remove from triangleCount
							triangleCount -= combination.first.element.TriangleCount;
							triangleCount -= combination.second.element.TriangleCount;

							error += combination.first.element.TriangleCount;
							error += combination.second.element.TriangleCount;

							Item newItem = combination.Merge();
							newItem.referenced = new List<Combination>();

							//add to triangle count
							triangleCount += newItem.element.TriangleCount;

							error -= newItem.element.TriangleCount;

							UpdateLists(closestElementsList, combination, newItem, itemLists);
						}

						foreach (KeyValuePair<int, List<Item>> itemList in itemLists)
						{
							foreach (Item item in itemList.Value)
							{
								writer.WriteElement(item.element);
							}
						}
					}
				}
			}
			else
			{
				error = float.PositiveInfinity;
				FileElementListWriter.CreateEmptyFile(filename);
			}
			return new FileElementList(filename);
		}

		private void InitilizeClosestElement(SkipList<Combination> closestElementsList, IEnumerable<int> factorys, Dictionary<int, List<Item>> itemsLists)
		{
			foreach (int factoryID in factorys)
			{
				if(factoryID == FactoryIDs.BuildingID)
				{
					List<Item> items = itemsLists[factoryID];
					double[,] points = new double[items.Count,2];
					int[] tags = new int[items.Count];
					for (int i = 0; i < items.Count; i++)
					{
						points[i, 0] = items[i].element.ReferencePoint[0];
						points[i, 1] = items[i].element.ReferencePoint[1];
						tags[i] = i;
					}
					alglib.kdtree tree;
					alglib.kdtreebuildtagged(points, tags, items.Count, 2, 0, 0, out tree);

					if (items.Count > 1)
					{
						for (int i = 0; i < items.Count; i++)
						{
							double[] point = new double[2];
							point[0] = items[i].element.ReferencePoint[0];
							point[1] = items[i].element.ReferencePoint[1];
							int[] closestIndexes = new int[1];

							int k = alglib.kdtreequeryknn(tree, point, 1, false);

							if (k > 0)
							{
								alglib.kdtreequeryresultstags(tree, ref closestIndexes);
								CreateCombination(closestElementsList, items[i], items[closestIndexes[0]]);
							}
						}
					}
				}
			}
		}

		private void UpdateLists(SkipList<Combination> closestElementsList, Combination oldCombination, Item newItem, Dictionary<int, List<Item>> itemsLists)
		{
			closestElementsList.Remove(oldCombination);

			int factoryID = newItem.element.FactoryID;

			List<Item> items = itemsLists[factoryID];

			//remove the old items to from the lists
			items.Remove(oldCombination.first);
			items.Remove(oldCombination.second);

			//remove combinations from the skiplist of the old items
			foreach (Combination combination in oldCombination.first.referenced)
			{
				combination.skipListItem.RemoveFromList();
				if(combination.first == oldCombination.first)
				{
					RemoveCombinationFromItem(closestElementsList, items, combination.second, combination);
				}
				else
				{
					RemoveCombinationFromItem(closestElementsList, items, combination.first, combination);
				}
			}
			foreach (Combination combination in oldCombination.second.referenced)
			{
				combination.skipListItem.RemoveFromList();
				if (combination.first == oldCombination.second)
				{
					RemoveCombinationFromItem(closestElementsList, items, combination.second, combination);
				}
				else
				{
					RemoveCombinationFromItem(closestElementsList, items, combination.first, combination);
				}
			}

			//create new item
			Item minItem = FindClosestItem(items, newItem);
			CreateCombination(closestElementsList, newItem, minItem);

			//add to the items list
			itemsLists[factoryID].Add(newItem);
		}

		private void RemoveCombinationFromItem(SkipList<Combination> closestElementsList, List<Item> items, Item item, Combination combination)
		{
			item.referenced.Remove(combination);
			CheckAndFindClosest(closestElementsList, items, item);
		}

		private void CheckAndFindClosest(SkipList<Combination> closestElementsList, List<Item> itemsList, Item item)
		{
			if (item.referenced.Count == 0)
			{
				Item minItem = FindClosestItem(itemsList, item);
				if(minItem != null)
				{
					CreateCombination(closestElementsList, item, minItem);
				}
			}
		}

		private Item FindClosestItem(List<Item> list, Item item)
		{
			var factory = FactoryIDs.GetFactory(item.element.FactoryID);
			float min = float.PositiveInfinity;
			Item output = null;
			for (int i = 0; i < list.Count; i++)
			{
				if (item != list[i])
				{
					float distance = CalcDistanceSquared(item, list[i]);
					if (distance < min && factory.CanMerge(new List<IElement>() { list[i].element, item.element }))
					{
						min = distance;
						output = list[i];
					}
				}
			}
			return output;
		}

		private void CreateCombination(SkipList<Combination> closestElementsList, Item item1, Item item2)
		{
			Combination combination = new Combination()
				                          {
											  first = item1,
											  second = item2
				                          };
			SkipListItem<Combination> skipListItem = closestElementsList.Insert(combination);
			combination.skipListItem = skipListItem;
			item1.referenced.Add(combination);
			item2.referenced.Add(combination);
		}

		private static float CalcDistanceSquared(Item a, Item b)
		{
			return (a.element.ReferencePoint - b.element.ReferencePoint).GetLengthSquared();
		}
	}
}
