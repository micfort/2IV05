using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using CG_2IV05.Common;
using micfort.GHL.Math2;
using micfort.GHL.Serialization;

namespace CG_2IV05.TreeBuilding
{
	class Program
	{
		private static readonly int MaxTriangleCount = 10000;
		private static int FileCount = 0;
		private static readonly string NodeFilenameFormat = "output\\data_{0}";
		private static readonly string TreeOutputFileFormat = "output\\tree";
		private static string InputFilename = "buildings.xml";

		static void Main(string[] args)
		{
			micfort.GHL.GHLWindowsInit.Init();

			if(!Directory.Exists("output"))
			{
				Directory.CreateDirectory("output");
			}

			Console.Out.WriteLine("Generating/Reading Buildings");
			List<Building> buildings = ReadBuildings();

			Console.Out.WriteLine("Create Nodes");
			Node root = CreateNode(buildings, null);
			CleanTag(root);
			Tree tree = new Tree();
			tree.Root = root;
			Console.Out.WriteLine("Writing Tree");
			using (FileStream file = File.Open(TreeOutputFileFormat, FileMode.Create, FileAccess.ReadWrite))
			{
				SerializableType<Tree>.SerializeToStream(tree, file, BinarySerializableTypeEngine.BinairSerializer);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buildings"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		/// <remarks>O(B^2*P)</remarks>
		public static Node CreateNode(List<Building> buildings, Node parent)
		{
			string dataFilename = CreateFilename();
			Node node = new Node
				            {
					            Children = new List<Node>(),
					            NodeDataFile = dataFilename,
					            Parent = parent,
					            Tag = buildings
				            };
			//for leaves this is O(B^2*P)
			if(buildings.Count <= 1 || TriangleCount(buildings) < MaxTriangleCount) 
			{
				//Leave
				NodeData data = CreateData(buildings); //O(B*P)
				using (FileStream file = File.Open(dataFilename, FileMode.Create, FileAccess.ReadWrite))
				{
					data.SaveToStream(file);
				}

				return node;
			}
			else
			//for non leaves this is O(log_4(B)*B)
			{
				//Not a leave
				List<Building>[] split = SplitList(buildings); //O(B)
				for (int i = 0; i < 4; i++)
				{
					Node child = CreateNode(split[i], node);
					node.Children.Add(child);
				}
				
				NodeData data = CreateDataFromChildren(buildings);//O(B)
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buildings"></param>
		/// <returns></returns>
		/// <remarks>O(B*P)</remarks>
		public static NodeData CreateData(List<Building> buildings)
		{
			NodeData output = new NodeData();
			List<NodeData> nodeDataList = buildings.ConvertAll(CreateData);
			int verticesCount = nodeDataList.ConvertAll(x => x.Vertices.Length).Sum();
			int indexesCount = nodeDataList.ConvertAll(x => x.Indexes.Length).Sum();

			output.Vertices = new HyperPoint<float>[verticesCount];
			output.Indexes = new int[indexesCount];

			int verticesI = 0;
			int indexI = 0;

			for (int i = 0; i < nodeDataList.Count; i++)
			{
				for (int j = 0; j < nodeDataList[i].Indexes.Length; j++)
				{
					nodeDataList[i].Indexes[j] += verticesI;
				}
				Array.Copy(nodeDataList[i].Vertices, 0, output.Vertices, verticesI, nodeDataList[i].Vertices.Length);
				Array.Copy(nodeDataList[i].Indexes, 0, output.Indexes, indexI, nodeDataList[i].Indexes.Length);
				verticesI += nodeDataList[i].Vertices.Length;
				indexI += nodeDataList[i].Indexes.Length;
			}

			return output;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="building"></param>
		/// <returns></returns>
		/// <remarks>O(P)</remarks>
		public static NodeData CreateData(Building building)
		{
			NodeData data = new NodeData();
			data.Vertices = new HyperPoint<float>[building.Polygon.Count*2]; //high and low point
			data.Indexes = new int[building.Polygon.Count * 2 * 3]; //2 triangles per square, 3 points per triangle
			for (int i = 0; i < building.Polygon.Count; i++)
			{
				int currentLow = i*2 + 0;
				int currentHigh = i*2 + 1;

				int LastLow = (i-1)*2 + 0;
				int LastHigh = (i-1)*2 + 1;
				if(LastLow < 0)
				{
					LastLow = (building.Polygon.Count - 1)*2 + 0;
					LastHigh = (building.Polygon.Count - 1)*2 + 1;
				}

				data.Vertices[i*2+0] = new HyperPoint<float>(building.Polygon[i].X, building.Polygon[i].Y, 0);
				data.Vertices[i*2+1] = new HyperPoint<float>(building.Polygon[i].X, building.Polygon[i].Y, building.Height);

				data.Indexes[i*2*3 + 3*0 + 0] = currentLow;
				data.Indexes[i*2*3 + 3*0 + 1] = currentHigh;
				data.Indexes[i*2*3 + 3*0 + 2] = LastLow;

				data.Indexes[i*2*3 + 3*1 + 0] = LastLow;
				data.Indexes[i*2*3 + 3*1 + 1] = currentHigh;
				data.Indexes[i*2*3 + 3*1 + 2] = LastHigh;
			}
			return data;
		}

		/// <summary>
		/// simple version of the data from children, just calculates the bounding box of every building in the set, also takes the average height
		/// </summary>
		/// <param name="buildings"></param>
		/// <returns></returns>
		/// <remarks>O(B) method</remarks>
		public static NodeData CreateDataFromChildren(List<Building> buildings)
		{
			HyperPoint<float> min = GetMinPoint(buildings);
			HyperPoint<float> max = GetMaxPoint(buildings);

			Building b = new Building();
			b.Polygon = new List<HyperPoint<float>>();
			b.Polygon.Add(new HyperPoint<float>(min.X, min.Y, 0));
			b.Polygon.Add(new HyperPoint<float>(min.X, max.Y, 0));
			b.Polygon.Add(new HyperPoint<float>(max.X, max.Y, 0));
			b.Polygon.Add(new HyperPoint<float>(max.X, min.Y, 0));
			b.Height = buildings.Average(x => x.Height);

			return CreateData(b);
		}

		public static NodeData CreateDataFromChildren(List<Node> Children)
		{
			throw new NotImplementedException();
		}

		public static string CreateFilename()
		{
			return string.Format(NodeFilenameFormat, FileCount++);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buildings"></param>
		/// <returns></returns>
		/// <remarks>O(B) method</remarks>
		public static List<Building>[] SplitList(List<Building> buildings)
		{
			HyperPoint<float> min = GetMinPoint(buildings);
			HyperPoint<float> max = GetMaxPoint(buildings);

			HyperPoint<float> center = (max - min);
			center = center*(1f/2f);
			center = center + min;

			List<Building>[] output = new List<Building>[4];
			for (int i = 0; i < 4; i++)
			{
				output[i] = new List<Building>();
			}
			for (int i = 0; i < buildings.Count; i++)
			{
				if (buildings[i].Polygon[0].Y < center.Y)
				{
					if (buildings[i].Polygon[0].X < center.X)
					{
						output[0].Add(buildings[i]);
					}
					else
					{
						output[1].Add(buildings[i]);
					}
				}
				else
				{
					if (buildings[i].Polygon[0].X < center.X)
					{
						output[2].Add(buildings[i]);
					}
					else
					{
						output[3].Add(buildings[i]);
					}
				}
			}
			return output;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buildings"></param>
		/// <returns></returns>
		/// <remarks>O(B) method</remarks>
		public static HyperPoint<float> GetMinPoint(List<Building> buildings)
		{
			float xMin = buildings.Min(x => x.Polygon.Min(y => y.X));
			float yMin = buildings.Min(x => x.Polygon.Min(y => y.Y));
			return new HyperPoint<float>(xMin, yMin, 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="buildings"></param>
		/// <returns></returns>
		/// <remarks>O(B) method</remarks>
		public static HyperPoint<float> GetMaxPoint(List<Building> buildings)
		{
			float xMax = buildings.Max(x => x.Polygon.Max(y => y.X));
			float yMax = buildings.Max(x => x.Polygon.Max(y => y.Y));
			return new HyperPoint<float>(xMax, yMax, 0);
		}

		public static void CleanTag(Node node)
		{
			node.Tag = null;
			node.Children.ForEach(CleanTag);
		}

		public static List<Building> CreateData()
		{
			List<Building> output = new List<Building>();
			for (int i = 0; i < 10000; i++)
			{
				for (int j = 0; j < 1000; j++)
				{
					Building b = new Building();
					int x = i*2;
					int y = j*2;
					b.Polygon = new List<HyperPoint<float>>();
					b.Polygon.Add(new HyperPoint<float>(x, y, 0));
					b.Polygon.Add(new HyperPoint<float>(x, y+1, 0));
					b.Polygon.Add(new HyperPoint<float>(x+1, y+1, 0));
					b.Polygon.Add(new HyperPoint<float>(x+1, y, 0));
					b.Height = 1;
					output.Add(b);
				}
			}
			return output;
		} 

		public static List<Building> ReadBuildings()
		{
			List<Building> output = new List<Building>(); 
			XmlReader reader = XmlReader.Create(InputFilename);
			while (reader.Read())
			{
				if(reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "Building")
					{
						output.Add(ReadBuilding(reader));
					}
				}
			}
			return output;
		}

		public static Building ReadBuilding(XmlReader reader)
		{
			Building output = new Building();
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "height")
					{
						output.Height = reader.ReadElementContentAsFloat();
					}
					else if(reader.Name == "gmlbase64")
					{
						output.Polygon = ReadGML(reader.ReadElementContentAsString());
					}
				}
				else if(reader.NodeType == XmlNodeType.EndElement)
				{
					if(reader.Name == "Building")
					{
						return output;
					}
				}
			}
			return null;
		}

		public static List<HyperPoint<float>> ReadGML(string gml64)
		{
			List<HyperPoint<float>> output = new List<HyperPoint<float>>();

			string gml = System.Text.Encoding.UTF8.GetString(micfort.GHL.Base64.DecodeS(gml64));
			gml = gml.Replace("gml:", "");
			XmlDocument dom = new XmlDocument();
			dom.LoadXml(gml);
			XmlNodeList nodes = dom.DocumentElement.SelectNodes("/Polygon/outerBoundaryIs/LinearRing/coordinates");
			string coordinates = nodes[0].InnerText;
			string[][] coordinatesSplit = Array.ConvertAll(coordinates.Split(' '), x => x.Split(','));
			for (int i = 0; i < coordinatesSplit.Length; i++)
			{
				output.Add(new HyperPoint<float>(float.Parse(coordinatesSplit[i][0], CultureInfo.InvariantCulture),
												 float.Parse(coordinatesSplit[i][1], CultureInfo.InvariantCulture),
												 float.Parse(coordinatesSplit[i][2], CultureInfo.InvariantCulture)));
			}
			return output;
		}

	}
}
