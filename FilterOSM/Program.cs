using System.Collections.Generic;
using System.IO;
using CG_2IV05.Common.OSM;

namespace CG_2IV05.FilterOSM
{
	class Program
	{
		static void Main(string[] args)
		{
			micfort.GHL.GHLWindowsInit.Init();
			using (FileStream input = File.OpenRead(@"D:\S120397\School\2IV05 ACCG\netherlands-latest.osm.pbf"))
			{
				using (FileStream output = File.OpenWrite("osm_data_Eindhoven"))
				{
					OSM.Filter(input, output);
				}
			}
		}
	}
}
