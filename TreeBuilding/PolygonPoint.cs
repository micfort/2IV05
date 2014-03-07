using micfort.GHL.Math2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CG_2IV05.TreeBuilding
{
    class PolygonPoint
    {
        public int id;
        public bool isReflex = false;
        public bool isEar = false;
        public HyperPoint<float> point;

        public PolygonPoint(int id, float x, float y)
        {
            this.id = id;
            point = new HyperPoint<float>(x, y);
        }
    }

    
}
