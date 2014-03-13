extern alias osm;
using osm::OsmSharp.Osm;
using osm::OsmSharp.Collections;
using osm::OsmSharp.Collections.Tags;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG_2IV05.Common.EarClipping;
using CG_2IV05.Common.Element;
using micfort.GHL.Math2;


namespace CG_2IV05.Common.OSM
{
	public class LandUseFactory: IOSMWayFactory
	{
		#region Implementation of IOSMWayFactory

		public IOSMWayElement Create(Way way, List<HyperPoint<float>> poly)
		{
			return new LandUse(way, poly);
		}

		public bool CheckKeyAcceptance(TagsCollectionBase Tags)
		{
			return CheckKey(Tags, "landuse", "meadow") || CheckKey(Tags, "landuse", "grass") || CheckKey(Tags, "landuse", "pasture") ||
			       CheckKey(Tags, "landcover", "grass") || CheckKey(Tags, "natural", "water");
		}

		public bool CheckKey(TagsCollectionBase Tags, string key, string value)
		{
			return Tags.ContainsKey(key) && Tags[key] == value;
		}

		public bool CheckPolyAcceptance(List<HyperPoint<float>> poly)
		{
			return poly.Count > 2;// && SimplePolygon(poly);
		}

		#endregion


	}

	public class LandUse: IOSMWayElement
	{
		private Way _way;
		private List<HyperPoint<float>> _points;

		public LandUse(Way way, List<HyperPoint<float>> poly)
		{
			_way = way;
			this._points = poly;
		}

		#region Implementation of IElement

		public bool FinalElement
		{
			get { return true; }
		}

		public int TriangleCount
		{
			get { return _points.Count - 2; }
		}

		public ScoreKey Score
		{
			get { return new ScoreKey(float.MaxValue); }
		}

		public HyperPoint<float> Min
		{
			get
			{
				float xMin = _points.Min(y => y.X);
				float yMin = _points.Min(y => y.Y);
				return new HyperPoint<float>(xMin, yMin, 0);
			}
		}
		public HyperPoint<float> Max
		{
			get
			{
				float xMax = _points.Max(y => y.X);
				float yMax = _points.Max(y => y.Y);
				return new HyperPoint<float>(xMax, yMax, 0);
			}
		}

		public HyperPoint<float> ReferencePoint
		{
			get { return _points[0]; }
		}

		public NodeData CreateData(HyperPoint<float> centerDataSet, TextureInfo textureInfo)
		{
			List<List<HyperPoint<float>>> polys = CreateSimplePolygons(_points);

			NodeData output = new NodeData();
			List<NodeData> nodeDataList = polys.ConvertAll(b => CreateData(b, centerDataSet, textureInfo));
			int verticesCount = nodeDataList.ConvertAll(x => x.Vertices.Length).Sum();
			int indexesCount = nodeDataList.ConvertAll(x => x.Indexes.Length).Sum();

			output.Vertices = new HyperPoint<float>[verticesCount];
			output.Normals = new HyperPoint<float>[verticesCount];
			output.TextCoord = new HyperPoint<float>[verticesCount];
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
				Array.Copy(nodeDataList[i].TextCoord, 0, output.TextCoord, verticesI, nodeDataList[i].TextCoord.Length);
				Array.Copy(nodeDataList[i].Indexes, 0, output.Indexes, indexI, nodeDataList[i].Indexes.Length);
				verticesI += nodeDataList[i].Vertices.Length;
				indexI += nodeDataList[i].Indexes.Length;
			}

			return output;
		}

		public IElement GetSimplifiedVersion(HyperPoint<float> centerDataSet, TextureInfo textureInfo)
		{
			return this;
		}

		private NodeData CreateData(List<HyperPoint<float>> polygon, HyperPoint<float> centerDataSet, TextureInfo textureInfo)
		{
			NodeData data = new NodeData();
			data.Vertices = new HyperPoint<float>[polygon.Count]; //high and low point for both points and one extra for each top
			data.Normals = new HyperPoint<float>[polygon.Count]; //high and low point for both points and one extra for each top
			data.TextCoord = new HyperPoint<float>[polygon.Count]; //high and low point for both points and one extra for each top
			data.Indexes = new int[(polygon.Count-2)*3];

			HyperPoint<float> normalRoof = new HyperPoint<float>(0, 0, 1);

			HyperPoint<float> texture;
			if (_way.Tags.ContainsKey("landuse"))
			{
				texture = textureInfo.GetTexture("landuse", _way.Tags["landuse"]);
			}
			else if (_way.Tags.ContainsKey("landuse"))
			{
				texture = textureInfo.GetTexture("landcover", _way.Tags["landcover"]);
			}
			else if (_way.Tags.ContainsKey("natural"))
			{
				texture = textureInfo.GetTexture("natural", _way.Tags["natural"]);
			}
			else
			{
				texture = textureInfo.Unknown;
			}

			HyperPoint<float> min = this.Min;
			HyperPoint<float> max = this.Max;
			HyperPoint<float> size = max - min;

			for (int i = 0; i < polygon.Count; i++)
			{
				data.Vertices[i] = new HyperPoint<float>(polygon[i].X, polygon[i].Y, 0) - centerDataSet;
				data.Normals[i] = normalRoof;
				data.TextCoord[i] = textureInfo.GetPoint(texture, new HyperPoint<float>((polygon[i].X - min.X) / size.X, (polygon[i].Y - min.Y) / size.Y));
			}

			data.Indexes = EarClippingTriangulator.triangulatePolygon(data.Vertices, 0);

			return data;
		}

		#endregion

		public static List<List<HyperPoint<float>>> CreateSimplePolygons(List<HyperPoint<float>> poly)
		{
			List<List<HyperPoint<float>>> output = new List<List<HyperPoint<float>>>();
			HyperPoint<float> crossing;
			int i, j;
			if(SimplePolygon(poly, out crossing, out i, out j))
			{
				output.Add(poly);
			}
			else
			{
				List<HyperPoint<float>> polygon1 = CreateSubPolygonMin(poly, crossing, i, j);
				List<HyperPoint<float>> polygon2 = CreateSubPolygonMin(poly, crossing, j, i);
				List<List<HyperPoint<float>>> simplePolygonList1 = CreateSimplePolygons(polygon1);
				List<List<HyperPoint<float>>> simplePolygonList2 = CreateSimplePolygons(polygon2);
				simplePolygonList1.ForEach(output.Add);
				simplePolygonList2.ForEach(output.Add);
			}
			return output;
		}


		private static List<HyperPoint<float>> CreateSubPolygonMin(List<HyperPoint<float>> complexPoly, HyperPoint<float> crossing, int from, int to)
		{
			List<HyperPoint<float>> output = new List<HyperPoint<float>>();
			while (from != to)
			{
				output.Add(complexPoly[from]);
				from--;
				if (from == -1)
				{
					from = complexPoly.Count - 1;
				}
			}
			output.Add(crossing);
			return output;
		}

		private static bool SimplePolygon(List<HyperPoint<float>> poly, out HyperPoint<float> crossing, out int oi, out int oj)
		{
			for (int i = 0; i < poly.Count; i++)
			{
				for (int j = 0; j < poly.Count; j++)
				{
					int k = i + 1;
					int l = j + 1;
					if (k == poly.Count)
						k = 0;
					if (l == poly.Count)
						l = 0;
					if (i != l && i != j && k != j)
					{
						if (Intersect(poly[i], poly[k], poly[j], poly[l], out crossing))
						{
							oi = i;
							oj = j;
							return false;
						}
					}
				}
			}
			oi = -1;
			oj = -1;
			crossing = new HyperPoint<float>(2);
			return true;
		}

		/// <summary>
		/// http://thirdpartyninjas.com/blog/2008/10/07/line-segment-intersection/
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="p3"></param>
		/// <param name="p4"></param>
		/// <returns></returns>
		private static bool Intersect(HyperPoint<float> p1, HyperPoint<float> p2, HyperPoint<float> p3, HyperPoint<float> p4, out HyperPoint<float> crossing)
		{
			float Ua = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / ((p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y));
			float Ub = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / ((p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y));
			crossing = new HyperPoint<float>(p1.X + Ua * (p2.X - p1.X), p1.Y + Ub * (p2.Y - p1.Y));
			return 0 <= Ua && Ua <= 1 && 0 <= Ub && Ub <= 1;
		}
	}
}
