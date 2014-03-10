﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
		private Vector2? LastMousePos;
		private Matrix4 lookAtMatrix;
		private int texture;
		
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
			CameraPos = new Vector3(0, 0, 10f);
			Mouse = new Vector2(0, 0);
			
			// setup settings, load textures, sounds
			game.VSync = VSyncMode.On;

			using (FileStream file = File.OpenRead(@"D:\S120397\School\2IV05 ACCG\2IV05\TreeBuilding\bin\Debug\output\data_0"))
			{
				NodeDataRaw data = NodeDataRaw.ReadFromStream(file);
				vbo = new VBO();
				vbo.LoadData(data);
			}

			using (FileStream file = File.OpenRead("Texture.fw.png"))
			{
				texture = LoadTexture(file);
			}

			//GL.Enable(EnableCap.ColorMaterial);
			GL.Disable(EnableCap.CullFace);
			GL.Enable(EnableCap.DepthTest);
			GL.CullFace(CullFaceMode.Front);
			GL.Enable(EnableCap.Normalize);
			GL.Enable(EnableCap.Texture2D);

			GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.Light0);
			GL.Light(LightName.Light0, LightParameter.Ambient, new Color4(0.2f, 0.2f, 0.2f, 1f));
			GL.Light(LightName.Light0, LightParameter.Diffuse, new Color4(0.8f, 0.8f, 0.8f, 1f));
			GL.Light(LightName.Light0, LightParameter.Position, new Vector4(1, 1, 10, 0));
			GL.Light(LightName.Light0, LightParameter.Specular, new Vector4(0, 0, 0, 0));

			System.Windows.Forms.Cursor.Position = game.PointToScreen(new Point(game.Width / 2, game.Height / 2));

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			Matrix4 projMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1920f / 1080f, 0.1f, 10000000f);
			GL.LoadMatrix(ref projMatrix);
		}

		void game_RenderFrame(object sender, FrameEventArgs e)
		{
			// render graphics
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref lookAtMatrix);
			
			GL.BindTexture(TextureTarget.Texture2D, texture);
			vbo.Draw();

			game.SwapBuffers();
		}

		void game_UpdateFrame(object sender, FrameEventArgs e)
		{
			#region mouse
			if (LastMousePos == null || !game.Mouse[MouseButton.Left])
			{
				LastMousePos = new Vector2(game.Mouse.X, game.Mouse.Y);
			}

			if (game.Mouse[MouseButton.Left])
			{
				Vector2 mouseDiff = (new Vector2(game.Mouse.X, game.Mouse.Y) - LastMousePos.Value) * (1 / 50f);
				Mouse = Mouse + mouseDiff;
				LastMousePos = new Vector2(game.Mouse.X, game.Mouse.Y);
			}
			#endregion
			
			#region Direction calculation
			Matrix4 Rotation = Matrix4.CreateRotationY(Mouse.Y) * Matrix4.CreateRotationZ(-Mouse.X);
			Vector3 Direction = Vector3.Transform(Vector3.UnitX, Rotation);
			#endregion

			#region keyboard
			Vector3 SideStep = Vector3.Cross(Direction, Vector3.UnitZ);
			float speed = 10f;

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
			#endregion

			lookAtMatrix = Matrix4.LookAt(this.CameraPos, this.CameraPos + Direction, Vector3.UnitZ);
		}

		void game_Resize(object sender, EventArgs e)
		{
			GL.Viewport(0, 0, game.Width, game.Height);

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			Matrix4 projMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Convert.ToSingle(game.Width) / game.Height, 0.1f, 10000000f);
			GL.LoadMatrix(ref projMatrix);
		}

		static int LoadTexture(Stream file)
		{
			if (file == null)
				throw new ArgumentNullException("file");

			int id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);

			Bitmap bmp = new Bitmap(file);
			BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

			bmp.UnlockBits(bmp_data);

			// We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
			// On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
			// mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			return id;
		}
	}
}
