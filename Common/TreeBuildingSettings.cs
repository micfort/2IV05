using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;

namespace CG_2IV05.Common
{
	public class TreeBuildingSettings
	{
		public static int MaxTriangleCount = 600000;
		public static int FileCount = 0;
		public static string NodeFilenameFormat = "{0}\\data_{1}";
		public static string TreeOutputFileFormat = "{0}\\tree";
		public static string DirectoryOutput = "output";
		public static string InputFilename = "buildings.xml";
		public static string InputOSMData = "../../../";
		public static bool Generate = false;
		public static int generateSizeX = 100;
		public static int generateSizeY = 100;
		public static HyperPoint<float> CenterDataSet;
		public static bool FindCenterDataSet = true;
	}
}
