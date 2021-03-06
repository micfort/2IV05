﻿using System;
using System.Linq;
using System.Collections.Generic;
using CG_2IV05.Common.BAG;
using CG_2IV05.Common.OSM;
using CG_2IV05.Common.TestElement;

namespace CG_2IV05.Common.Element
{
	public class FactoryIDs
	{
		//Local IDs
		public const int ElementListID = 001;
		public const int TestID = 002;
		public const int TestViewID = 003;
		//BAG IDs
		public const int BuildingID = 101;
		//OSM IDs
		public const int RoadID = 201;
		public const int LandUseID = 202;
		public const int Road2ID = 203;


		private static Dictionary<int, IElementFactory> factories = new Dictionary<int, IElementFactory>()
			                                                            {
																			//local IDs
																			{ElementListID, null},
																			{TestViewID, new TestElementFactory()},
																			//BAG IDs 
				                                                            {BuildingID, new BuildingFactory()},
																			//OSM IDs
																			{RoadID, new RoadFactory()},
																			{LandUseID, new LandUseFactory()},
																			{Road2ID, new Road2Factory()}
			                                                            }; 

		public static IElementFactory GetFactory(int ID)
		{
			return factories[ID];
		}

		public static List<int> GetFactoryIDs()
		{
			return factories.Keys.ToList();
		}

		public static void AddElementFactory(IElementFactory factory)
		{
			if(!factories.ContainsKey(factory.FactoryID))
			{
				factories[factory.FactoryID] = factory;
			}
		}
	}
}
