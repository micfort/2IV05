using System;
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
		private uint _indexes = 0;
		private uint _vertices = 0;
		private uint _textCoord = 0;
		private uint _normals = 0;
		private int _numElements;
		private NodeDataRaw raw = null;
		
		public bool DataLoaded { get; private set; }

		public VBO()
		{
			DataLoaded = false;
		}

		public void LoadData(NodeDataRaw data)
		{
			this.raw = data;

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

			if(raw != null)
				CopyToGPU();

			GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
			GLError();
			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, _normals);
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
				GL.BindBuffer(BufferTarget.ArrayBuffer, _vertices);
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
				GL.BindBuffer(BufferTarget.ArrayBuffer, _textCoord);
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
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexes);
				GLError();
				// Draw the elements in the element array buffer
				// Draws up items in the Color, Vertex, TexCoordinate, and Normal Buffers using indices in the ElementArrayBuffer
				GL.DrawElements(BeginMode.Triangles, _numElements, DrawElementsType.UnsignedInt, IntPtr.Zero);
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

		private void CopyToGPU()
		{
			_indexes = CreateBuffer();
			_vertices = CreateBuffer();
			_normals = CreateBuffer();
			_textCoord = CreateBuffer();

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexes);
			GLError();
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(raw.Indexes.Length * sizeof(int)), raw.Indexes, BufferUsageHint.DynamicDraw);
			GLError();

			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertices);
			GLError();
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(raw.Vertices.Length * sizeof(float)), raw.Vertices, BufferUsageHint.DynamicDraw);
			GLError();

			GL.BindBuffer(BufferTarget.ArrayBuffer, _textCoord);
			GLError();
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(raw.TextCoord.Length * sizeof(float)), raw.TextCoord, BufferUsageHint.DynamicDraw);
			GLError();

			GL.BindBuffer(BufferTarget.ArrayBuffer, _normals);
			GLError();
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(raw.Normals.Length * sizeof(float)), raw.Normals, BufferUsageHint.DynamicDraw);
			GLError();

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			_numElements = raw.Indexes.Length;

			raw = null;
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			if(_indexes != 0) GL.DeleteBuffer(_indexes);
			if (_indexes != 0) GL.DeleteBuffer(_vertices);
			if (_indexes != 0) GL.DeleteBuffer(_normals);
			if (_indexes != 0) GL.DeleteBuffer(_textCoord);
		}

		#endregion
	}
}
