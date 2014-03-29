﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TreeViewer
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			micfort.GHL.GHLWindowsInit.Init();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
