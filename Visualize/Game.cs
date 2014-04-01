using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CG_2IV05.Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using micfort.GHL.Math2;
using micfort.GHL.Collections;
using micfort.GHL.Serialization;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace CG_2IV05.Visualize
{
    public class Game
	{
        public HyperPoint<float> currentPosition = new HyperPoint<float>(0,0,0); 
		public Vector3 CameraPos = Vector3.Zero;
		private Vector2 Mouse;
		private Vector2? LastMousePos;
	    private Vector3 viewDirection = Vector3.UnitX;
		private Matrix4 lookAtMatrix;
		private int texture;
		private float speed = 0.5f;
		private List<NodeWithData> vbos;
		private List<NodeWithData> releaseNodes; 
        
        public enum ViewMode{ Roaming, Walking, TopDown };       
        private ViewMode viewMode = ViewMode.Roaming;

		private Tree tree;
		public NodeManager manager;
		private Settings settingsForm;

		private VBO vbo;
	    private SkyBox skybox;
	    private bool mouseControl = false;

        private List<Keys> pressedKeys = new List<Keys>();

	    public Game()
	    {
	    }

		public void game_Unload()
		{
			manager.Stop();
		}

        public void game_Load()
		{
			CameraPos = new Vector3(0, 0, 10f);
			Mouse = new Vector2(0, 0);

			vbos = new List<NodeWithData>();
			releaseNodes = new List<NodeWithData>();

			using (FileStream file = File.OpenRead(VisualizeSettings.TreePath)) 
			{
				this.tree = SerializableType<Tree>.DeserializeFromStream(file, BinarySerializableTypeEngine.BinairSerializer);
				TreeBuildingSettings.DirectoryOutput = Path.GetDirectoryName(VisualizeSettings.TreePath);
			}

			//using (FileStream file = File.OpenRead(@"output\data_13"))
			//{
			//	vbo = OnDemand<VBO>.Create();
			//	vbo.LoadData(NodeDataRaw.ReadFromStream(file));
			//}

			manager = new NodeManager();
			manager.Tree = tree;
			manager.VBOList = vbos;
			manager.ReleaseNodes = releaseNodes;
			manager.Position = CameraPos.ToHyperPoint();
            manager.Start();

            skybox = new SkyBox(this);
            skybox.Load();

			// setup settings, load textures, sounds
			//game.VSync = VSyncMode.On;

			using (FileStream file = File.OpenRead("Texture.fw.png"))
			{
				texture = LoadTexture(file);
			}

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			Matrix4 projMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1920f / 1080f, 0.1f, 10000000f);
			GL.LoadMatrix(ref projMatrix);

			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Back);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Normalize);
			GL.Enable(EnableCap.Texture2D);
			GL.ShadeModel(ShadingModel.Flat);

			GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.Light0);
			GL.Light(LightName.Light0, LightParameter.Ambient, new Color4(1.0f, 1.0f, 1.0f, 1f));
			GL.Light(LightName.Light0, LightParameter.Diffuse, new Color4(1.0f, 1.0f, 1.0f, 1f));
			GL.Light(LightName.Light0, LightParameter.Specular, new Vector4(1.0f, 1.0f, 1.0f, 1.0f));

			GL.Color3(Color.Blue);
		}

        public void game_RenderFrame()
		{
			// render graphics
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref lookAtMatrix);
			GL.Light(LightName.Light0, LightParameter.Position, new Vector4(1, 1, 10, 0));
			
            skybox.Draw();

			GL.BindTexture(TextureTarget.Texture2D, texture);

			if(vbo != null)
			{
				vbo.Draw();
			}
			else
			{
				lock (vbos)
				{
					for (int index = 0; index < vbos.Count; index++)
					{
						NodeWithData node = vbos[index];
						node.vbo.Draw();
					}
				}	
			}

			lock (releaseNodes)
			{
				foreach (NodeWithData nodeWithData in releaseNodes)
				{
					nodeWithData.ReleaseVBO();
				}
				releaseNodes.Clear();
			}
			
		}

	    public void game_Resize(object sender, EventArgs e)
        {
            GLControl control = (GLControl) sender;
            
			GL.Viewport(0, 0, control.Size.Width, control.Size.Height);

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			Matrix4 projMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Convert.ToSingle(control.Size.Width) / control.Size.Height, 0.1f, 10000000f);
			GL.LoadMatrix(ref projMatrix);
		}

	    public static int LoadTexture(Stream file)
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

        public Vector3 GetCurrentDirection()
        {
            return viewDirection;
        }

	    public void OnKeyDown(object sender, KeyEventArgs e)
	    {
            if (!pressedKeys.Contains(e.KeyCode))
                pressedKeys.Add(e.KeyCode);

	        processPressedKeys();
	    }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyCode);
            if(e.KeyCode == Keys.Space || e.KeyCode == Keys.NumPad0)
            {
                SetNextViewState();
            }
            processPressedKeys();
        }

	    public void processPressedKeys()
	    {
	        Vector3 forwardDirection = viewDirection;
            if(viewMode == ViewMode.Walking || viewMode == ViewMode.TopDown){
                    forwardDirection.Z = 0;
                    forwardDirection.Normalize();
            }
            Vector3 SideStep = Vector3.Cross(forwardDirection, Vector3.UnitZ);
            SideStep.Normalize();

            Vector3 moveDirection = Vector3.Zero;
            if (pressedKeys.Contains(Keys.Up) || pressedKeys.Contains(Keys.W))
            {
                moveDirection += forwardDirection;
            }
            if (pressedKeys.Contains(Keys.Down) || pressedKeys.Contains(Keys.S))
            {
                moveDirection -= forwardDirection;
            }
            if (pressedKeys.Contains(Keys.Left) || pressedKeys.Contains(Keys.A))
            {
                moveDirection -= SideStep;
            }
            if (pressedKeys.Contains(Keys.Right) || pressedKeys.Contains(Keys.D))
            {
                moveDirection += SideStep;
            }
            if (pressedKeys.Contains(Keys.E) || pressedKeys.Contains(Keys.PageUp))
            {
                switch (viewMode)
                {
                    case ViewMode.Roaming:
                    case ViewMode.Walking:
                        moveDirection = (forwardDirection + SideStep);
                        moveDirection.Normalize();
                        break;
                    case ViewMode.TopDown:
                        moveDirection += Vector3.UnitZ;
                        break;
                }
            }
            if (pressedKeys.Contains(Keys.Q) || pressedKeys.Contains(Keys.PageDown))
            {
                
                switch (viewMode)
                {
                    case ViewMode.Roaming:
                    case ViewMode.Walking:
                        moveDirection = (forwardDirection - SideStep);
                        moveDirection.Normalize();
                        break;
                    case ViewMode.TopDown:
                        moveDirection -= Vector3.UnitZ;
                        break;
                }
            }
            if (pressedKeys.Contains(Keys.D1))
            {
                speed = 0.1f;
            }
            if (pressedKeys.Contains(Keys.D2))
            {
                speed = 0.2f;
            }
            if (pressedKeys.Contains(Keys.D3))
            {
                speed = 0.5f;
            }
            if (pressedKeys.Contains(Keys.D4))
            {
                speed = 1.0f;
            }
            if (pressedKeys.Contains(Keys.D5))
            {
                speed = 2.0f;
            }
            if (pressedKeys.Contains(Keys.D6))
            {
                speed = 3.0f;
            }
            if (pressedKeys.Contains(Keys.D7))
            {
                speed = 5.0f;
            }
            if (pressedKeys.Contains(Keys.D8))
            {
                speed = 10.0f;
            }
            if (pressedKeys.Contains(Keys.D9))
            {
                speed = 20.0f;
            }
            if (pressedKeys.Contains(Keys.D0))
            {
                speed = 50.0f;
            }
            if (CameraPos.Z < 1.75f)
                CameraPos.Z = 1.75f;

            this.CameraPos = this.CameraPos + moveDirection * speed;
            manager.Position = CameraPos.ToHyperPoint();
            lookAtMatrix = Matrix4.LookAt(this.CameraPos, this.CameraPos + viewDirection, Vector3.UnitZ);
        }

	    public void OnMouseDown(object sender, MouseEventArgs e)
	    {
	       
            if (e.Button == MouseButtons.Right)
            {
                this.mouseControl = true;
            }
            else if (e.Button == MouseButtons.Left)
            {
                this.CameraPos.Z = 1.75f;
                manager.Position = CameraPos.ToHyperPoint();
                lookAtMatrix = Matrix4.LookAt(this.CameraPos, this.CameraPos + viewDirection, Vector3.UnitZ);
            }
	    }
        
        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                this.mouseControl = false;
            }
        }

	    public void OnMouseMove(object sender, MouseEventArgs e)
	    {
            if (LastMousePos == null || !mouseControl)
            {
            	LastMousePos = new Vector2(e.Location.X, e.Location.Y);
            }
            if (mouseControl)
            {
                Vector2 mouseDiff = (new Vector2(e.Location.X, e.Location.Y) - LastMousePos.Value)*(1/50f);
                Mouse = Mouse + mouseDiff;

                Mouse.Y = MathHelper.Clamp(Mouse.Y, -MathHelper.PiOver2 + 0.01f,
                                           MathHelper.PiOver2 - 0.01f);
                LastMousePos = new Vector2(e.Location.X, e.Location.Y);
            }

	        #region viewDirection calculation
            Matrix4 Rotation = Matrix4.CreateRotationY(Mouse.Y) * Matrix4.CreateRotationZ(-Mouse.X);
            viewDirection = Vector3.Transform(Vector3.UnitX, Rotation);
            #endregion
            
            manager.Position = CameraPos.ToHyperPoint();
            lookAtMatrix = Matrix4.LookAt(this.CameraPos, this.CameraPos + viewDirection, Vector3.UnitZ);
	    }

        public HyperPoint<float> getDataCenter()
        {
            if(tree.centerData != null)
                return tree.centerData;

            return new HyperPoint<float>(0,0);
        } 

        public void SetNextViewState()
        {
            switch (viewMode)
            {
                case ViewMode.Roaming:
                    SetViewMode(ViewMode.Walking);
                    break;
                case ViewMode.Walking:
                    SetViewMode(ViewMode.TopDown);
                    break;
                case ViewMode.TopDown:
                    SetViewMode(ViewMode.Roaming);
                    break;
            }
        }

        public void SetViewMode(ViewMode mode)
        {
            switch (mode)
            {
                case ViewMode.Roaming:
                    break;
                case ViewMode.Walking:
                    viewDirection.Z = 0;
                    viewDirection.Normalize();
                    CameraPos.Z = 1.75f;
                    Mouse.Y = 0;
                    break;
                case ViewMode.TopDown:
                    viewDirection = -Vector3.UnitZ;
                    CameraPos.Z = (CameraPos.Z < 500) ? 500 : CameraPos.Z;
                    Mouse.Y = MathHelper.PiOver2 - 0.01f;
                    break;
            }

            viewMode = mode;
            Matrix4 Rotation = Matrix4.CreateRotationY(Mouse.Y) * Matrix4.CreateRotationZ(-Mouse.X);
            viewDirection = Vector3.Transform(Vector3.UnitX, Rotation);
            
            manager.Position = CameraPos.ToHyperPoint();
            lookAtMatrix = Matrix4.LookAt(this.CameraPos, this.CameraPos + viewDirection, Vector3.UnitZ);
        }

        public ViewMode getCurrentViewMode()
        {
            return viewMode;
        }

        public void GoToPoint(HyperPoint<float> goToPoint)
        {
            this.CameraPos = new Vector3(goToPoint.X, goToPoint.Y, 300);
            this.manager.Position = this.CameraPos.ToHyperPoint();
            lookAtMatrix = Matrix4.LookAt(this.CameraPos, this.CameraPos + viewDirection, Vector3.UnitZ);
        }
	}
}
