﻿using System;
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

			data.Normals = new HyperPoint<float>[8];

			#region vertices

			data.Normals[0] = new HyperPoint<float>(1, 1, 1, 1);
			data.Normals[1] = new HyperPoint<float>(1, 1, -1, 1);
			data.Normals[2] = new HyperPoint<float>(1, -1, 1, 1);
			data.Normals[3] = new HyperPoint<float>(1, -1, -1, 1);
			data.Normals[4] = new HyperPoint<float>(-1, 1, 1, 1);
			data.Normals[5] = new HyperPoint<float>(-1, 1, -1, 1);
			data.Normals[6] = new HyperPoint<float>(-1, -1, 1, 1);
			data.Normals[7] = new HyperPoint<float>(-1, -1, -1, 1);

			#endregion

			data.TextCoord = new HyperPoint<float>[8];

			#region texture coordinates

			data.TextCoord[0] = new HyperPoint<float>(1, 1);
			data.TextCoord[1] = new HyperPoint<float>(1, 1);
			data.TextCoord[2] = new HyperPoint<float>(1, -1);
			data.TextCoord[3] = new HyperPoint<float>(1, -1);
			data.TextCoord[4] = new HyperPoint<float>(-1, 1);
			data.TextCoord[5] = new HyperPoint<float>(-1, 1);
			data.TextCoord[6] = new HyperPoint<float>(-1, -1);
			data.TextCoord[7] = new HyperPoint<float>(-1, -1);

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
			Assert.AreEqual(data.Normals.Length, data2.Normals.Length);
			Assert.AreEqual(data.Indexes.Length, data2.Indexes.Length);

			for (int i = 0; i < data.Vertices.Length; i++)
			{
				Assert.AreEqual(data.Vertices[i], data2.Vertices[i]);
			}

			for (int i = 0; i < data.Normals.Length; i++)
			{
				Assert.AreEqual(data.Normals[i], data2.Normals[i]);
			}

			for (int i = 0; i < data.TextCoord.Length; i++)
			{
				Assert.AreEqual(data.TextCoord[i], data2.TextCoord[i]);
			}

			for (int i = 0; i < data.Indexes.Length; i++)
			{
				Assert.AreEqual(data.Indexes[i], data2.Indexes[i]);
			}

			File.Delete("test_data");
		}

		[Test]
		public void TestRaw()
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

			data.Normals = new HyperPoint<float>[8];

			#region vertices

			data.Normals[0] = new HyperPoint<float>(1, 1, 1, 1);
			data.Normals[1] = new HyperPoint<float>(1, 1, -1, 1);
			data.Normals[2] = new HyperPoint<float>(1, -1, 1, 1);
			data.Normals[3] = new HyperPoint<float>(1, -1, -1, 1);
			data.Normals[4] = new HyperPoint<float>(-1, 1, 1, 1);
			data.Normals[5] = new HyperPoint<float>(-1, 1, -1, 1);
			data.Normals[6] = new HyperPoint<float>(-1, -1, 1, 1);
			data.Normals[7] = new HyperPoint<float>(-1, -1, -1, 1);

			#endregion

			data.TextCoord = new HyperPoint<float>[8];

			#region texture coordinates

			data.TextCoord[0] = new HyperPoint<float>(1, 1);
			data.TextCoord[1] = new HyperPoint<float>(1, 1);
			data.TextCoord[2] = new HyperPoint<float>(1, -1);
			data.TextCoord[3] = new HyperPoint<float>(1, -1);
			data.TextCoord[4] = new HyperPoint<float>(-1, 1);
			data.TextCoord[5] = new HyperPoint<float>(-1, 1);
			data.TextCoord[6] = new HyperPoint<float>(-1, -1);
			data.TextCoord[7] = new HyperPoint<float>(-1, -1);

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

			NodeDataRaw data2;

			using (FileStream stream = File.Open("test_data", FileMode.Open, FileAccess.Read))
			{
				data2 = NodeDataRaw.ReadFromStream(stream);
			}

			Assert.AreEqual(data.Vertices.Length*3, data2.Vertices.Length);
			Assert.AreEqual(data.Normals.Length*3, data2.Normals.Length);
			Assert.AreEqual(data.TextCoord.Length*2, data2.TextCoord.Length);
			Assert.AreEqual(data.Indexes.Length, data2.Indexes.Length);

			for (int i = 0; i < data.Vertices.Length; i++)
			{
				Assert.AreEqual(data.Vertices[i][0], data2.Vertices[i*3 + 0]);
				Assert.AreEqual(data.Vertices[i][1], data2.Vertices[i * 3 + 1]);
				Assert.AreEqual(data.Vertices[i][2], data2.Vertices[i * 3 + 2]);
			}

			for (int i = 0; i < data.Normals.Length; i++)
			{
				Assert.AreEqual(data.Vertices[i][0], data2.Normals[i * 3 + 0]);
				Assert.AreEqual(data.Vertices[i][1], data2.Normals[i * 3 + 1]);
				Assert.AreEqual(data.Vertices[i][2], data2.Normals[i * 3 + 2]);
			}

			for (int i = 0; i < data.TextCoord.Length; i++)
			{
				Assert.AreEqual(data.TextCoord[i][0], data2.TextCoord[i * 2 + 0]);
				Assert.AreEqual(data.TextCoord[i][1], data2.TextCoord[i * 2 + 1]);
			}

			for (int i = 0; i < data.Indexes.Length; i++)
			{
				Assert.AreEqual(data.Indexes[i], data2.Indexes[i]);
			}

			File.Delete("test_data");
		}
    }
}
