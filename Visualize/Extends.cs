using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace CG_2IV05.Visualize
{
	static class Extends
	{
		public static OpenTK.Vector3 ToVector3(this micfort.GHL.Math2.HyperPoint<float> point)
		{
			if(point.Dim != 3)
				throw new ArgumentException("point has to have dimension 3");
			return new Vector3(point.X, point.Y, point.Z);
		}
	}
}
