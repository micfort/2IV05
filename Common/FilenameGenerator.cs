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
	}
}
