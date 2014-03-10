using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CG_2IV05.Common.Element;
using micfort.GHL.Math2;
using micfort.GHL.Serialization;
using OsmSharp.Osm;
using OsmSharp.Osm.PBF.Streams;
using Way = OsmSharp.Osm.Way;

namespace CG_2IV05.Common.OSM
{
	public class OSM
	{
		static HyperPoint<float> min = new HyperPoint<float>(5.4231f, 51.4046f);
		static HyperPoint<float> max = new HyperPoint<float>(5.5302f, 51.4983f);

		public static List<IElement> Read()
		{
			List<Road> roads = new List<Road>();
			FileStream file = File.OpenRead(@"D:\S120397\School\2IV05 ACCG\netherlands-latest.osm.pbf");
			Dictionary<long, NodeRD> nodes = new Dictionary<long, NodeRD>();
			PBFOsmStreamSource source = new PBFOsmStreamSource(file);
			source.Initialize();
			int nodeCount = 0;
			int elementCount = 0;
			while (source.MoveNext())
			{
				var geo = source.Current();
				if (geo.Type == OsmGeoType.Node)
				{
					if (elementCount % 1000000 == 0)
					{
						Console.Out.WriteLine("Processing node element {0:N0}", elementCount);
					}
					if (geo.Id != null)
					{
						OsmSharp.Osm.Node node = (OsmSharp.Osm.Node)geo;
						if (node.Latitude != null && node.Longitude != null)
						{
							if (min.X < node.Longitude && node.Longitude < max.X
								&& min.Y < node.Latitude && node.Latitude < max.Y)
							{
								HyperPoint<double> RDCoordinate =
									new HyperPoint<double>(node.Longitude.Value, node.Latitude.Value);
								HyperPoint<float> RDCoordinateF = RDCoordinate.ConvertToRD().ConvertTo<float>();

								NodeRD nodeRD = new NodeRD()
								{
									Node = node,
									RDCoordinate = RDCoordinateF
								};
								nodes.Add(geo.Id.Value, nodeRD);
							}
						}
					}
				}
				else if (geo.Type == OsmGeoType.Way)
				{
					if (elementCount % 1000000 == 0)
					{
						Console.Out.WriteLine("Processing way element {0}", elementCount);
					}
					Way way = (Way)geo;
					List<string> keys = new List<string>() { "highway", "sidewalk" };
					if (way.Tags != null && keys.Any(way.Tags.ContainsKey) && Road.ExistInDataset(way, nodes))
					{
						roads.Add(new Road(way, nodes));
					}
				}
				else if(geo.Type == OsmGeoType.Relation)
				{
					if (elementCount % 1000000 == 0)
					{
						Console.Out.WriteLine("Processing relation element {0}", elementCount);
					}
				}
				elementCount++;
			}
			return roads.ConvertAll(x => (IElement)x);
		}

		public static List<Road> ReadFiltered(Stream input)
		{
			List<Road> roads = SerializableType<List<Road>>.DeserializeFromStream(input, GHLBinarySerializableTypeEngine.GHLBinairSerializer);
			roads.ForEach(x => PolygonHelper.RemoveRepeatition(x.Points));
			return roads;
		}

		public static void Filter(Stream input, Stream output)
		{
			List<Road> roads = new List<Road>();
			Dictionary<long, NodeRD> nodes = new Dictionary<long, NodeRD>();
			PBFOsmStreamSource source = new PBFOsmStreamSource(input);
			source.Initialize();
			int elementCount = 0;
			while (source.MoveNext())
			{
				var geo = source.Current();
				if (geo.Type == OsmGeoType.Node)
				{
					if (elementCount % 1000000 == 0)
					{
						Console.Out.WriteLine("Processing node element {0:N0}", elementCount);
					}
					if (geo.Id != null)
					{
						OsmSharp.Osm.Node node = (OsmSharp.Osm.Node)geo;
						if (node.Latitude != null && node.Longitude != null)
						{
							if (min.X < node.Longitude && node.Longitude < max.X
								&& min.Y < node.Latitude && node.Latitude < max.Y)
							{
								HyperPoint<double> RDCoordinate =
									new HyperPoint<double>(node.Longitude.Value, node.Latitude.Value);
								HyperPoint<float> RDCoordinateF = RDCoordinate.ConvertToRD().ConvertTo<float>();

								NodeRD nodeRD = new NodeRD()
								{
									Node = node,
									RDCoordinate = RDCoordinateF
								};
								nodes.Add(geo.Id.Value, nodeRD);
							}
						}
					}
				}
				else if (geo.Type == OsmGeoType.Way)
				{
					if (elementCount % 1000000 == 0)
					{
						Console.Out.WriteLine("Processing way element {0:N0}", elementCount);
					}
					Way way = (Way)geo;
					List<string> keys = new List<string>() { "highway", "sidewalk" };
					if (way.Tags != null && keys.Any(way.Tags.ContainsKey) && Road.ExistInDataset(way, nodes))
					{
						roads.Add(new Road(way, nodes));
					}
				}
				else if (geo.Type == OsmGeoType.Relation)
				{
					if (elementCount % 1000000 == 0)
					{
						Console.Out.WriteLine("Processing relation element {0:N0}", elementCount);
					}
				}
				elementCount++;
			}
			Console.Out.WriteLine("Finshed reading, saving");
			SerializableType<List<Road>>.SerializeToStream(roads, output, GHLBinarySerializableTypeEngine.GHLBinairSerializer);
		}
	}
}
