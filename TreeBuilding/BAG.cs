using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using micfort.GHL.Math2;
using CG_2IV05.Common;
using CG_2IV05.Common.Element;

namespace CG_2IV05.TreeBuilding
{
	class BAG
	{
		public static List<Building> ReadBuildings(string filename)
		{
			List<Building> output = new List<Building>();
			XmlReader reader = XmlReader.Create(filename);
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					if (reader.Name == "Building")
					{
						output.Add(ReadBuilding(reader));
					}
				}
			}
			return output;
		}

		private static Building ReadBuilding(XmlReader reader)
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
					else if (reader.Name == "gmlbase64")
					{
						output.Polygon = RemoveRepeatition(ReadGML(reader.ReadElementContentAsString()));
					}
				}
				else if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.Name == "Building")
					{
						return output;
					}
				}
			}
			return null;
		}

		private static List<HyperPoint<float>> RemoveRepeatition(List<HyperPoint<float>> polygon)
		{
			for (int i = polygon.Count - 1; i >= 1; i--)
			{
				if (polygon[i] == polygon[i - 1])
				{
					polygon.RemoveAt(i);
				}
			}
			if (polygon[0] == polygon[polygon.Count - 1])
			{
				polygon.RemoveAt(polygon.Count - 1);
			}
			return polygon;
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
