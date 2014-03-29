extern alias osm;
using osm::OsmSharp.Osm;
using osm::OsmSharp.Collections;
using osm::OsmSharp.Osm.PBF.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CG_2IV05.Common.Element;
using micfort.GHL.Math2;
using micfort.GHL.Serialization;

using Way = osm::OsmSharp.Osm.Way;

namespace CG_2IV05.Common.OSM
{
	public class OSM
	{
		private static List<IOSMWayFactory> factories = new List<IOSMWayFactory>() {new LandUseFactory(), new Road2Factory()};

		public static void Read(Stream file, Action<IElement> handler)
		{
			int nodeCount = CountNodes(file);
			file.Position = 0;
			Dictionary<long, HyperPoint<float>> nodes = new Dictionary<long, HyperPoint<float>>(nodeCount);
			PBFOsmStreamSource source = new PBFOsmStreamSource(file);
			source.Initialize();
			int elementCount = 0;
			while (source.MoveNext())
			{
				var geo = source.Current();
				if (geo.Type == OsmGeoType.Node)
				{
					if (elementCount % 100000 == 0)
					{
						Console.Out.WriteLine("Processing node element {0:N0}", elementCount);
					}
					if (geo.Id != null)
					{
						osm::OsmSharp.Osm.Node node = (osm::OsmSharp.Osm.Node)geo;
						HyperPoint<double> RDCoordinate =
							new HyperPoint<double>(node.Longitude.Value, node.Latitude.Value);
						HyperPoint<float> RDCoordinateF = RDCoordinate.ConvertToRD().ConvertTo<float>();

						nodes.Add(geo.Id.Value, RDCoordinateF);
					}
				}
				else if (geo.Type == OsmGeoType.Way)
				{
					if (elementCount % 100000 == 0)
					{
						Console.Out.WriteLine("Processing way element {0:N0}", elementCount);
					}
					Way way = (Way)geo;
					if (way.Tags != null && way.Nodes.Count > 0)
					{
						IOSMWayFactory factory = factories.Find(x => x.CheckKeyAcceptance(way.Tags));
						if (factory != null)
						{
							List<HyperPoint<float>> wayCoordinates = GetCoordinates(way.Nodes, nodes);
							if (factory.CheckPolyAcceptance(wayCoordinates))
							{
								IOSMWayElement element = factory.Create(way, wayCoordinates);
								handler(element);
							}
						}
					}
				}
				else if (geo.Type == OsmGeoType.Relation)
				{
					if (elementCount % 100000 == 0)
					{
						Console.Out.WriteLine("Processing relation element {0:N0}", elementCount);
					}
				}
				elementCount++;
			}
		}

		public static List<IElement> Read(Stream file)
		{
			List<IElement> elements = new List<IElement>();
			Dictionary<long, HyperPoint<float>> nodes = new Dictionary<long, HyperPoint<float>>();
			PBFOsmStreamSource source = new PBFOsmStreamSource(file);
			source.Initialize();
			int nodeCount = 0;
			int elementCount = 0;
			while (source.MoveNext())
			{
				var geo = source.Current();
				if (geo.Type == OsmGeoType.Node)
				{
					if (elementCount % 100000 == 0)
					{
						Console.Out.WriteLine("Processing node element {0:N0}", elementCount);
					}
					if (geo.Id != null)
					{
						osm::OsmSharp.Osm.Node node = (osm::OsmSharp.Osm.Node)geo;
						HyperPoint<double> RDCoordinate =
							new HyperPoint<double>(node.Longitude.Value, node.Latitude.Value);
						HyperPoint<float> RDCoordinateF = RDCoordinate.ConvertToRD().ConvertTo<float>();

						nodes.Add(geo.Id.Value, RDCoordinateF);
					}
				}
				else if (geo.Type == OsmGeoType.Way)
				{
					if (elementCount % 100000 == 0)
					{
						Console.Out.WriteLine("Processing way element {0:N0}", elementCount);
					}
					Way way = (Way)geo;
					if (way.Tags != null && way.Nodes.Count > 0)
					{
						IOSMWayFactory factory = factories.Find(x => x.CheckKeyAcceptance(way.Tags));
						if (factory != null)
						{
							List<HyperPoint<float>> wayCoordinates = GetCoordinates(way.Nodes, nodes);
							if(factory.CheckPolyAcceptance(wayCoordinates))
							{
								elements.Add(factory.Create(way, wayCoordinates));
							}
						}
					}
				}
				else if(geo.Type == OsmGeoType.Relation)
				{
					if (elementCount % 100000 == 0)
					{
						Console.Out.WriteLine("Processing relation element {0:N0}", elementCount);
					}
				}
				elementCount++;
			}
			return elements;
		}

		public static List<HyperPoint<float>> GetCoordinates(List<long> nodesWay, Dictionary<long, HyperPoint<float>> nodes)
		{
			List<HyperPoint<float>>  points = new List<HyperPoint<float>>();
			nodesWay.ForEach(x => points.Add(nodes[x]));
			PolygonHelper.RemoveRepeatition(points);
			return points;
		}

		private static int CountNodes(Stream stream)
		{
			int count = 0;
			PBFOsmStreamSource source = new PBFOsmStreamSource(stream);
			source.Initialize();
			while (source.MoveNext())
			{
				var geo = source.Current();
				if (geo.Type == OsmGeoType.Node)
				{
					if (geo.Id != null)
					{
						count++;
						if (count % 100000 == 0)
						{
							Console.Out.WriteLine("Counting node element {0:N0}", count);
						}
					}
				}
			}
			return count;
		}
	}
}
