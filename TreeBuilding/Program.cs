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
		private static int MaxTriangleCount = 10000;
		private static int FileCount = 0;
		private static string NodeFilenameFormat = "{0}\\data_{1}";
		private static string TreeOutputFileFormat = "{0}\\tree";
		private static string DirectoryOutput = "output";
		private static string InputFilename = "buildings.xml";
		private static bool Generate = false;
		private static int generateSizeX = 100;
		private static int generateSizeY = 100;
		private static HyperPoint<float> CenterDataSet;
		private static bool FindCenterDataSet = true;

		static void Main(string[] args)
		{
			micfort.GHL.GHLWindowsInit.Init();

			ParseCommandLine(args);

			if (!Directory.Exists(DirectoryOutput))
			{
				Directory.CreateDirectory(DirectoryOutput);
			}

			Console.Out.WriteLine("Generating/Reading Buildings");
			List<Building> buildings;
			if(Generate)
			{
				buildings = CreateData();
			}
			else
			{
				buildings = ReadBuildings();
			}

			SetCenterDateSet(buildings);

			Console.Out.WriteLine("Create Nodes");
			Node root = CreateNode(buildings, null);
			CleanTag(root);
			Tree tree = new Tree();
			tree.Root = root;
			Console.Out.WriteLine("Writing Tree");
			using (FileStream file = File.Open(string.Format(TreeOutputFileFormat, DirectoryOutput), FileMode.Create, FileAccess.ReadWrite))
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
			output.Normals = new HyperPoint<float>[verticesCount];
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
				Array.Copy(nodeDataList[i].Normals, 0, output.Normals, verticesI, nodeDataList[i].Normals.Length);
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
			data.Vertices = new HyperPoint<float>[building.Polygon.Count * 4]; //high and low point for both points
			data.Normals = new HyperPoint<float>[building.Polygon.Count * 4]; //high and low point for both points
			data.Indexes = new int[building.Polygon.Count * 2 * 3]; //2 triangles per square, 3 points per triangle
			for (int i = 0; i < building.Polygon.Count; i++)
			{
				int j = i;
				if (i == 0)
				{
					j = building.Polygon.Count;
				}
				
				int currentLow = i*4 + 0;
				int currentHigh = i*4 + 1;

				int LastLow = i*4 + 2;
				int LastHigh = i*4 + 3;

				data.Vertices[currentLow] = new HyperPoint<float>(building.Polygon[i].X, building.Polygon[i].Y, 0);
				data.Vertices[currentHigh] = new HyperPoint<float>(building.Polygon[i].X, building.Polygon[i].Y, building.Height);
				
				data.Vertices[LastLow] = new HyperPoint<float>(building.Polygon[j - 1].X, building.Polygon[j - 1].Y, 0);
				data.Vertices[LastHigh] = new HyperPoint<float>(building.Polygon[j - 1].X, building.Polygon[j - 1].Y, building.Height);

				data.Vertices[currentLow] = data.Vertices[currentLow] - CenterDataSet;
				data.Vertices[currentHigh] = data.Vertices[currentHigh] - CenterDataSet;
				data.Vertices[LastLow] = data.Vertices[LastLow] - CenterDataSet;
				data.Vertices[LastHigh] = data.Vertices[LastHigh] - CenterDataSet;

				HyperPoint<float> sizeX = data.Vertices[currentHigh] - data.Vertices[currentLow];
				HyperPoint<float> sizeY = data.Vertices[LastLow] - data.Vertices[currentLow];
				HyperPoint<float> normal = HyperPoint<float>.Cross3D(sizeX.Normilize(), sizeY.Normilize()).Normilize();

				data.Normals[currentLow] = normal;
				data.Normals[currentHigh] = normal;

				data.Normals[LastLow] = normal;
				data.Normals[LastHigh] = normal;

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
			return string.Format(NodeFilenameFormat, DirectoryOutput, FileCount++);
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
			for (int i = 0; i < generateSizeX; i++)
			{
				for (int j = 0; j < generateSizeY; j++)
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
						output.Polygon = RemoveRepeatition(ReadGML(reader.ReadElementContentAsString()));
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

		public static List<HyperPoint<float>> RemoveRepeatition(List<HyperPoint<float>> polygon)
		{
			for (int i = polygon.Count - 1; i >= 1; i--)
			{
				if (polygon[i] == polygon[i - 1])
				{
					polygon.RemoveAt(i);
				}
			}
			if(polygon[0] == polygon[polygon.Count-1])
			{
				polygon.RemoveAt(polygon.Count - 1);
			}
			return polygon;
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

		public static void SetCenterDateSet(List<Building> buildings)
		{
			if (FindCenterDataSet)
			{
				HyperPoint<float> min = GetMinPoint(buildings);
				HyperPoint<float> max = GetMaxPoint(buildings);

				CenterDataSet = (max - min);
				CenterDataSet = CenterDataSet * (1f / 2f);
				CenterDataSet = CenterDataSet + min;
			}
			else
			{
				CenterDataSet = new HyperPoint<float>(0, 0, 0);
			}
		}

		public static void ParseCommandLine(string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "--triangles" || args[i] == "-t")
				{
					i++;
					MaxTriangleCount = int.Parse(args[i]);
				}
				else if (args[i] == "--max-triangles" || args[i] == "-m")
				{
					MaxTriangleCount = int.MaxValue;
				}
				else if (args[i] == "--output" || args[i] == "-o")
				{
					i++;
					DirectoryOutput = args[i];
				}
				else if (args[i] == "--input" || args[i] == "-i")
				{
					i++;
					InputFilename = args[i];
				}
				else if (args[i] == "--generate-data" || args[i] == "-g")
				{
					Generate = true;
					i++;
					generateSizeX = int.Parse(args[i]);
					i++;
					generateSizeY = int.Parse(args[i]);
				}
				else if(args[i] == "--center-data" || args[i] == "-c")
				{
					FindCenterDataSet = true;
				}
			}
		}
	}
}
