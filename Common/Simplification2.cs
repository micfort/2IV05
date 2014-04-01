using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG_2IV05.Common.Element;

namespace CG_2IV05.Common
{
	public class Simplification2
	{
		public class Item
		{
			public IElement element;
			public List<Combination> referenced;

			public Item(IElement element)
			{
				this.element = element;
			}
		}
		public class Combination: IComparable<Combination>
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

			public bool ReferenceTo(Item item)
			{
				return first == item || second == item;
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

		public FileElementList CreateDataFromChildren(List<FileElementList> children, string filename, List<int> heights, int currentDepth)
		{
			if (currentDepth >= TreeBuildingSettings.MinCurrentDepthForData)
			{
				using (FileElementListWriter writer = new FileElementListWriter(filename))
				{
					if (heights.Exists(x => x == 0) && TreeBuildingSettings.SimplifySingleElements)
					{
						#region First step (convex hulls)

						for (int i = 0; i < children.Count; i++)
						{
							if (heights[i] == 0)
							{
								foreach (IElement element in children[i])
								{
									writer.WriteElement(element.GetSimplifiedVersion(heights.Max()+1));
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
						//calculate original triangle count
						int triangleCount = children.Aggregate(0, (i, list) => i + list.TriangleCount);
						//Read all elements into memory
						List<ElementList> lists = children.ConvertAll(x => x.ToElementList());
						//create 1 big list
						List<IElement> elements = lists.Aggregate(new List<IElement>(),
						                                          (list, elementList) =>
							                                          {
								                                          list.AddRange(elementList.Elements);
								                                          return list;
							                                          });

						//get all factorys
						List<int> factorys = FactoryIDs.GetFactoryIDs();
						//create lists for every factory
						Dictionary<int, List<Item>> itemLists = new Dictionary<int, List<Item>>(factorys.Count);
						factorys.ForEach(x => itemLists[x] = new List<Item>());
						
						//Create items with the elements and add them in the correct list
						elements.ForEach(x => itemLists[x.FactoryID].Add(new Item(x)));
						factorys.ForEach(x => itemLists[x].ForEach(y => y.referenced = new List<Combination>()));

						//remove elements
						RemoveElements(itemLists, factorys, heights.Max() + 1);

						//simplifyData
						SimplifyData(itemLists, factorys, triangleCount, TreeBuildingSettings.MaxTriangleCount);

						//write everything to the output file
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
				//when the depth is too low, create an empty file
				FileElementListWriter.CreateEmptyFile(filename);
			}
			return new FileElementList(filename);
		}

		public void RemoveElements(Dictionary<int, List<Item>> itemLists, List<int> factorys, int height)
		{
			//remove items if neccesary of the factory
			foreach (int factoryID in factorys)
			{
				IElementFactory factory = FactoryIDs.GetFactory(factoryID);
				itemLists[factoryID].RemoveAll(x => factory.RemoveItem(x.element, height));
			}
		}

		public void SimplifyData(Dictionary<int, List<Item>> itemLists, List<int> factorys, int triangleCount, int maxTriangleCount)
		{
			//create skiplist
			SkipList<Combination> closestElementsList = new SkipList<Combination>();
			//initilize the the skip list
			InitilizeClosestElement(closestElementsList, factorys, itemLists);

			//while too much triangles
			while (triangleCount >= maxTriangleCount && closestElementsList.Any())
			{
				//get lowest distance combination
				Combination combination = closestElementsList.First();

				//remove from triangleCount
				triangleCount -= combination.first.element.TriangleCount;
				triangleCount -= combination.second.element.TriangleCount;

				//merge the combination
				Item newItem = combination.Merge();
				newItem.referenced = new List<Combination>();

				//add to triangle count
				triangleCount += newItem.element.TriangleCount;

				//update the lists (remove old combination, add new item)
				UpdateLists(closestElementsList, combination, newItem, itemLists);
			}
		}

		public void InitilizeClosestElement(SkipList<Combination> closestElementsList, IEnumerable<int> factorys, Dictionary<int, List<Item>> itemsLists)
		{
			//for every factory
			foreach (int factoryID in factorys)
			{
				if(factoryID == FactoryIDs.BuildingID || factoryID == FactoryIDs.TestID || factoryID == FactoryIDs.TestViewID)//debug (this only can be done for buildings, the other can't be merged yet)
				{
					//get the correct list
					List<Item> items = itemsLists[factoryID];

					//if there is only 0 or 1 childeren then there isn't a possibilty to merge
					if (items.Count > 1)
					{
						//create the points list with tags
						double[,] points = new double[items.Count,2];
						int[] tags = new int[items.Count];
						for (int i = 0; i < items.Count; i++)
						{
							points[i, 0] = items[i].element.ReferencePoint[0];
							points[i, 1] = items[i].element.ReferencePoint[1];
							tags[i] = i;
						}

						//build the k-d tree
						alglib.kdtree tree;
						alglib.kdtreebuildtagged(points, tags, items.Count, 2, 0, 0, out tree);

						//for every item
						for (int i = 0; i < items.Count; i++)
						{
							//query the k-d tree
							double[] point = new double[2];
							point[0] = items[i].element.ReferencePoint[0];
							point[1] = items[i].element.ReferencePoint[1];
							int k = alglib.kdtreequeryknn(tree, point, 1, false);

							//if there is point found
							if (k > 0)
							{
								//query the index
								int[] closestIndexes = new int[1];
								alglib.kdtreequeryresultstags(tree, ref closestIndexes);
								//create a combination between the 2 elements
								CreateCombination(closestElementsList, items[i], items[closestIndexes[0]]);
							}
						}
					}
				}
			}
		}

		public void UpdateLists(SkipList<Combination> closestElementsList, Combination oldCombination, Item newItem, Dictionary<int, List<Item>> itemsLists)
		{	
			//get the factory id
			int factoryID = newItem.element.FactoryID;
			//get the correct items
			List<Item> items = itemsLists[factoryID];
			
			//remove the skiplist item
			oldCombination.skipListItem.RemoveFromList();

			//remove the old items to from the lists
			if(!items.Remove(oldCombination.first))
				throw new Exception("Removing item that doesn't exist");
			if(!items.Remove(oldCombination.second))
				throw new Exception("Removing item that doesn't exist");

			//remove combination that aren't of any use anymore

			//for everything where the first references too
			foreach (Combination combination in oldCombination.first.referenced)
			{
				//remove that item from the skip list
				if(combination.skipListItem.InSkipList) combination.skipListItem.RemoveFromList();
				//if it was the first in combination
				if (combination.first == oldCombination.first)
				{
					//then remove the combination from the second
					RemoveCombinationFromItem(closestElementsList, items, combination.second, combination);
				}
				else
				{
					//else remove the combination from the first
					RemoveCombinationFromItem(closestElementsList, items, combination.first, combination);
				}
			}
			//for everything where the second references too
			foreach (Combination combination in oldCombination.second.referenced)
			{
				//remove that combination from the skiplist
				if (combination.skipListItem.InSkipList) combination.skipListItem.RemoveFromList();
				//if it was the first in the combination
				if (combination.first == oldCombination.second)
				{
					//then remove the combination from the second
					RemoveCombinationFromItem(closestElementsList, items, combination.second, combination);
				}
				else
				{
					//else remove the combination from the first
					RemoveCombinationFromItem(closestElementsList, items, combination.first, combination);
				}
			}

			//find the closest item to the newItem and add it to the closest elementList
			CheckAndFindClosest(closestElementsList, items, newItem);

			//add the new item the items list
			items.Add(newItem);
		}

		public void RemoveCombinationFromItem(SkipList<Combination> closestElementsList, List<Item> items, Item item, Combination combination)
		{
			//remove the combination
			item.referenced.Remove(combination);
			//check if there is need for adding a new combination
			CheckAndFindClosest(closestElementsList, items, item);
		}

		public void CheckAndFindClosest(SkipList<Combination> closestElementsList, List<Item> itemsList, Item item)
		{
			//find closest item
			Item minItem = FindClosestItem(itemsList, item);
			//if found
			if(minItem != null)
			{
				//create the combination
				CreateCombination(closestElementsList, item, minItem);
			}
		}

		public Item FindClosestItem(List<Item> list, Item item)
		{
			var factory = FactoryIDs.GetFactory(item.element.FactoryID);

			//init variable
			float min = float.PositiveInfinity;
			Item output = null;
			//for every item
			for (int i = 0; i < list.Count; i++)
			{
				//if it isn't the same
				if (item != list[i])
				{
					//calculate distance
					float distance = CalcDistanceSquared(item, list[i]);
					//if the distance is less than the minimal distance and the items can merge together
					if (distance < min && factory.CanMerge(new List<IElement>() { list[i].element, item.element }))
					{
						//use the new item as minimal item
						min = distance;
						output = list[i];
					}
				}
			}
			return output;
		}

		public void CreateCombination(SkipList<Combination> closestElementsList, Item item1, Item item2)
		{
			//if connection doesn't exist
			if (!ConnectionExist(item1, item2))
			{
				//create combination
				Combination combination = new Combination()
					                          {
						                          first = item1,
						                          second = item2
					                          };
				//insert it into the skiplist
				SkipListItem<Combination> skipListItem = closestElementsList.Insert(combination);
				//make sure the combination knows of the skiplist item
				combination.skipListItem = skipListItem;
				//update references to the combination in item1 and item2
				item1.referenced.Add(combination);
				item2.referenced.Add(combination);
			}
		}

		public bool ConnectionExist(Item item1, Item item2)
		{
			bool output = false;
			//test if the connection exist in item1
			foreach (Combination combination in item1.referenced)
			{
				output |= combination.first == item1 && combination.second == item2;
				output |= combination.second == item1 && combination.first == item2;
				if(output) break;
			}
			//test if the connection exist in item2
			foreach (Combination combination in item2.referenced)
			{
				output |= combination.first == item1 && combination.second == item2;
				output |= combination.second == item1 && combination.first == item2;
				if (output) break;
			}
			return output;
		}

		public static float CalcDistanceSquared(Item a, Item b)
		{
			return (a.element.ReferencePoint - b.element.ReferencePoint).GetLengthSquared();
		}
	}
}
