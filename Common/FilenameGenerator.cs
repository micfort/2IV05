using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CG_2IV05.Common
{
	class FilenameGenerator
	{
		private static object _lockObject = new object();
		public static string CreateFilename()
		{
			lock (_lockObject)
			{
				return string.Format(TreeBuildingSettings.NodeFilenameFormat, TreeBuildingSettings.FileCount++);	
			}
		}

		public static string GetOutputPathToFile(string filename)
		{
			return TreeBuildingSettings.DirectoryOutput + Path.DirectorySeparatorChar + filename;
		}

		public static string CreateTempFilename()
		{
			lock (_lockObject)
			{
				return string.Format(TreeBuildingSettings.TmpFilenameFormat, TreeBuildingSettings.TmpDirectory, TreeBuildingSettings.TmpFileCount++);
			}
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

		public static string GetWorkingFilename(string file)
		{
			return string.Format("{0}//{1}", TreeBuildingSettings.DirectoryWorking, file);
		}
	}
}
