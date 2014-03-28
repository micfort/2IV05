extern alias osm;
using CG_2IV05.Common.Element;
using osm::OsmSharp.Osm;
using osm::OsmSharp.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;

namespace CG_2IV05.Common.OSM
{
	public interface IOSMWayFactory: IElementFactory
	{
		IOSMWayElement Create(Way way, List<HyperPoint<float>> poly);
		bool CheckKeyAcceptance(osm::OsmSharp.Collections.Tags.TagsCollectionBase Tags);
		bool CheckPolyAcceptance(List<HyperPoint<float>> poly);
	}
}
