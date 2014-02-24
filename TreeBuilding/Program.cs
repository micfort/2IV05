using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CG_2IV05.Common;
using micfort.GHL.Math2;
using micfort.GHL.Serialization;

namespace CG_2IV05.TreeBuilding
{
	class Program
	{
		private static readonly int MaxTriangleCount = 10000;
		private static int FileCount = 0;
		private static readonly string FilenameFormat = "data_{0}";

		static void Main(string[] args)
		{
			micfort.GHL.GHLWindowsInit.Init();
			


		}

		public static Node CreateNode(List<Building> buildings, Node parent)
		{
			if(buildings.Count == 1 || TriangleCount(buildings) < MaxTriangleCount)
			{
				//Leave
				string dataFilename = CreateFilename();
				NodeData data = CreateData(buildings);
				using (FileStream file = File.Open(dataFilename, FileMode.Create, FileAccess.ReadWrite))
				{
					data.SaveToStream(file);
				}
				Node node = new Node
					            {
						            Children = new List<Node>(), 
									NodeDataFile = dataFilename, 
									Parent = parent
					            };
				return node;
			}
			else
			{
				//Not a leave

				string dataFilename = CreateFilename();

				Node node = new Node();

				List<Building>[] split = SplitList(buildings);
				for (int i = 0; i < 4; i++)
				{
					Node child = CreateNode(split[i], node);
					node.Children.Add(child);
				}
				node.NodeDataFile = dataFilename;
				node.Parent = parent;

				
				NodeData data = CreateDataFromChildren(node.Children);
				using (FileStream file = File.Open(dataFilename, FileMode.Create, FileAccess.ReadWrite))
				{
					data.SaveToStream(file);
				}

				return node;
			}
		}

		public static int TriangleCount(List<Building> buildings)
		{
			int n = buildings.Sum(t => t.Polygon.Count*2);
			return n;
		}

		public static NodeData CreateData(List<Building> buildings)
		{
			throw new NotImplementedException();
		}

		public static NodeData CreateDataFromChildren(List<Node> Children)
		{
			throw new NotImplementedException();
		}

		public static string CreateFilename()
		{
			return string.Format(FilenameFormat, FileCount++);
		}

		public static List<Building>[] SplitList(List<Building> buildings)
		{
			throw new NotImplementedException();
		}
	}
}
