using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CG_2IV05.Common.EarClipping;
using CG_2IV05.Common.Element;
using micfort.GHL.Math2;

namespace CG_2IV05.Common.BAG
{
	public class BuildingFactory: IElementFactory
	{
		#region Implementation of IElementFactory

		public IElement ReadFromStream(Stream stream)
		{
			float height = BinaryToStream.ReadFloatFromStream(stream);
			List<HyperPoint<float>> poly = PolygonHelper.ReadPolyFromStream(stream);
			return new Building(poly, height);
		}

		public int FactoryID
		{
			get { return FactoryIDs.BuildingID; }
		}

		public IElement Merge(List<IElement> elements)
		{
			if(elements.Any(x => !(x is Building)))
				throw new ArgumentException("elements should be buildings", "elements");

			List<Building> buildings = elements.ConvertAll(x => x as Building);
			List<HyperPoint<float>> points = buildings.Aggregate(new List<HyperPoint<float>>(), (list, building) =>
				                                                                                    {
					                                                                                    list.AddRange(building.Polygon);
					                                                                                    return list;
				                                                                                    });
			List<HyperPoint<float>> convex = PolygonHelper.CreateConvexHull(points);
			float height = buildings.Average(x => x.Height);
			return new Building(convex, height);
		}

		public bool CanMerge(List<IElement> elements)
		{
			return true;
		}

		public bool RemoveItem(IElement element, int height)
		{
			return false;
		}

		#endregion
	}

	public class Building : IElement
	{
		public float Height { get; private set; }
		public List<HyperPoint<float>> Polygon
		{
			get { return _polygon; }
			private set
			{
				_polygon = value;
				RecalculateReferencePoint();
			}
		}


		public bool FinalElement { get { return true; } }

        private int scorePointIndex1;
        private int scorePointIndex2;

		public Building(List<HyperPoint<float>> polygon, float height)
		{
			this.Polygon = polygon;
			this.Height = height;
			score = new ScoreKey(0f);
		}

        private ScoreKey score = new ScoreKey(float.MaxValue);
		private List<HyperPoint<float>> _polygon;

		public ScoreKey Score
        {
            get { return score; }

        }

		public int TriangleCount
		{
			get
			{
				return Polygon.Count * 2 + (Polygon.Count - 2);
			}
		}

		public HyperPoint<float> Min
		{
			get
			{
				float xMin = Polygon.Min(y => y.X);
				float yMin = Polygon.Min(y => y.Y);
				return new HyperPoint<float>(xMin, yMin, 0); 
			}
		}
		public HyperPoint<float> Max
		{
			get
			{
				float xMax = Polygon.Max(y => y.X);
				float yMax = Polygon.Max(y => y.Y);
				return new HyperPoint<float>(xMax, yMax, 0);
			}
		}

		public HyperPoint<float> ReferencePoint { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="building"></param>
		/// <returns></returns>
		/// <remarks>O(P)</remarks>
		public NodeData CreateData(HyperPoint<float> centerDataSet, TextureInfo textureInfo)
		{
			if(!PolygonHelper.IsSimplePolygon(Polygon))
			{
				return new NodeData()
					       {
						       Indexes = new int[0],
							   Vertices = new HyperPoint<float>[0],
							   TextCoord = new HyperPoint<float>[0],
							   Normals = new HyperPoint<float>[0]
					       };
			}
			if(Polygon.Count < 3)
			{
				return new NodeData()
					       {
						       Indexes = new int[0],
						       Vertices = new HyperPoint<float>[0],
						       TextCoord = new HyperPoint<float>[0],
						       Normals = new HyperPoint<float>[0]
					       };
			}
			NodeData data = new NodeData();
			data.Vertices = new HyperPoint<float>[Polygon.Count * 4 + Polygon.Count]; //high and low point for both points and one extra for each top
			data.Normals = new HyperPoint<float>[Polygon.Count * 4 + Polygon.Count]; //high and low point for both points and one extra for each top
			data.Indexes = new int[Polygon.Count * 2 * 3 + (Polygon.Count - 2) * 3]; //2 triangles per square, 3 points per triangle + (n-2) triangles for triangulation
			data.TextCoord = new HyperPoint<float>[Polygon.Count * 4 + Polygon.Count]; //high and low point for both points and one extra for each top

			HyperPoint<float>[] roofVertices = new HyperPoint<float>[Polygon.Count];
			HyperPoint<float> normalRoof = new HyperPoint<float>(0, 0, 1);

			HyperPoint<float> textureItem = textureInfo.Buildings[0];
			HyperPoint<float> roof = textureInfo.Roof[0];
			HyperPoint<float> min = this.Min;
			HyperPoint<float> max = this.Max;
			HyperPoint<float> size = max - min;

			for (int i = 0; i < Polygon.Count; i++)
			{
				int j = i;
				if (i == 0)
				{
					j = Polygon.Count;
				}

				int currentLow = i * 4 + 0;
				int currentHigh = i * 4 + 1;

				int LastLow = i * 4 + 2;
				int LastHigh = i * 4 + 3;

				#region Vertices
				data.Vertices[currentLow] = new HyperPoint<float>(Polygon[i].X, Polygon[i].Y, 0);
				data.Vertices[currentHigh] = new HyperPoint<float>(Polygon[i].X, Polygon[i].Y, Height);

				data.Vertices[LastLow] = new HyperPoint<float>(Polygon[j - 1].X, Polygon[j - 1].Y, 0);
				data.Vertices[LastHigh] = new HyperPoint<float>(Polygon[j - 1].X, Polygon[j - 1].Y, Height);

				data.Vertices[currentLow] = data.Vertices[currentLow] - centerDataSet;
				data.Vertices[currentHigh] = data.Vertices[currentHigh] - centerDataSet;
				data.Vertices[LastLow] = data.Vertices[LastLow] - centerDataSet;
				data.Vertices[LastHigh] = data.Vertices[LastHigh] - centerDataSet;
				#endregion vertices

				#region Normals

				HyperPoint<float> distanceX = data.Vertices[currentHigh] - data.Vertices[currentLow];
				HyperPoint<float> distanceY = data.Vertices[LastLow] - data.Vertices[currentLow];
				HyperPoint<float> normal = HyperPoint<float>.Cross3D(distanceX.Normilize(), distanceY.Normilize()).Normilize();

				data.Normals[currentLow] = normal;
				data.Normals[currentHigh] = normal;

				data.Normals[LastLow] = normal;
				data.Normals[LastHigh] = normal;

				#endregion

				#region TextCoord

				data.TextCoord[currentLow] = textureInfo.GetLeftBottom(textureItem);
				data.TextCoord[currentHigh] = textureInfo.GetLeftTop(textureItem);
				data.TextCoord[LastLow] = textureInfo.GetRightBottom(textureItem);
				data.TextCoord[LastHigh] = textureInfo.GetRightTop(textureItem);

				#endregion

				#region Indexes

				data.Indexes[i * 2 * 3 + 3 * 0 + 0] = currentLow;
				data.Indexes[i * 2 * 3 + 3 * 0 + 1] = currentHigh;
				data.Indexes[i * 2 * 3 + 3 * 0 + 2] = LastLow;

				data.Indexes[i * 2 * 3 + 3 * 1 + 0] = LastLow;
				data.Indexes[i * 2 * 3 + 3 * 1 + 1] = currentHigh;
				data.Indexes[i * 2 * 3 + 3 * 1 + 2] = LastHigh;

				#endregion

				#region Roof

				roofVertices[i] = new HyperPoint<float>(Polygon[i].X, Polygon[i].Y, Height) - centerDataSet;
				data.Vertices[Polygon.Count * 4 + i] = roofVertices[i];
				data.Normals[Polygon.Count * 4 + i] = normalRoof;
				data.TextCoord[Polygon.Count * 4 + i] = textureInfo.GetPoint(roof, new HyperPoint<float>((Polygon[i].X-min.X)/size.X, (Polygon[i].Y-min.Y)/size.Y));

				#endregion
			}

			int[] roofIndexes = EarClippingTriangulator.triangulatePolygon(roofVertices, Polygon.Count * 4);
			Array.Copy(roofIndexes, 0, data.Indexes, Polygon.Count * 6, roofIndexes.Length);

			return data;
		}

        public void createBuildingScore()
        {
	        if (Polygon.Count <= 4)
	        {
		        score.Score = float.MaxValue;
		        return;
	        }

	        float minDistance = float.MaxValue;
	        int minDistanceIndex1 = 0;
	        int minDistanceIndex2 = 0;

	        for (int i = 0; i < Polygon.Count; i++)
	        {
		        int j = (i + 1)%Polygon.Count;
		        HyperPoint<float> pointI = Polygon[i];
		        HyperPoint<float> pointJ = Polygon[j];

		        float distance = (pointI - pointJ).GetLengthSquared();
		        if (distance < minDistance)
		        {
			        minDistance = distance;
			        minDistanceIndex1 = i;
			        minDistanceIndex2 = j;
		        }
	        }

	        score.Score = minDistance/3;
	        scorePointIndex1 = minDistanceIndex1;
	        scorePointIndex2 = minDistanceIndex2;
        }

		public IElement GetSimplifiedVersion(int height)
        {
            if (score.Score < float.MaxValue)
            {
				Polygon = PolygonHelper.CreateConvexHull(Polygon);

	            score = new ScoreKey(float.MaxValue);
            }

            return this;
        }

		public void SaveToStream(Stream stream)
		{
			BinaryToStream.WriteToStream(Height, stream);
			PolygonHelper.WriteToStream(stream, Polygon);
		}

		public int FactoryID
		{
			get { return FactoryIDs.BuildingID; }
		}

		private void RecalculateReferencePoint()
		{
			HyperPoint<float> summation = _polygon.Aggregate(new HyperPoint<float>(2), (point, hyperPoint) => point + hyperPoint);
			HyperPoint<float> center = summation * (1f/_polygon.Count);
			ReferencePoint = center;
		}
	}
}
