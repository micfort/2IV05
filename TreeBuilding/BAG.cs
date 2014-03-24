using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using CG_2IV05.Common.BAG;
using micfort.GHL.Math2;
using CG_2IV05.Common;
using CG_2IV05.Common.Element;

namespace CG_2IV05.TreeBuilding
{
	class BAG
	{
		public static List<Building> ReadBuildings(string filename)
		{
			int buildingCount = 0;
			List<Building> output = new List<Building>();
			XmlReader reader = XmlReader.Create(filename);
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "Building")
					{
						buildingCount++;
						if (buildingCount % 10000 == 0)
						{
							Console.Out.WriteLine("Processing building {0:N0}", buildingCount);
						}
						output.Add(ReadBuilding(reader));
					}
				}
			}
			return output;
		}

		private static Building ReadBuilding(XmlReader reader)
		{
			List<HyperPoint<float>> polygon = null;
			float height = 0;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "height")
					{
						height = reader.ReadElementContentAsFloat();
					}
					else if (reader.Name == "gmlbase64")
					{
						polygon = PolygonHelper.RemoveRepeatition(ReadGML(reader.ReadElementContentAsString()));
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.Name == "Building")
					{
						return new Building(polygon, height);
					}
				}
			}
			return null;
		}

		private static List<HyperPoint<float>> ReadGML(string gml64)
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
