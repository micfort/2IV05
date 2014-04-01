using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CG_2IV05.Common.Element;
using CG_2IV05.Common.OSM;
using micfort.GHL.Math2;

namespace CG_2IV05.Common.TestElement
{
	class TestElementFactory: IElementFactory
	{
		#region Implementation of IElementFactory

		private int _factoryID;

		public IElement ReadFromStream(Stream stream)
		{
			return new TestElement(PolygonHelper.ReadHyperPointFromStream(stream), PolygonHelper.ReadHyperPointFromStream(stream));
		}

		public int FactoryID
		{
			get { return _factoryID; }
		}

		public IElement Merge(List<IElement> elements)
		{
			if (elements.Any(x => !(x is TestElement)))
				throw new ArgumentException("elements should be TestElement", "elements");

			HyperPoint<float> min = new HyperPoint<float>(elements.Min(x => x.Min.X), elements.Min(x => x.Min.Y), 0);
			HyperPoint<float> max = new HyperPoint<float>(elements.Max(x => x.Max.X), elements.Max(x => x.Max.Y), 0);
			return new TestElement(min, max);
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
	public class TestElement: IElement
	{
		#region Implementation of IElement

		private ScoreKey _score;

		private HyperPoint<float> _min;

		private HyperPoint<float> _max;

		public bool FinalElement
		{
			get { return true; }
		}

		public int TriangleCount
		{
			get { return 500000; }
		}

		public ScoreKey Score
		{
			get { return _score; }
		}

		public HyperPoint<float> Min
		{
			get { return _min; }
		}

		public HyperPoint<float> Max
		{
			get { return _max; }
		}

		public HyperPoint<float> ReferencePoint
		{
			get { return (_min+_max)/2; }
		}

		public TestElement()
		{
			
		}

		public TestElement(HyperPoint<float> min, HyperPoint<float> max)
		{
			_min = min;
			_max = max;
		}

		public NodeData CreateData(HyperPoint<float> centerDataSet, TextureInfo textureInfo)
		{
			NodeData data = new NodeData();
			data.Vertices = new HyperPoint<float>[4]; //high and low point for both points and one extra for each top
			data.Normals = new HyperPoint<float>[4]; //high and low point for both points and one extra for each top
			data.TextCoord = new HyperPoint<float>[4]; //high and low point for both points and one extra for each top
			data.Indexes = new int[2*3];

			data.Vertices[0] = Min - centerDataSet;
			data.Vertices[1] = new HyperPoint<float>(_min.X, _max.Y, 0) - centerDataSet;
			data.Vertices[2] = Max - centerDataSet;
			data.Vertices[3] = new HyperPoint<float>(_max.X, _min.Y, 0) - centerDataSet;

			data.Normals[0] = new HyperPoint<float>(0, 0, 1);
			data.Normals[1] = new HyperPoint<float>(0, 0, 1);
			data.Normals[2] = new HyperPoint<float>(0, 0, 1);
			data.Normals[3] = new HyperPoint<float>(0, 0, 1);

			data.TextCoord[0] = textureInfo.GetLeftTop(textureInfo.Unknown);
			data.TextCoord[1] = textureInfo.GetLeftBottom(textureInfo.Unknown);
			data.TextCoord[2] = textureInfo.GetRightBottom(textureInfo.Unknown);
			data.TextCoord[3] = textureInfo.GetRightTop(textureInfo.Unknown);

			data.Indexes[0] = 2;
			data.Indexes[1] = 1;
			data.Indexes[2] = 0;

			data.Indexes[3] = 0;
			data.Indexes[4] = 3;
			data.Indexes[5] = 2;

			return data;
		}

		public IElement GetSimplifiedVersion(int height)
		{
			return this;
		}

		public void SaveToStream(Stream stream)
		{
			PolygonHelper.WriteToStream(stream, _min);
			PolygonHelper.WriteToStream(stream, _max);
		}

		public int FactoryID
		{
			get { return FactoryIDs.TestViewID; }
		}

		#endregion
	}
}
