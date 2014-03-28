using System;
using System.Drawing;
using System.IO;
using CG_2IV05.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using micfort.GHL.Logging;

namespace CG_2IV05.Visualize
{
	class Program
	{
		static void Main2(string[] args)
		{
			micfort.GHL.GHLWindowsInit.Init();
			micfort.GHL.Logging.ErrorReporting.Instance.Engine = new TextWriterLoggingEngine(Console.Out);
			Game g = new Game();
		//	g.Run();	
		}
	}
}
