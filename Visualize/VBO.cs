using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG_2IV05.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CG_2IV05.Visualize
{
	class VBO
	{
		public uint Indexes { get; private set; }
		public uint Vertices { get; private set; }
		public uint Normals { get; private set; }
		public int NumElements { get; private set; }

		public VBO(NodeDataRaw data)
		{
			Indexes = CreateBuffer();
			Vertices = CreateBuffer();
			Normals = CreateBuffer();

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, Indexes);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(data.Indexes.Length * sizeof(int)), data.Indexes, BufferUsageHint.DynamicDraw);

			GL.BindBuffer(BufferTarget.ArrayBuffer, Vertices);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Vertices.Length * Vector3.SizeInBytes), data.Vertices, BufferUsageHint.DynamicDraw);

			GL.BindBuffer(BufferTarget.ArrayBuffer, Normals);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Normals.Length * Vector3.SizeInBytes), data.Normals, BufferUsageHint.DynamicDraw);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			NumElements = data.Indexes.Length;
		}

		public void Draw()
		{
			GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, Normals);

				// Set the Pointer to the current bound array describing how the data ia stored
				GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);

				// Enable the client state so it will use this array buffer pointer
				GL.EnableClientState(EnableCap.NormalArray);
			}


			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, Vertices);

				// Set the Pointer to the current bound array describing how the data ia stored
				GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);

				// Enable the client state so it will use this array buffer pointer
				GL.EnableClientState(EnableCap.VertexArray);
			}

			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, Indexes);

				// Draw the elements in the element array buffer
				// Draws up items in the Color, Vertex, TexCoordinate, and Normal Buffers using indices in the ElementArrayBuffer
				GL.DrawElements(BeginMode.Triangles, NumElements, DrawElementsType.UnsignedInt, IntPtr.Zero);

				// Could also call GL.DrawArrays which would ignore the ElementArrayBuffer and just use primitives
				// Of course we would have to reorder our data to be in the correct primitive order
			}

			GL.PopClientAttrib();
		}

		private uint CreateBuffer()
		{
			uint i;
			GL.GenBuffers(1, out i);
			return i;
		}
	}
}
