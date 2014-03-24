using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CG_2IV05.Common
{
	class FilenameGenerator
	{
		public static string CreateFilename()
		{
			return string.Format(TreeBuildingSettings.NodeFilenameFormat, TreeBuildingSettings.DirectoryOutput, TreeBuildingSettings.FileCount++);
		}

		public static string CreateTempFilename()
		{
			return string.Format(TreeBuildingSettings.TmpFilenameFormat, TreeBuildingSettings.TmpDirectory, TreeBuildingSettings.TmpFileCount++);
		}

		public static string[] CreateTempFilenames(int count)
		{
			string[] output = new string[count];
			for (int i = 0; i < count; i++)
			{
				output[i] = CreateTempFilename();
			}
			return output;
		}
	}
}
