extern alias osm;
using System;
using System.Collections.Generic;
using System.Linq;
using CG_2IV05.Common.Element;
using osm::OsmSharp.Osm;
using osm::OsmSharp.Collections.Tags;
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
			return Tags.ContainsKey("highway") && !(Tags.ContainsKey("area") && Tags["area"] == "yes");
		}

		public bool CheckPolyAcceptance(List<HyperPoint<float>> poly)
		{
			return poly.Count > 1;
		}

		#endregion
	}

	public class Road: IOSMWayElement
	{
		private int scorePointIndex;
		private Way _way;
		private List<HyperPoint<float>> _points;

		public Road(Way way, List<HyperPoint<float>> poly)
		{
			Score = new ScoreKey(float.MaxValue);
			_way = way;
			_points = poly;
			InsertSteps(_points, 2.0f, 3.0f);
			SetScore();
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
			HyperPoint<float> textureItem = textureInfo.GetTexture("highway", _way.Tags["highway"]);

			for (int i = 0; i < _points.Count; i++)
			{
				float width = 2;
				if(_way.Tags.ContainsKey("lanes"))
				{
					int lanes;
					if(int.TryParse(_way.Tags["lanes"], out lanes))
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

	    public IElement GetSimplifiedVersion(HyperPoint<float> centerDataSet, TextureInfo textureInfo)
	    {
			if (Score.Score < float.MaxValue)
			{
				HyperPoint<float> pointI = _points[scorePointIndex];
				HyperPoint<float> pointJ = _points[scorePointIndex+1];
				HyperPoint<float> newPoint = (pointI + pointJ) / 2;

				_points[scorePointIndex] = newPoint;
				_points.RemoveAt(scorePointIndex+1);

				SetScore();
			}

			return this;
	    }

	    #endregion

		private void SetScore()
		{
			if (_points.Count <= 2)
			{
				Score.Score = float.MaxValue;
				return;
			}

			float minDistance = float.MaxValue;
			int minDistanceIndex = 0;

			for (int i = 0; i < _points.Count-1; i++)
			{
				HyperPoint<float> pointI = _points[i];
				HyperPoint<float> pointJ = _points[i+1];

				float distance = (pointI - pointJ).GetLengthSquared();
				if (distance < minDistance)
				{
					minDistance = distance;
					minDistanceIndex = i;
				}
			}

			Score.Score = minDistance / 3;
			scorePointIndex = minDistanceIndex;
		}

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