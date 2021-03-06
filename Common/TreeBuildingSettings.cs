﻿using System;
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
		public static string DirectoryWorking = "working";
		public static string InputFilename = @"D:\S120397\School\2IV05 ACCG\datasets\Test\squares.elements";
		//public static string InputFilename = @"D:\S120397\School\2IV05 ACCG\datasets\Eindhoven\NL_ehv_ALL.elements";
		public static bool Generate = false;
		public static int generateSizeX = 50;
		public static int generateSizeY = 50;
		public static HyperPoint<float> CenterDataSet;
		public static bool FindCenterDataSet = true;
		public static int MinCurrentDepthForData = 0;
		public static int CreateThreadDepth = 2;
		public static bool SimplifySingleElements = true;

		public static string TmpDirectory = "tmp";
		public static int TmpFileCount = 0;
		public static string TmpFilenameFormat = "{0}\\{1}";
	}
}
