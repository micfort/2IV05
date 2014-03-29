extern alias osm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CG_2IV05.Common.Element;
using osm::OsmSharp.Osm;
using osm::OsmSharp.Collections.Tags;
using micfort.GHL.Math2;

namespace CG_2IV05.Common.OSM
{
	public class Road2Factory: IOSMWayFactory
	{
		private static Tuple<string, string> CreateKey(string tag, string name)
		{
			return new Tuple<string, string>(tag, name);
		} 

		private int FindRemoveHeight(string tag, string type)
		{
			Tuple<string, string> key = new Tuple<string, string>(tag, type);
			if(removeHeights.ContainsKey(key))
			{
				return removeHeights[key];
			}
			else
			{
				return int.MaxValue;
			}
		}

		private Dictionary<Tuple<string, string>, int> removeHeights = new Dictionary<Tuple<string, string>, int>()
			                                                              {
				                                                              //unknown
				                                                              {CreateKey("highway", "construction"), 1},
				                                                              {CreateKey("highway", "unsurfaced"), 1},
				                                                              //asfalt
				                                                              {CreateKey("highway", "motorway"), int.MaxValue},
				                                                              {CreateKey("highway", "trunk"), int.MaxValue},
				                                                              {CreateKey("highway", "primary"), 5},
				                                                              {CreateKey("highway", "secondary"), 4},
				                                                              {CreateKey("highway", "tertiary"), 3},
				                                                              {CreateKey("highway", "unclassified"), 1},
				                                                              {CreateKey("highway", "residential"), 1},
				                                                              {CreateKey("highway", "service"), 1},
				                                                              {CreateKey("highway", "motorway_link"), 3},
				                                                              {CreateKey("highway", "trunk_link"), 4},
				                                                              {CreateKey("highway", "primary_link"), 3},
				                                                              {CreateKey("highway", "secondary_link"), 2},
				                                                              {CreateKey("highway", "tertiary_link"), 1},
				                                                              {CreateKey("highway", "living_street"), 1},

				                                                              //cycle way
				                                                              {CreateKey("highway", "cycleway"), 1},
				                                                              //looppad
				                                                              {CreateKey("highway", "footway"), 1},
				                                                              {CreateKey("highway", "pedestrian"),1},
				                                                              //track
				                                                              {CreateKey("highway", "track"), 1},
				                                                              {CreateKey("highway", "path"), 1},
				                                                              {CreateKey("highway", "bridleway"), 1},
				                                                              //steps
				                                                              {CreateKey("highway", "steps"), 1},

			                                                              };

		#region Implementation of IOSMWayFactory

		public IOSMWayElement Create(Way way, List<HyperPoint<float>> poly)
		{
			return new Road(way, poly);
		}

		public bool CheckKeyAcceptance(TagsCollectionBase Tags)
		{
			return Tags.ContainsKey("highway") && !(Tags.ContainsKey("area") && Tags["area"] == "yes");
		}

		public bool CheckPolyAcceptance(List<HyperPoint<float>> poly)
		{
			return poly.Count > 1;
		}

		#endregion

		#region Implementation of IElementFactory

		public IElement ReadFromStream(Stream stream)
		{
			List<HyperPoint<float>> poly = PolygonHelper.ReadPolyFromStream(stream);
			TagsCollectionBase tags = TagsCollectionStream.ReadTagsCollectionFromStream(stream);
			bool useExtraPoints = BinaryToStream.ReadBoolFromStream(stream);
			return new Road2(tags, poly, useExtraPoints);
		}

		public int FactoryID
		{
			get { return FactoryIDs.RoadID; }
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
			if(!(element is Road2))
				throw new ArgumentException("element should be a road2", "element");

			Road2 road = element as Road2;
			return FindRemoveHeight("highway", road.TagsCollection["highway"]) <= height;
		}

		#endregion
	}

	public class Road2: IOSMWayElement
	{
		private TagsCollectionBase _tagsCollection;
		private List<HyperPoint<float>> _points;
		private List<HyperPoint<float>> _pointsExtra;
		private bool UseExtraPoints { get; set; }

		public Road2(Way way, List<HyperPoint<float>> poly, bool useExtraPoints)
		{
			UseExtraPoints = useExtraPoints;
			_tagsCollection = way.Tags;
			_points = poly;
			_pointsExtra = new List<HyperPoint<float>>(poly);
			InsertSteps(_pointsExtra, 2.0f, 3.0f);
		}

		public Road2(TagsCollectionBase collection, List<HyperPoint<float>> poly, bool useExtraPoints)
		{
			UseExtraPoints = useExtraPoints;
			_tagsCollection = collection;
			_points = poly;
			_pointsExtra = new List<HyperPoint<float>>(poly);
			InsertSteps(_pointsExtra, 2.0f, 3.0f);
		}

		public TagsCollectionBase TagsCollection
		{
			get { return _tagsCollection; }
		}

		#region Implementation of IOSMWayElement

		public bool FinalElement
		{
			get { return true; }
		}

		public int TriangleCount
		{
			get { return ((UseExtraPoints?_pointsExtra.Count:_points.Count) - 1) * 2; }
		}

		public ScoreKey Score { get; private set; }

		public HyperPoint<float> Min
		{
			get { return new HyperPoint<float>(_points.Min(x => x.X), _points.Min(x => x.Y)); }
		}

		public HyperPoint<float> Max
		{
			get { return new HyperPoint<float>(_points.Max(x => x.X), _points.Max(x => x.Y)); }
		}

		public HyperPoint<float> ReferencePoint
		{
			get { return _points[0]; }
		}

		public NodeData CreateData(HyperPoint<float> centerDataSet, TextureInfo textureInfo)
		{
			NodeData data = new NodeData();
			data.Vertices = new HyperPoint<float>[_points.Count*2];
			data.Normals = new HyperPoint<float>[_points.Count*2];
			data.TextCoord = new HyperPoint<float>[_points.Count*2];
			data.Indexes = new int[TriangleCount*3];
			
			HyperPoint<float> up = new HyperPoint<float>(0, 0, 1);
			HyperPoint<float> textureItem = textureInfo.GetTexture("highway", _tagsCollection["highway"]);

			for (int i = 0; i < _points.Count; i++)
			{
				float width = 2;
				if (_tagsCollection.ContainsKey("lanes"))
				{
					int lanes;
					if (int.TryParse(_tagsCollection["lanes"], out lanes))
					{
						width = 2.5f*lanes;
					}
				}

				HyperPoint<float> cross;
				HyperPoint<float> current = new HyperPoint<float>(_points[i], 0);
				if(i == 0)
				{
					HyperPoint<float> next = new HyperPoint<float>(_points[i + 1], 0);

					HyperPoint<float> diff = next - current;
					cross = HyperPoint<float>.Cross3D(up, diff.Normilize());
				}
				else if(i == _points.Count-1)
				{
					HyperPoint<float> last = new HyperPoint<float>(_points[i - 1], 0);

					HyperPoint<float> diff = last - current;
					cross = HyperPoint<float>.Cross3D(diff.Normilize(), up);
				}
				else
				{
					HyperPoint<float> last = new HyperPoint<float>(_points[i - 1], 0);
					HyperPoint<float> next = new HyperPoint<float>(_points[i + 1], 0);

					HyperPoint<float> firstDiff = last - current;
					HyperPoint<float> secondDiff = next - current;
					HyperPoint<float> firstCross = HyperPoint<float>.Cross3D(firstDiff.Normilize(), up);
					HyperPoint<float> secondCross = HyperPoint<float>.Cross3D(up, secondDiff.Normilize());
					cross = (firstCross + secondCross) * (1f / 2f);
				}

				HyperPoint<float> first = current + (cross*(width/2));
				HyperPoint<float> second = current - (cross*(width/2));

				data.Vertices[i*2 + 0] = first - centerDataSet;
				data.Vertices[i*2 + 1] = second - centerDataSet;
				
				data.Normals[i*2 + 0] = new HyperPoint<float>(0, 0, 1);
				data.Normals[i*2 + 1] = new HyperPoint<float>(0, 0, 1);

				if(i % 2 == 0)
				{
					data.TextCoord[i*2 + 0] = textureInfo.GetRightTop(textureItem);
					data.TextCoord[i*2 + 1] = textureInfo.GetLeftTop(textureItem);
				}
				else
				{
					data.TextCoord[i * 2 + 0] = textureInfo.GetRightBottom(textureItem);
					data.TextCoord[i * 2 + 1] = textureInfo.GetLeftBottom(textureItem);
				}

				if(i != _points.Count-1)
				{
					int currentLeft = i * 2 + 0;
					int currentRight = i * 2 + 1;

					int NextLeft = i * 2 + 2;
					int NextRight = i * 2 + 3;

					data.Indexes[i * 6 + 0 * 3 + 0] = NextLeft;
					data.Indexes[i * 6 + 0 * 3 + 1] = currentLeft;
					data.Indexes[i * 6 + 0 * 3 + 2] = currentRight;

					data.Indexes[i * 6 + 1 * 3 + 0] = NextLeft;
					data.Indexes[i * 6 + 1 * 3 + 1] = currentRight;
					data.Indexes[i * 6 + 1 * 3 + 2] = NextRight;
				}
			}

			return data;
		}

	    public IElement GetSimplifiedVersion(int height)
	    {
		    this.UseExtraPoints = false;
			return this;
	    }

		public void SaveToStream(Stream stream)
		{
			PolygonHelper.WriteToStream(stream, _points);
			TagsCollectionStream.WriteToStream(_tagsCollection, stream);
			BinaryToStream.WriteToStream(UseExtraPoints, stream);
		}

		public int FactoryID
		{
			get { return FactoryIDs.Road2ID; }
		}

		#endregion

		private void InsertSteps(List<HyperPoint<float>> poly, float createStepSize, float maximumStep)
		{
			for (int i = 0; i < poly.Count-1; i++)
			{
				while ((poly[i] - poly[i + 1]).GetLengthSquared() > maximumStep * maximumStep)
				{
					poly.Insert(i + 1, PointInLineSegmentsWithAbsoluteLength(poly[i], poly[i + 1], createStepSize));
					i++;
				}
			}
		}

		private HyperPoint<float> PointInLineSegmentsWithAbsoluteLength(HyperPoint<float> p1, HyperPoint<float> p2, float length)
		{
			return PointInLineSegments(p1, p2, length/(p2 - p1).GetLength());
		} 
		
		private HyperPoint<float> PointInLineSegments(HyperPoint<float> p1, HyperPoint<float> p2, float U)
		{
			return p1 + (p2 - p1)*U;
		}
	}
}