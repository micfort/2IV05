using System;
using System.Drawing;
using System.IO;
using CG_2IV05.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace CG_2IV05.Visualize
{
	class Program
	{
		static void Main(string[] args)
		{
			Common.NodeData data;
			using (FileStream file = File.OpenRead(@"D:\S120397\School\2IV05 ACCG\2IV05\TreeBuilding\bin\Debug\output_eindhoven_single_node\data_0"))
			{
				data = NodeData.ReadFromStream(file);	
			}
			float[] Vertices = new float[data.Vertices.Length*8];
			for (int i = 0; i < data.Vertices.Length; i++)
			{
				Vertices[i*4+0] = data.Vertices[i].X;
				Vertices[i*4+1] = data.Vertices[i].Y;
				Vertices[i*4+2] = data.Vertices[i].Z;
				Vertices[i*4+3] = data.Vertices[i].W;

				Vertices[i*4+4] = 0;
				Vertices[i*4+5] = 1;
				Vertices[i*4+6] = 0;
				Vertices[i*4+7] = 0.5f;
			}

			using (var game = new GameWindow())
			{
				game.Load += (sender, e) =>
				{
					// setup settings, load textures, sounds
					game.VSync = VSyncMode.On;

					uint[] VBOid = new uint[2];
					GL.GenBuffers(2, VBOid);

					GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOid[1]);
					GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(data.Indexes.Length * sizeof(int)), data.Indexes, BufferUsageHint.StaticDraw);

					GL.BindBuffer( BufferTarget.ArrayBuffer, VBOid[ 0 ] );
					GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (Vertices.Length*8*sizeof (float)), Vertices, BufferUsageHint.StaticDraw);

					GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

				};

				game.Resize += (sender, e) =>
				{
					GL.Viewport(0, 0, game.Width, game.Height);
				};

				game.UpdateFrame += (sender, e) =>
				{
					// add game logic, input handling
					if (game.Keyboard[Key.Escape])
					{
						game.Exit();
					}
				};

				game.RenderFrame += (sender, e) =>
				{
					// render graphics
					GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

					GL.MatrixMode(MatrixMode.Projection);
					GL.LoadIdentity();
					GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);

					GL.Begin(PrimitiveType.Triangles);

					GL.Color3(Color.MidnightBlue);
					GL.Vertex2(-1.0f, 1.0f);
					GL.Color3(Color.SpringGreen);
					GL.Vertex2(0.0f, -1.0f);
					GL.Color3(Color.Ivory);
					GL.Vertex2(1.0f, 1.0f);

					GL.End();

					game.SwapBuffers();
				};

				// Run the game at 60 updates per second
				game.Run(60.0);
			}
		}
	}
}
