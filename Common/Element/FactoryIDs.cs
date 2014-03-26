using System;
using System.Linq;
using System.Collections.Generic;
using CG_2IV05.Common.BAG;
using CG_2IV05.Common.OSM;

namespace CG_2IV05.Common.Element
{
	public class FactoryIDs
	{
		//Local IDs
		public const int ElementListID = 001;
		//BAG IDs
		public const int BuildingID = 101;
		//OSM IDs
		public const int RoadID = 201;
		public const int LandUseID = 202;

		private static readonly Dictionary<int, IElementFactory> factories = new Dictionary<int, IElementFactory>()
			                                                            {
																			//local IDs
																			{ElementListID, null},
																			//BAG IDs 
				                                                            {BuildingID, new BuildingFactory()},
																			//OSM IDs
																			{RoadID, new RoadFactory()},
																			{LandUseID, new LandUseFactory()},
			                                                            }; 

		public static IElementFactory GetFactory(int ID)
		{
			return factories[ID];
		}

		public static List<int> GetFactoryIDs()
		{
			return factories.Keys.ToList();
		}
	}
}
