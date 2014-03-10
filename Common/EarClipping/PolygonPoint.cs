using micfort.GHL.Math2;

namespace CG_2IV05.Common.EarClipping
{
    public class PolygonPoint
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
