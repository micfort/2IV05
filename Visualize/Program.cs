using System;
using System.Drawing;
using System.IO;
using CG_2IV05.Common;
using CG_2IV05.Visualize.Interface;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using micfort.GHL.Logging;

namespace CG_2IV05.Visualize
{
	class Program
	{
		static void Main(string[] args)
		{
			micfort.GHL.GHLWindowsInit.Init();
			ErrorReporting.Instance.Engine = new MultipleLoggingEngine(new TextWriterLoggingEngine(Console.Out),
			                                                           new TextWriterLoggingEngine(
				                                                           new StreamWriter(new FileStream("visualize.log",
				                                                                                           FileMode.OpenOrCreate,
				                                                                                           FileAccess.ReadWrite,
				                                                                                           FileShare.Delete, 1024*4,
				                                                                                           FileOptions.WriteThrough))));

			ParseCommandLine(args);

			MainWindow window = new MainWindow();
			window.ShowDialog();
		}

		public static void ParseCommandLine(string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "--tree" || args[i] == "-t")
				{
					i++;
					VisualizeSettings.TreePath = args[i];
					ErrorReporting.Instance.ReportInfoT("Program", string.Format("Using {0} as tree", VisualizeSettings.TreePath));
				}
			}
		}
	}
}
