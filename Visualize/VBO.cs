﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG_2IV05.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CG_2IV05.Visualize
{
	public class VBO: IDisposable
	{
		public uint Indexes { get; private set; }
		public uint Vertices { get; private set; }
		public uint TextCoord { get; private set; }
		public uint Normals { get; private set; }
		public int NumElements { get; private set; }
		public bool DataLoaded { get; private set; }

		public VBO()
		{
			DataLoaded = false;

			Indexes = CreateBuffer();
			Vertices = CreateBuffer();
			Normals = CreateBuffer();
			TextCoord = CreateBuffer();
		}

		public void LoadData(NodeDataRaw data)
		{
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, Indexes);
			GLError();
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(data.Indexes.Length * sizeof(int)), data.Indexes, BufferUsageHint.DynamicDraw);
			GLError();
			
			GL.BindBuffer(BufferTarget.ArrayBuffer, Vertices);
			GLError();
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Vertices.Length * sizeof(float)), data.Vertices, BufferUsageHint.DynamicDraw);
			GLError();

			GL.BindBuffer(BufferTarget.ArrayBuffer, TextCoord);
			GLError();
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.TextCoord.Length * sizeof(float)), data.TextCoord, BufferUsageHint.DynamicDraw);
			GLError();

			GL.BindBuffer(BufferTarget.ArrayBuffer, Normals);
			GLError();
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Normals.Length * sizeof(float)), data.Normals, BufferUsageHint.DynamicDraw);
			GLError();

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			NumElements = data.Indexes.Length;

			DataLoaded = true;
		}

		private void GLError()
		{
			var error = GL.GetError();
			if(error != ErrorCode.NoError)
			{
				micfort.GHL.Logging.ErrorReporting.Instance.ReportDebugT(this, string.Format("OpenGL error {0}", error));
			}
		}

		public void Draw()
		{
			if (!DataLoaded)
				throw new ArgumentException("There should be data loaded.");

			GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
			GLError();
			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, Normals);
				GLError();
				// Set the Pointer to the current bound array describing how the data ia stored
				GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
				GLError();
				// Enable the client state so it will use this array buffer pointer
				GL.EnableClientState(EnableCap.NormalArray);
				GLError();
			}


			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, Vertices);
				GLError();
				// Set the Pointer to the current bound array describing how the data ia stored
				GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
				GLError();
				// Enable the client state so it will use this array buffer pointer
				GL.EnableClientState(EnableCap.VertexArray);
				GLError();
			}

			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, TextCoord);
				GLError();
				// Set the Pointer to the current bound array describing how the data ia stored
				GL.TexCoordPointer(2, TexCoordPointerType.Float, Vector2.SizeInBytes, IntPtr.Zero);
				GLError();
				// Enable the client state so it will use this array buffer pointer
				GL.EnableClientState(EnableCap.TextureCoordArray);
				GLError();
			}

			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, Indexes);
				GLError();
				// Draw the elements in the element array buffer
				// Draws up items in the Color, Vertex, TexCoordinate, and Normal Buffers using indices in the ElementArrayBuffer
				GL.DrawElements(BeginMode.Triangles, NumElements, DrawElementsType.UnsignedInt, IntPtr.Zero);
				GLError();
				// Could also call GL.DrawArrays which would ignore the ElementArrayBuffer and just use primitives
				// Of course we would have to reorder our data to be in the correct primitive order
			}

			GL.PopClientAttrib();
			GLError();
		}

		private uint CreateBuffer()
		{
			uint i;
			GL.GenBuffers(1, out i);
			return i;
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			GL.DeleteBuffer(Indexes);
			GL.DeleteBuffer(Vertices);
			GL.DeleteBuffer(Normals);
			GL.DeleteBuffer(TextCoord);
		}

		#endregion
	}
}
