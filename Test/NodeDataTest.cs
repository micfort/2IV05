using System;
using System.IO;
using NUnit;
using CG_2IV05.Common;
using NUnit.Framework;
using micfort.GHL.Math2;

namespace CG_2IV05.Test
{
	[TestFixture]
    public class NodeDataTest
    {
		[SetUp]
		public void setup()
		{
			micfort.GHL.GHLWindowsInit.Init();
		}

		[Test]
		public void Test1()
		{
			NodeData data = new NodeData();

			data.Vertices = new HyperPoint<float>[8];

			#region vertices

			data.Vertices[0] = new HyperPoint<float>(1, 1, 1, 1);
			data.Vertices[1] = new HyperPoint<float>(1, 1, -1, 1);
			data.Vertices[2] = new HyperPoint<float>(1, -1, 1, 1);
			data.Vertices[3] = new HyperPoint<float>(1, -1, -1, 1);
			data.Vertices[4] = new HyperPoint<float>(-1, 1, 1, 1);
			data.Vertices[5] = new HyperPoint<float>(-1, 1, -1, 1);
			data.Vertices[6] = new HyperPoint<float>(-1, -1, 1, 1);
			data.Vertices[7] = new HyperPoint<float>(-1, -1, -1, 1);

			#endregion

			data.Indexes = new int[12 * 3];

			#region indexes

			int j = 0;
			data.Indexes[j++] = 0;
			data.Indexes[j++] = 1;
			data.Indexes[j++] = 2;

			data.Indexes[j++] = 3;
			data.Indexes[j++] = 4;
			data.Indexes[j++] = 6;

			data.Indexes[j++] = 7;
			data.Indexes[j++] = 8;
			data.Indexes[j++] = 0;

			data.Indexes[j++] = 1;
			data.Indexes[j++] = 2;
			data.Indexes[j++] = 3;

			data.Indexes[j++] = 4;
			data.Indexes[j++] = 5;
			data.Indexes[j++] = 6;

			data.Indexes[j++] = 7;
			data.Indexes[j++] = 8;
			data.Indexes[j++] = 0;

			data.Indexes[j++] = 1;
			data.Indexes[j++] = 2;
			data.Indexes[j++] = 3;

			data.Indexes[j++] = 4;
			data.Indexes[j++] = 5;
			data.Indexes[j++] = 6;

			data.Indexes[j++] = 7;
			data.Indexes[j++] = 8;
			data.Indexes[j++] = 0;

			data.Indexes[j++] = 1;
			data.Indexes[j++] = 2;
			data.Indexes[j++] = 3;

			data.Indexes[j++] = 4;
			data.Indexes[j++] = 5;
			data.Indexes[j++] = 6;

			data.Indexes[j++] = 7;
			data.Indexes[j++] = 8;
			data.Indexes[j++] = 0;
			#endregion

			using (FileStream stream = File.Open("test_data", FileMode.Create, FileAccess.Write))
			{
				data.SaveToStream(stream);
			}

			NodeData data2;

			using (FileStream stream = File.Open("test_data", FileMode.Open, FileAccess.Read))
			{
				data2 = NodeData.ReadFromStream(stream);
			}

			Assert.AreEqual(data.Vertices.Length, data2.Vertices.Length);
			Assert.AreEqual(data.Indexes.Length, data2.Indexes.Length);

			for (int i = 0; i < data.Vertices.Length; i++)
			{
				Assert.AreEqual(data.Vertices[i], data2.Vertices[i]);
			}

			for (int i = 0; i < data.Indexes.Length; i++)
			{
				Assert.AreEqual(data.Indexes[i], data2.Indexes[i]);
			}

			//File.Delete("test_data");
		}
    }
}
