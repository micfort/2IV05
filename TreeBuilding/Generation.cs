using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG_2IV05.Common;
using CG_2IV05.Common.Element;
using micfort.GHL.Math2;

namespace CG_2IV05.TreeBuilding
{
	class Generation
	{
		public static List<Building> CreateData()
		{
			int stepsize = 10;
			List<Building> output = new List<Building>();
			for (int i = 0; i < TreeBuildingSettings.generateSizeX; i++)
			{
				for (int j = 0; j < TreeBuildingSettings.generateSizeY; j++)
				{
					Building b = new Building();
					int x = i * stepsize;
					int y = j * stepsize;
					b.Polygon = new List<HyperPoint<float>>();
					b.Polygon.Add(new HyperPoint<float>(x, y, 0));
					b.Polygon.Add(new HyperPoint<float>(x, y + 1, 0));
					b.Polygon.Add(new HyperPoint<float>(x + 1, y + 1, 0));
					b.Polygon.Add(new HyperPoint<float>(x + 1, y, 0));
					b.Height = 1;
					output.Add(b);
				}
			}
			return output;
		}
	}
}
