extern alias osm;
using System.IO;
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
				   CheckKey(Tags, "landcover", "grass") || CheckKey(Tags, "natural", "water") || CheckKey(Tags, "highway", "pedestrian", true);
		}

		public bool CheckKey(TagsCollectionBase Tags, string key, string value, bool checkArea = false)
		{
			if (Tags.ContainsKey(key) && Tags[key] == value)
			{
				if(checkArea)
				{
					if(CheckKey(Tags, "area", "yes"))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		public bool CheckPolyAcceptance(List<HyperPoint<float>> poly)
		{
			return poly.Count > 2;
		}

		#endregion

		#region Implementation of IElementFactory

		public IElement ReadFromStream(Stream stream)
		{
			List<HyperPoint<float>> poly = PolygonHelper.ReadPolyFromStream(stream);
			TagsCollectionBase tags = TagsCollectionStream.ReadTagsCollectionFromStream(stream);
			return new LandUse(tags, poly);
		}

		public int FactoryID
		{
			get { return FactoryIDs.LandUseID; }
		}

		public IElement Merge(List<IElement> elements)
		{
			throw new NotImplementedException();
		}

		public bool CanMerge(List<IElement> elements)
		{
			return false;
		}

		public bool RemoveItem(IElement element, int height)
		{
			return false;
		}

		#endregion
	}

	public class LandUse: IOSMWayElement
	{
		private TagsCollectionBase _tagsCollection;
		private List<HyperPoint<float>> _points;

		public LandUse(Way way, List<HyperPoint<float>> poly)
		{
			_tagsCollection = way.Tags;
			this._points = poly;
		}

		public LandUse(TagsCollectionBase tags, List<HyperPoint<float>> poly)
		{
			_tagsCollection = tags;
			this._points = poly;
		}

		public List<HyperPoint<float>> Polygon
		{
			get { return _points; }
			set { _points = value; }
		}

		public TagsCollectionBase TagsCollection
		{
			get { return _tagsCollection; }
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
			List<List<HyperPoint<float>>> polys = PolygonHelper.CreateSimplePolygons(_points);

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

		public IElement GetSimplifiedVersion(int height)
		{
			//_points = PolygonHelper.CreateConvexHull(_points);
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
			if (_tagsCollection.ContainsKey("landuse"))
			{
				texture = textureInfo.GetTexture("landuse", _tagsCollection["landuse"]);
			}
			else if (_tagsCollection.ContainsKey("landcover"))
			{
				texture = textureInfo.GetTexture("landcover", _tagsCollection["landcover"]);
			}
			else if (_tagsCollection.ContainsKey("natural"))
			{
				texture = textureInfo.GetTexture("natural", _tagsCollection["natural"]);
			}
			else if (_tagsCollection.ContainsKey("highway"))
			{
				texture = textureInfo.GetTexture("highway", _tagsCollection["highway"]);
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

		public void SaveToStream(Stream stream)
		{
			PolygonHelper.WriteToStream(stream, _points);
			TagsCollectionStream.WriteToStream(_tagsCollection, stream);
		}

		public int FactoryID
		{
			get { return FactoryIDs.LandUseID; }
		}

		#endregion
	}
}
