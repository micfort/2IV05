using System;
using System.Collections.Generic;
using System.Linq;
using CG_2IV05.Common.Element;
using OsmSharp.Osm;
using micfort.GHL.Math2;


namespace CG_2IV05.Common.OSM
{
	public class RoadFactory: IOSMWayFactory
	{
		#region Implementation of IOSMWayFactory

		public IOSMWayElement Create(Way way, List<HyperPoint<float>> poly)
		{
			return new Road(way, poly);
		}

		public bool CheckKeyAcceptance(TagsCollectionBase Tags)
		{
			return Tags.ContainsKey("highway");
		}

		public bool CheckPolyAcceptance(List<HyperPoint<float>> poly)
		{
			return poly.Count > 1;
		}

		#endregion
	}

	public class Road: IOSMWayElement
	{
		private Way _way;
		private List<HyperPoint<float>> _points;

		public Road(Way way, List<HyperPoint<float>> poly)
		{
			_way = way;
			_points = poly;
		}

		#region Implementation of IOSMWayElement

		public bool FinalElement
		{
			get { return true; }
		}

		public int TriangleCount
		{
			get { return (_points.Count - 1)*2; }
		}

        public ScoreKey Score
        {
            get { return new ScoreKey(float.MaxValue); }
        }

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
			HyperPoint<float> textureItem = textureInfo.GetTexture("highway", _way.Tags["highway"]);

			for (int i = 0; i < _points.Count; i++)
			{
				HyperPoint<float> first;
				HyperPoint<float> second;
				if(i == 0)
				{
					HyperPoint<float> current = new HyperPoint<float>(_points[i], 0);
					HyperPoint<float> next = new HyperPoint<float>(_points[i + 1], 0);

					HyperPoint<float> diff = next - current;
					HyperPoint<float> cross = HyperPoint<float>.Cross3D(up, diff.Normilize());
					first = current + cross;
					second = current - cross;
				}
				else if(i == _points.Count-1)
				{
					HyperPoint<float> last = new HyperPoint<float>(_points[i - 1], 0);
					HyperPoint<float> current = new HyperPoint<float>(_points[i], 0);

					HyperPoint<float> diff = last - current;
					HyperPoint<float> cross = HyperPoint<float>.Cross3D(diff.Normilize(), up);
					first = current + cross;
					second = current - cross;
				}
				else
				{
					HyperPoint<float> last = new HyperPoint<float>(_points[i - 1], 0);
					HyperPoint<float> current = new HyperPoint<float>(_points[i], 0);
					HyperPoint<float> next = new HyperPoint<float>(_points[i + 1], 0);

					HyperPoint<float> firstDiff = last - current;
					HyperPoint<float> secondDiff = next - current;
					HyperPoint<float> firstCross = HyperPoint<float>.Cross3D(firstDiff.Normilize(), up);
					HyperPoint<float> secondCross = HyperPoint<float>.Cross3D(up, secondDiff.Normilize());
					HyperPoint<float> average = (firstCross + secondCross) * (1f / 2f);
					first = current + average;
					second = current - average;
				}

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

	    public IElement GetSimplifiedVersion(HyperPoint<float> centerDataSet, TextureInfo textureInfo)
	    {
	        return this;
	    }

	    #endregion
	}
}