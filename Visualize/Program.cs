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
			Game g = new Game();
			g.Run();	
		}
	}
}
