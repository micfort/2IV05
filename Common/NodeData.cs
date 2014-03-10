using System;
using System.IO;
using micfort.GHL.Math2;

namespace CG_2IV05.Common
{
	public class NodeDataRaw
	{
		public float[] Vertices { get; set; }
		public float[] Normals { get; set; }
		public float[] TextCoord { get; set; }
		public int[] Indexes { get; set; }

		public static NodeDataRaw ReadFromStream(Stream stream)
		{
			NodeDataRaw output = new NodeDataRaw();
			int vertexCount = ReadInt32(stream);
			int triangleCount = ReadInt32(stream);

			output.Vertices = new float[vertexCount * 3];
			output.Normals = new float[vertexCount * 3];
			output.TextCoord = new float[vertexCount * 2];
			output.Indexes = new int[triangleCount * 3];

			byte[] vertexPointsByteBuffer = new byte[vertexCount * 3 * sizeof(float)];

			stream.Read(vertexPointsByteBuffer, 0, vertexCount * 3 * sizeof(float));
			ReadFloatArray(output.Vertices, vertexPointsByteBuffer, vertexCount*3);

			stream.Read(vertexPointsByteBuffer, 0, vertexCount * 3 * sizeof(float));
			ReadFloatArray(output.Normals, vertexPointsByteBuffer, vertexCount*3);

			stream.Read(vertexPointsByteBuffer, 0, vertexCount * 2 * sizeof(float));
			ReadFloatArray(output.TextCoord, vertexPointsByteBuffer, vertexCount*2);

			byte[] indexByteBuffer = new byte[triangleCount * 3 * sizeof(int)];

			stream.Read(indexByteBuffer, 0, indexByteBuffer.Length);
			ReadIntArray(output.Indexes, indexByteBuffer, triangleCount*3);

			return output;
		}

		private static void ReadFloatArray(float[] array, byte[] buffer, int count)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] = BitConverter.ToSingle(buffer, i * sizeof(float));
			}
		}

		private static void ReadIntArray(int[] array, byte[] buffer, int count)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] = BitConverter.ToInt32(buffer, i * sizeof(float));
			}
		}

		private static int ReadInt32(Stream inputStream)
		{
			byte[] buffer = new byte[sizeof(int)];
			inputStream.Read(buffer, 0, buffer.Length);
			int output = BitConverter.ToInt32(buffer, 0);
			return output;
		}
	}

	public class NodeData
	{
		public HyperPoint<float>[] Vertices { get; set; }
		public HyperPoint<float>[] Normals { get; set; }
		public HyperPoint<float>[] TextCoord { get; set; } 
		public int[] Indexes { get; set; }

		public static NodeData ReadFromStream(Stream stream)
		{
			NodeData output = new NodeData();
			int VertexCount = ReadInt32(stream);
			int TriangleCount = ReadInt32(stream);

			output.Vertices = new HyperPoint<float>[VertexCount];
			output.Normals = new HyperPoint<float>[VertexCount];
			output.TextCoord = new HyperPoint<float>[VertexCount];
			output.Indexes = new int[TriangleCount*3];

			byte[] vertexPointsByteBuffer = new byte[VertexCount * 3 * sizeof(float)];
			stream.Read(vertexPointsByteBuffer, 0, VertexCount * 3 * sizeof(float));
			for (int i = 0; i < VertexCount; i++)
			{
				output.Vertices[i] = new HyperPoint<float>(
					BitConverter.ToSingle(vertexPointsByteBuffer, (i * 3 + 0) * sizeof(float)),
					BitConverter.ToSingle(vertexPointsByteBuffer, (i * 3 + 1) * sizeof(float)),
					BitConverter.ToSingle(vertexPointsByteBuffer, (i * 3 + 2) * sizeof(float)),
					1);
			}

			stream.Read(vertexPointsByteBuffer, 0, VertexCount * 3 * sizeof(float));
			for (int i = 0; i < VertexCount; i++)
			{
				output.Normals[i] = new HyperPoint<float>(
					BitConverter.ToSingle(vertexPointsByteBuffer, (i*3 + 0)*sizeof (float)),
					BitConverter.ToSingle(vertexPointsByteBuffer, (i*3 + 1)*sizeof (float)),
					BitConverter.ToSingle(vertexPointsByteBuffer, (i*3 + 2)*sizeof (float)),
					1);
			}

			stream.Read(vertexPointsByteBuffer, 0, VertexCount * 2 * sizeof(float));
			for (int i = 0; i < VertexCount; i++)
			{
				output.TextCoord[i] = new HyperPoint<float>(
					BitConverter.ToSingle(vertexPointsByteBuffer, (i * 2 + 0) * sizeof(float)),
					BitConverter.ToSingle(vertexPointsByteBuffer, (i * 2 + 1) * sizeof(float)));
			}

			byte[] indexByteBuffer = new byte[TriangleCount * 3 * sizeof(int)];
			stream.Read(indexByteBuffer, 0, indexByteBuffer.Length);
			for (int i = 0; i < TriangleCount * 3; i++)
			{
				output.Indexes[i] = BitConverter.ToInt32(indexByteBuffer, i * sizeof(int));
			}

			return output;
		}

		public void SaveToStream(Stream stream)
		{
			WriteInt32(Vertices.Length, stream);
			WriteInt32(Indexes.Length / 3, stream);

			for (int i = 0; i < Vertices.Length; i++)
			{
				WriteFloat(Vertices[i].X, stream);
				WriteFloat(Vertices[i].Y, stream);
				WriteFloat(Vertices[i].Z, stream);
			}

			for (int i = 0; i < Vertices.Length; i++)
			{
				WriteFloat(Normals[i].X, stream);
				WriteFloat(Normals[i].Y, stream);
				WriteFloat(Normals[i].Z, stream);
			}

			for (int i = 0; i < Vertices.Length; i++)
			{
				WriteFloat(TextCoord[i].X, stream);
				WriteFloat(TextCoord[i].Y, stream);
			}

			for (int i = 0; i < Indexes.Length; i++)
			{
				WriteInt32(Indexes[i], stream);
			}
		}

		private static int ReadInt32(Stream inputStream)
		{
			byte[] buffer = new byte[sizeof(int)];
			inputStream.Read(buffer, 0, buffer.Length);
			int output = BitConverter.ToInt32(buffer, 0);
			return output;
		}

		private static void WriteInt32(int o, Stream outputStream)
		{
			byte[] buffer = BitConverter.GetBytes(o);
			outputStream.Write(buffer, 0, buffer.Length);
		}

		private static void WriteFloat(float o, Stream outputStream)
		{
			byte[] buffer = BitConverter.GetBytes(o);
			outputStream.Write(buffer, 0, buffer.Length);
		}
	}
}
