using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;

namespace CG_2IV05.Common
{
	public class PolygonHelper
	{
		public static List<HyperPoint<float>> RemoveRepeatition(List<HyperPoint<float>> polygon)
		{
			for (int i = polygon.Count - 1; i >= 1; i--)
			{
				if (polygon[i] == polygon[i - 1])
				{
					polygon.RemoveAt(i);
				}
			}
			if (polygon.Count == 1)
			{
				return polygon;
			}
			if (polygon[0] == polygon[polygon.Count - 1])
			{
				polygon.RemoveAt(polygon.Count - 1);
			}
			return polygon;
		}
	}
}
