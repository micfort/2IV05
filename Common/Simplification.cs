using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG_2IV05.Common.Element;

namespace CG_2IV05.Common
{
	class Simplification
	{
		public FileElementList CreateDataFromChildren(List<FileElementList> children, List<int> heights, int currentDepth, out float error)
		{
			string filename = FilenameGenerator.CreateTempFilename();
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
						Dictionary<int, List<IElement>> elementLists = new Dictionary<int, List<IElement>>(factorys.Count);
						foreach (int factoryID in factorys)
						{
							elementLists[factoryID] = new List<IElement>();
						}
						elements.ForEach(x => elementLists[x.FactoryID].Add(x));
						SortedList<float, Tuple<IElement, IElement>> closestElementsList = new SortedList<float, Tuple<IElement, IElement>>();

						InitilizeClosestElement(closestElementsList, factorys, elementLists);

						while (triangleCount > TreeBuildingSettings.MaxTriangleCount && closestElementsList.Count > 0)
						{
							//get lowest distance
							KeyValuePair<float, Tuple<IElement, IElement>> closestElement = closestElementsList.First();
							int factoryID = closestElement.Value.Item1.FactoryID;

							//remove from triangleCount
							triangleCount -= closestElement.Value.Item1.TriangleCount;
							triangleCount -= closestElement.Value.Item2.TriangleCount;

							//remove out of collections
							closestElementsList.RemoveAt(0);
							elementLists[factoryID].Remove(closestElement.Value.Item1);
							elementLists[factoryID].Remove(closestElement.Value.Item2);

							//merge elements
							var factory = FactoryIDs.GetFactory(factoryID);
							IElement newElement = factory.Merge(new List<IElement>() { closestElement.Value.Item1, closestElement.Value.Item2 });

							//add to triangle count
							triangleCount += newElement.TriangleCount;

							//add to the collection
							elementLists[factoryID].Add(newElement);

							//find new closest one for this factory ID
							AddClosestElement(closestElementsList, factoryID, elementLists);
						}

						foreach (KeyValuePair<int, List<IElement>> elementList in elementLists)
						{
							foreach (IElement element in elementList.Value)
							{
								writer.WriteElement(element);
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

		private void InitilizeClosestElement(SortedList<float, Tuple<IElement, IElement>> closestElementsList, List<int> factorys, Dictionary<int, List<IElement>> elementLists)
		{
			foreach (int factory in factorys)
			{
				AddClosestElement(closestElementsList, factory, elementLists);
			}
		}

		private void AddClosestElement(SortedList<float, Tuple<IElement, IElement>> closestElementsList, int factory, Dictionary<int, List<IElement>> elementLists)
		{
			Tuple<float, Tuple<IElement, IElement>> closestElement = Find2ClosestElements(elementLists[factory], factory);
			if (closestElement.Item2 != null)
			{
				closestElementsList.Add(closestElement.Item1, closestElement.Item2);
			}
		}

		private Tuple<float, Tuple<IElement, IElement>> Find2ClosestElements(List<IElement> list, int factoryID)
		{
			IElementFactory factory = FactoryIDs.GetFactory(factoryID);
			Tuple<IElement, IElement> output = null;
			float min = float.PositiveInfinity;
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < list.Count; j++)
				{
					if(i != j)
					{
						float distance = DistanceSquared(list[i], list[j]);
						if (distance < min && factory.CanMerge(new List<IElement>() { list[i], list[j] }))
						{
							min = distance;
							output = new Tuple<IElement, IElement>(list[i], list[j]);
						}
					}
				}
			}
			return new Tuple<float, Tuple<IElement, IElement>>(min, output);
		}

		private float DistanceSquared(IElement a, IElement b)
		{
			return (a.ReferencePoint - b.ReferencePoint).GetLengthSquared();
		}
	}
}
