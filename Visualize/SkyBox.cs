using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace CG_2IV05.Visualize
{
    class SkyBox
    {
        private int[] textures = new int[6];
        private string[] textureNames = { "sk_front", "sk_left", "sk_back", "sk_right", "sk_top"};
        private Game game;

        public SkyBox(Game game)
        {
            this.game = game;
        }

        public void Load()
        {

            for (int i = 0; i < textureNames.Length; i++)
            {
                textures[i] = LoadTexture(".\\skybox\\"+textureNames[i]+".png");
            }

        }

        private int LoadTexture(string file)
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
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);

            return id;
        }

        public void Draw()
        {
            // Just in case we set all vertices to white.
            GL.Color4(0, 1, 1, 1);

            Vector3 currentDirection = game.GetCurrentDirection();
            float width = 1;
            float height = 0.5f;

            // Center the skybox
            float x = -width / 2;
            float y = -width / 2;
            float z = 0;
            
            GL.PushMatrix();
            GL.LoadIdentity();
            Matrix4 lookAt = Matrix4.LookAt(Vector3.Zero, currentDirection, Vector3.UnitZ);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookAt);

            GL.PushAttrib(AttribMask.EnableBit);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
            //GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.Texture2D);

            // TOP
            GL.BindTexture(TextureTarget.Texture2D, textures[4]);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(1, 0); GL.Vertex3(x, y, z + height);
            GL.TexCoord2(1, 1); GL.Vertex3(x, y + width, z + height);
            GL.TexCoord2(0, 1); GL.Vertex3(x + width, y + width, z + height);
            GL.TexCoord2(0, 0); GL.Vertex3(x + width, y, z + height);
            GL.End();

            // back
            GL.BindTexture(TextureTarget.Texture2D, textures[2]);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 1); GL.Vertex3(x, y, z);
            GL.TexCoord2(0, 0); GL.Vertex3(x, y, z + height);
            GL.TexCoord2(1, 0); GL.Vertex3(x + width, y, z + height);
            GL.TexCoord2(1, 1); GL.Vertex3(x + width, y, z);
            GL.End();

            // Front
            GL.BindTexture(TextureTarget.Texture2D, textures[0]);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 1); GL.Vertex3(x + width, y + width, z);
            GL.TexCoord2(0, 0); GL.Vertex3(x + width, y + width, z + height);
            GL.TexCoord2(1, 0); GL.Vertex3(x, y + width, z + height);
            GL.TexCoord2(1, 1); GL.Vertex3(x, y + width, z);
            GL.End();

            // LEFT
            GL.BindTexture(TextureTarget.Texture2D, textures[1]);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 1); GL.Vertex3(x, y + width, z);
            GL.TexCoord2(0, 0); GL.Vertex3(x, y + width, z + height);
            GL.TexCoord2(1, 0); GL.Vertex3(x, y, z + height);
            GL.TexCoord2(1, 1); GL.Vertex3(x, y, z);
            GL.End();

            // RIGHT
            GL.BindTexture(TextureTarget.Texture2D, textures[3]);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 1); GL.Vertex3(x + width, y, z);
            GL.TexCoord2(0, 0); GL.Vertex3(x + width, y, z + height);
            GL.TexCoord2(1, 0); GL.Vertex3(x + width, y + width, z + height);
            GL.TexCoord2(1, 1); GL.Vertex3(x + width, y + width, z);
            GL.End();

            GL.PopAttrib();
            GL.PopMatrix();
        }

    }
}
