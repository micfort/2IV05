using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm;
using OsmSharp.Collections.Tags;
using micfort.GHL.Math2;

namespace CG_2IV05.Common.OSM
{
	public interface IOSMWayFactory
	{
		IOSMWayElement Create(Way way, List<HyperPoint<float>> poly);
		bool CheckKeyAcceptance(TagsCollectionBase Tags);
		bool CheckPolyAcceptance(List<HyperPoint<float>> poly);
	}
}
