using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG_2IV05.Common;
using CG_2IV05.Common.BAG;
using CG_2IV05.Common.Element;
using micfort.GHL.Math2;

namespace CG_2IV05.PreProcess2
{
	class Generation
	{
		public static void CreateData(Action<Building> handler)
		{
			int stepsize = 10;
			for (int i = 0; i < TreeBuildingSettings.generateSizeX; i++)
			{
				for (int j = 0; j < TreeBuildingSettings.generateSizeY; j++)
				{
					
					int x = i * stepsize;
					int y = j * stepsize;
					List<HyperPoint<float>> polygon = new List<HyperPoint<float>>
						                                  {
							                                  new HyperPoint<float>(x, y, 0),
							                                  new HyperPoint<float>(x, y + 1, 0),
							                                  new HyperPoint<float>(x + 1, y + 1, 0),
							                                  new HyperPoint<float>(x + 1, y, 0)
						                                  };
					float height = 1;
					Building b = new Building(polygon, height);
					handler(b);
				}
			}
		}
	}
}
