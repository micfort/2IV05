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
		public static int MaxElementCount = 50000;
		public static int FileCount = 0;
		public static string NodeFilenameFormat = "data_{0}";
		public static string TreeOutputFileFormat = "{0}\\tree";
		public static string DirectoryOutput = "output";
		public static string InputFilename = @"D:\S120397\School\2IV05 ACCG\datasets\NL\NL_ALL.elements";
		public static bool Generate = false;
		public static int generateSizeX = 100;
		public static int generateSizeY = 100;
		public static HyperPoint<float> CenterDataSet;
		public static bool FindCenterDataSet = true;
		public static int MinCurrentDepthForData = 4;
		public static int CreateThreadDepth = 1;

		public static string TmpDirectory = "tmp";
		public static int TmpFileCount = 0;
		public static string TmpFilenameFormat = "{0}\\{1}";
	}
}
