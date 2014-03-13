using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CG_2IV05.Common;
using NUnit.Framework;
using CG_2IV05.Common.OSM;
using micfort.GHL.Math2;

namespace CG_2IV05.Test
{
	[TestFixture]
	public class PolygonTest
	{
		[Test]
		public void Test1()
		{
			List<HyperPoint<float>> poly =  new List<HyperPoint<float>>()
				                                {
					                                new HyperPoint<float>(0, 0),
													new HyperPoint<float>(0, 1),
													new HyperPoint<float>(1, 1),
													new HyperPoint<float>(1, 0)
				                                };
			List<List<HyperPoint<float>>> polyList = PolygonHelper.CreateSimplePolygons(poly);
			Assert.AreEqual(1, polyList.Count);

			Assert.AreEqual(4, polyList[0].Count);

			Assert.AreEqual(0, polyList[0][0].X);
			Assert.AreEqual(0, polyList[0][0].Y);

			Assert.AreEqual(0, polyList[0][1].X);
			Assert.AreEqual(1, polyList[0][1].Y);

			Assert.AreEqual(1, polyList[0][2].X);
			Assert.AreEqual(1, polyList[0][2].Y);

			Assert.AreEqual(1, polyList[0][3].X);
			Assert.AreEqual(0, polyList[0][3].Y);
		}

		[Test]
		public void Test2()
		{
			List<HyperPoint<float>> poly = new List<HyperPoint<float>>()
				                                {
					                                new HyperPoint<float>(0, 0),
													new HyperPoint<float>(1, 1),
													new HyperPoint<float>(0, 1),
													new HyperPoint<float>(1, 0)
				                                };
			List<List<HyperPoint<float>>> polyList = PolygonHelper.CreateSimplePolygons(poly);
			Assert.AreEqual(2, polyList.Count);

			Assert.AreEqual(3, polyList[0].Count);
			Assert.AreEqual(3, polyList[1].Count);

			Assert.AreEqual(0, polyList[0][0].X);
			Assert.AreEqual(0, polyList[0][0].Y);

			Assert.AreEqual(1, polyList[0][1].X);
			Assert.AreEqual(0, polyList[0][1].Y);

			Assert.AreEqual(0.5f, polyList[0][2].X);
			Assert.AreEqual(0.5f, polyList[0][2].Y);

			Assert.AreEqual(0, polyList[1][0].X);
			Assert.AreEqual(1, polyList[1][0].Y);

			Assert.AreEqual(1, polyList[1][1].X);
			Assert.AreEqual(1, polyList[1][1].Y);

			Assert.AreEqual(0.5f, polyList[1][2].X);
			Assert.AreEqual(0.5f, polyList[1][2].Y);
		}
		[Test]
		public void CrossingAtLastLine()
		{
			List<HyperPoint<float>> poly = new List<HyperPoint<float>>()
				                                {
					                                new HyperPoint<float>(1, 0),
													new HyperPoint<float>(0, 0),
													new HyperPoint<float>(1, 1),
													new HyperPoint<float>(0, 1)
				                                };
			List<List<HyperPoint<float>>> polyList = PolygonHelper.CreateSimplePolygons(poly);
			Assert.AreEqual(2, polyList.Count);

			Assert.AreEqual(3, polyList[0].Count);
			Assert.AreEqual(3, polyList[1].Count);

			Assert.AreEqual(0, polyList[0][0].X);
			Assert.AreEqual(0, polyList[0][0].Y);

			Assert.AreEqual(1, polyList[0][1].X);
			Assert.AreEqual(0, polyList[0][1].Y);

			Assert.AreEqual(0.5f, polyList[0][2].X);
			Assert.AreEqual(0.5f, polyList[0][2].Y);

			Assert.AreEqual(0, polyList[1][0].X);
			Assert.AreEqual(1, polyList[1][0].Y);

			Assert.AreEqual(1, polyList[1][1].X);
			Assert.AreEqual(1, polyList[1][1].Y);

			Assert.AreEqual(0.5f, polyList[1][2].X);
			Assert.AreEqual(0.5f, polyList[1][2].Y);
		}

		[Test]
		public void TwoCrossing()
		{
			List<HyperPoint<float>> poly = new List<HyperPoint<float>>()
				                                {
					                                new HyperPoint<float>(0, 0),
													new HyperPoint<float>(0, 1),
													new HyperPoint<float>(3, 1),
													new HyperPoint<float>(3, 0),
													new HyperPoint<float>(2, 0),
													new HyperPoint<float>(2, 2),
													new HyperPoint<float>(1, 2),
													new HyperPoint<float>(1, 0)
				                                };
			List<List<HyperPoint<float>>> polyList = PolygonHelper.CreateSimplePolygons(poly);
			Assert.AreEqual(3, polyList.Count);

			Assert.AreEqual(4, polyList[0].Count);
			Assert.AreEqual(4, polyList[1].Count);
			Assert.AreEqual(4, polyList[2].Count);
		}

		[Test]
		public void PointAtTheSameLocation()
		{
			List<HyperPoint<float>> poly = new List<HyperPoint<float>>()
				                                {
					                                new HyperPoint<float>(0, 0),
													new HyperPoint<float>(0, 1),
													new HyperPoint<float>(1, 1),
													new HyperPoint<float>(1, 2),
													new HyperPoint<float>(2, 2),
													new HyperPoint<float>(2, 1),
													new HyperPoint<float>(1, 1),
													new HyperPoint<float>(1, 0)
				                                };
			List<List<HyperPoint<float>>> polyList = PolygonHelper.CreateSimplePolygons(poly);
			Assert.AreEqual(2, polyList.Count);

			Assert.AreEqual(4, polyList[0].Count);
			Assert.AreEqual(4, polyList[1].Count);
		}
	}
}
