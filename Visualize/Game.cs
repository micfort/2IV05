using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using CG_2IV05.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace CG_2IV05.Visualize
{
	

	class Game
	{
		private GameWindow game;
		private VBO vbo;
		private Vector3 CameraPos;
		private Vector2 Mouse;
		private Matrix4 viewMatrix;
		

		public Game()
		{
			
		}

		public void Run()
		{
			using(game = new GameWindow())
			{
				game.Load += game_Load;
				game.Resize += game_Resize;
				game.UpdateFrame += game_UpdateFrame;
				game.RenderFrame += game_RenderFrame;

				game.Run();
			}
		}

		void game_Load(object sender, EventArgs e)
		{
			CameraPos = new Vector3(-10, 0, 0);
			Mouse = new Vector2(0, 0);
			
			// setup settings, load textures, sounds
			game.VSync = VSyncMode.On;

			using (FileStream file = File.OpenRead(@"D:\S120397\School\2IV05 ACCG\2IV05\TreeBuilding\bin\Debug\output\data_0"))
			{
				NodeDataRaw data = NodeDataRaw.ReadFromStream(file);
				vbo = new VBO(data);
			}

			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.DepthTest);
			GL.CullFace(CullFaceMode.Front);

			GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.Light0);
			GL.Light(LightName.Light0, LightParameter.Ambient, new Color4(0.5f, 0.5f, 0.5f, 1f));
			GL.Light(LightName.Light0, LightParameter.Diffuse, new Color4(0.8f, 0.8f, 0.8f, 1f));
			GL.Light(LightName.Light0, LightParameter.Position, new Vector4(1, 1, 10, 0));

			System.Windows.Forms.Cursor.Position = game.PointToScreen(new Point(game.Width / 2, game.Height / 2));
		}

		void game_RenderFrame(object sender, FrameEventArgs e)
		{
			// render graphics
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			Matrix4 projMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1920f / 1080f, 0.1f, 100f);
			GL.LoadMatrix(ref projMatrix);
			
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			GL.LoadMatrix(ref viewMatrix);

			GL.Color3(Color.SpringGreen);
			vbo.Draw();

			game.SwapBuffers();
		}

		void game_UpdateFrame(object sender, FrameEventArgs e)
		{
			Vector2 MouseDiff = new Vector2(game.Mouse.X - game.Width / 2f, game.Mouse.Y - game.Height / 2f) * (1 / 50f);
			System.Windows.Forms.Cursor.Position = game.PointToScreen(new Point(game.Width / 2, game.Height / 2));

			Mouse = Mouse + MouseDiff;
			Matrix4 Rotation = Matrix4.CreateRotationY(Mouse.Y) * Matrix4.CreateRotationZ(-Mouse.X);
			Vector3 Direction = Vector3.Transform(Vector3.UnitX, Rotation);
			Vector3 SideStep = Vector3.Cross(Direction, Vector3.UnitZ);
			float speed = 1/10f;

			// add game logic, input handling
			if (game.Keyboard[Key.Escape])
			{
				game.Exit();
			}
			if (game.Keyboard[Key.Up] || game.Keyboard[Key.W])
			{
				this.CameraPos = this.CameraPos + Direction*speed;
			}
			if (game.Keyboard[Key.Down] || game.Keyboard[Key.S])
			{
				this.CameraPos = this.CameraPos - Direction * speed;
			}
			if (game.Keyboard[Key.Left] || game.Keyboard[Key.A])
			{
				this.CameraPos = this.CameraPos - SideStep * speed;
			}
			if (game.Keyboard[Key.Right] || game.Keyboard[Key.D])
			{
				this.CameraPos = this.CameraPos + SideStep * speed;
			}

			viewMatrix = Matrix4.LookAt(this.CameraPos, this.CameraPos + Direction, Vector3.UnitZ);
		}

		void game_Resize(object sender, EventArgs e)
		{
			GL.Viewport(0, 0, game.Width, game.Height);
		}


	}
}
