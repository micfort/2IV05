using micfort.GHL.Math2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CG_2IV05.TreeBuilding
{
    static class EarClippingTriangulator
    {
        /// <summary>
        /// Triangulate a polygon by using ear-clipping/ear-slicing
        /// </summary>
        /// <param name="polyogon"></param>
        /// <param name="offset">ofset for index pointers to inedeces</param>
        /// <returns></returns>
        public static int[] triangulatePolygon(HyperPoint<float>[] polygon, int offset)
        {
            List<PolygonPoint> polygonList = initPolygon(polygon);
            return triangulate(polygonList, offset).ToArray();
        }

        /// <summary>
        /// Transform array to List and initialise all reflex/concave vertices and ear vertices
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private static List<PolygonPoint> initPolygon(HyperPoint<float>[] polygon)
        {
            int amount = polygon.Count();
            List<PolygonPoint> polygonList = new List<PolygonPoint>();
            for (int i = 0; i < amount; i++)
            {
                PolygonPoint point = new PolygonPoint(i, polygon[i].X, polygon[i].Y);
                polygonList.Add(point);
            }

            // Algorithm designed for counter clockwise ordering
            if(!isCounterClockwise(polygon))
                polygonList.Reverse();

            findReflexVertices(polygonList);
            findEarVertices(polygonList);

            return polygonList;
        }

        /// <summary>
        /// Initialise reflex/concave property of vertices
        /// </summary>
        /// <param name="polygonList"></param>
        private static void findReflexVertices(List<PolygonPoint> polygonList)
        {
            for (int i = 0; i < polygonList.Count(); i++)
            {
                isReflex(polygonList, i);
            }
        }

        /// <summary>
        /// Check if vertex is reflex. Vertex is reflex if the angle between the edges
        /// prev-cur and cur-next is larger than 180 degrees.
        /// Calculated with cross product. Positive magnitude means vertex is reflex.
        /// </summary>
        /// <param name="polygonList"></param>
        /// <param name="i"></param>
        private static void isReflex(List<PolygonPoint> polygonList, int i)
        {
            int count = polygonList.Count();

            PolygonPoint vertex = polygonList[i];
            PolygonPoint next = polygonList[(i + 1) % count];
            PolygonPoint prev = (i == 0) ? polygonList[count - 1] : polygonList[i - 1];

            HyperPoint<float> p0 = vertex.point;
            HyperPoint<float> p1 = p0 - prev.point;
            HyperPoint<float> p2 = p0 - next.point;

            float cross = p1.Cross2D(p2);
            if (cross > 0)
                vertex.isReflex = true;
            else
                vertex.isReflex = false;
        }
        
        /// <summary>
        /// Find all ears in polygon.
        /// </summary>
        /// <param name="polygonList"></param>
        private static void findEarVertices(List<PolygonPoint> polygonList)
        {
            for (int i = 0; i < polygonList.Count(); i++)
            {
                isEar(polygonList, i);
            }
        }

        /// <summary>
        /// Test whether an vertex is the top of an ear or not.
        /// Vertex is an ear top if it is not reflex and if the triangle prev,ear,next does not
        /// contain any other (reflex) points.
        /// Vertex cannot be an ear when its reflex.
        /// </summary>
        /// <param name="polygonList"></param>
        /// <param name="index"></param>
        private static void isEar(List<PolygonPoint> polygonList, int index)
        {
            int count = polygonList.Count();
            bool isEar = true;

            PolygonPoint vertex = polygonList[index];
            if (vertex.isReflex)
            {
                vertex.isEar = false;
                return;
            }

            PolygonPoint next = polygonList[(index + 1) % count];
            PolygonPoint prev = (index == 0) ? polygonList[count - 1] : polygonList[index - 1];

            // Test containment of all other points
            for (int j = 2; j < count-1; j++)
            {
                // Only test reflex points
                PolygonPoint p = polygonList[(index + j) % count];
                if (p.isReflex)
                {
                    if (triangleContainsPoint(prev.point, vertex.point, next.point, p.point))
                    {
                        isEar = false;
                        break;
                    }
                }
            }
            vertex.isEar = isEar;
        }

        /// <summary>
        /// Calculate whether point p lays within a triangle formed by p1,p2 and p3 using Barycentric coordinates
        /// http://stackoverflow.com/questions/13300904/determine-whether-point-lies-inside-triangle
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p"></param>
        private static bool triangleContainsPoint(HyperPoint<float> p1, HyperPoint<float> p2, HyperPoint<float> p3, HyperPoint<float> p)
        {
            float alpha = ((p2.Y - p3.Y) * (p.X - p3.X) + (p3.X - p2.X) * (p.Y - p3.Y)) /
                    ((p2.Y - p3.Y) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Y - p3.Y));
            float beta = ((p3.Y - p1.Y) * (p.X - p3.X) + (p1.X - p3.X) * (p.Y - p3.Y)) /
                   ((p2.Y - p3.Y) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Y - p3.Y));
            float gamma = 1.0f - alpha - beta;

            return alpha > 0 && beta > 0 && gamma > 0;
        }

        /// <summary>
        /// Perform triangulation on polygon by using ear slicing.
        /// Each time an ear is found then the top of the ear is removed from the polygon and
        /// the process is repeated till only a triangle is left.
        /// </summary>
        /// <param name="polygonList"></param>
        /// <returns></returns>
        private static List<int> triangulate(List<PolygonPoint> polygonList, int offset)
        {
            int index = 1;
            List<int> indexes = new List<int>((polygonList.Count() - 2) * 3);

            PolygonPoint vertex = polygonList[index];
            PolygonPoint next = polygonList[(index + 1) % polygonList.Count()];
            PolygonPoint prev = (index == 0) ? polygonList[polygonList.Count() - 1] : polygonList[index - 1];

            while (polygonList.Count() > 3)
            {
                vertex = polygonList[index];
                if (vertex.isEar)
                {
                    next = polygonList[(index + 1) % polygonList.Count()];
                    prev = (index == 0) ? polygonList[polygonList.Count() - 1] : polygonList[index - 1];

                    indexes.Add(prev.id + offset);
                    indexes.Add(vertex.id + offset);
                    indexes.Add(next.id + offset);

                    polygonList.RemoveAt(index);

                    int prevIndex = (index == 0) ? polygonList.Count() - 1 : index - 1;
                    int nextIndex = index % polygonList.Count();
                    if (prev.isReflex)
                    {
                        isReflex(polygonList, prevIndex);
                    }
                    if (next.isReflex)
                    {
                        isReflex(polygonList, nextIndex);
                    }

                    if (!prev.isReflex)
                    {
                        isEar(polygonList, prevIndex);
                    }
                    if (!next.isReflex)
                    {
                        isEar(polygonList, nextIndex);
                    }
                    index %= polygonList.Count();
                }
                else
                {
                    index = (index + 1) % polygonList.Count();
                }
            }

            vertex = polygonList[index];
            next = polygonList[(index + 1) % polygonList.Count()];
            prev = (index == 0) ? polygonList[polygonList.Count() - 1] : polygonList[index - 1];

            indexes.Add(prev.id + offset);
            indexes.Add(vertex.id + offset);
            indexes.Add(next.id + offset);

            return indexes;
        }

        /// <summary>
        /// Check if the order of the vertices of the polygon are in counter clockwise order
        /// http://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static bool isCounterClockwise(HyperPoint<float>[] polygon)
        {
            double sum = 0.0;
            for (int i = 0; i < polygon.Count(); i++)
            {
                HyperPoint<float> v1 = polygon[i];
                HyperPoint<float> v2 = polygon[(i + 1) % polygon.Count()];
                sum += (v2.X - v1.X) * (v2.Y + v1.Y);
            }
            return sum < 0.0;
        }

    }
}
