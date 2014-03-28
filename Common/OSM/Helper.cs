extern alias osm;
using System;
using System.IO;
using micfort.GHL.Math2;
using osm::OsmSharp.Collections.Tags;

namespace CG_2IV05.Common.OSM
{
	public class NodeRD
	{
		public HyperPoint<float> RDCoordinate { get; set; }
		public osm::OsmSharp.Osm.Node Node { get; set; }

		public override string ToString()
		{
			return RDCoordinate.ToString();
		}
	}

	public static class CoordinateSystemConvert
	{
		public static HyperPoint<double> ConvertToRD(this HyperPoint<double> WGS84)
		{
			// http://forum.geocaching.nl/index.php?showtopic=7886&page=3
			//
			// Dim SomX
			// Dim SomY
			// Dim dF
			// Dim dL
			// Dim InputF
			// Dim InputL
			// Dim X
			// Dim Y
			//
			//
			// dF = 0.36 * (InputF - 52.15517440)
			// dL = 0.36 * (InputL - 5.38720621)
			//
			// SomX= (190094.945 * dL) + (-11832.228 * dF * dL) + (-144.221 * dF^2 * dL) + (-32.391 * dL^3) 
			//	+ (-0.705 * dF) + (-2.340 * dF^3 * dL) + (-0.608 * dF * dL^3) + (-0.008 * dL^2) 
			//	+ (0.148 * dF^2 * dL^3)
			//
			// SomY = (309056.544 * dF) + (3638.893 * dL^2) + (73.077 * dF^2 ) + (-157.984 * dF * dL^2) 
			//	+ (59.788 * dF^3 ) + (0.433 * dL) + (-6.439 * dF^2 * dL^2) + (-0.032 * dF * dL) 
			//	+ (0.092 * dL^4) + (-0.054 * dF * dL^4)
			//
			// X = 155000 + SomX
			// Y = 463000 + SomY

			double SomX;
			double SomY;
			double dF;
			double dL;
			double X;
			double Y;

			dF = 0.36 * (WGS84.Y - 52.15517440);
			dL = 0.36 * (WGS84.X - 5.38720621);

			SomX = (190094.945 * dL) + (-11832.228 * dF * dL) + (-144.221 * Math.Pow(dF, 2) * dL) + (32.391 * Math.Pow(dL, 3))
				   + (-0.705 * dF) + (-2.340 * Math.Pow(dF, 3) * dL) + (-0.608 * dF * Math.Pow(dL, 3)) + (-0.008 * Math.Pow(dL, 2))
				   + (0.148 * Math.Pow(dF, 2) * Math.Pow(dL, 3));
			SomY = (309056.544 * dF) + (3638.893 * Math.Pow(dL, 2)) + (73.077 * Math.Pow(dF, 2)) + (-157.984 * dF * Math.Pow(dL, 2))
				   + (59.788 * Math.Pow(dF, 3)) + (0.433 * dL) + (-6.439 * Math.Pow(dF, 2) * Math.Pow(dL, 2)) + (-0.032 * dF * dL)
				   + (0.092 * Math.Pow(dL, 4)) + (-0.054 * dF * Math.Pow(dL, 4));
			X = 155000 + SomX;
			Y = 463000 + SomY;

			return new HyperPoint<double>(X, Y);

		}
	}

	public static class TagsCollectionStream
	{
		public static void WriteToStream(TagsCollectionBase c, Stream outputStream)
		{
			BinaryToStream.WriteToStream(c.Count, outputStream);
			foreach (Tag tag in c)
			{
				BinaryToStream.WriteToStream(tag.Key, outputStream);
				BinaryToStream.WriteToStream(tag.Value, outputStream);
			}
		}

		public static TagsCollection ReadTagsCollectionFromStream(Stream inputStream)
		{
			TagsCollection c = new TagsCollection();
			int count = BinaryToStream.ReadIntFromStream(inputStream);
			for (int i = 0; i < count; i++)
			{
				string key = BinaryToStream.ReadStringFromStream(inputStream);
				string value = BinaryToStream.ReadStringFromStream(inputStream);
				c.Add(key, value);
			}
			return c;
		}
	}
}
