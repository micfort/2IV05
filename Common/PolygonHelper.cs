﻿using System;
using System.Collections.Generic;
using System.IO;
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

		public static bool IsSimplePolygon(List<HyperPoint<float>> poly)
		{
			HyperPoint<float> crossing;
			int oi;
			int oj;
			return SimplePolygon(poly, out crossing, out oi, out oj);
		}

		public static List<List<HyperPoint<float>>> CreateSimplePolygons(List<HyperPoint<float>> poly)
		{
			List<List<HyperPoint<float>>> output = new List<List<HyperPoint<float>>>();
			HyperPoint<float> crossing;
			int i, j;
			if (SimplePolygon(poly, out crossing, out i, out j))
			{
				output.Add(poly);
			}
			else
			{
				List<HyperPoint<float>> polygon1 = CreateSubPolygonMin(poly, crossing, i, j);
				List<HyperPoint<float>> polygon2 = CreateSubPolygonMin(poly, crossing, j, i);
				List<List<HyperPoint<float>>> simplePolygonList1 = CreateSimplePolygons(polygon1);
				List<List<HyperPoint<float>>> simplePolygonList2 = CreateSimplePolygons(polygon2);
				simplePolygonList1.ForEach(output.Add);
				simplePolygonList2.ForEach(output.Add);
			}
			return output;
		}


		private static List<HyperPoint<float>> CreateSubPolygonMin(List<HyperPoint<float>> complexPoly, HyperPoint<float> crossing, int from, int to)
		{
			List<HyperPoint<float>> output = new List<HyperPoint<float>>();
			while (from != to)
			{
				output.Add(complexPoly[from]);
				from--;
				if (from == -1)
				{
					from = complexPoly.Count - 1;
				}
			}
			output.Add(crossing);
			return output;
		}

		private static bool SimplePolygon(List<HyperPoint<float>> poly, out HyperPoint<float> crossing, out int oi, out int oj)
		{
			for (int i = 0; i < poly.Count; i++)
			{
				for (int j = 0; j < poly.Count; j++)
				{
					int k = i + 1;
					int l = j + 1;
					if (k == poly.Count)
						k = 0;
					if (l == poly.Count)
						l = 0;
					if (i != l && i != j && k != j)
					{
						if (Intersect(poly[i], poly[k], poly[j], poly[l], out crossing))
						{
							oi = i;
							oj = j;
							return false;
						}
					}
				}
			}
			oi = -1;
			oj = -1;
			crossing = new HyperPoint<float>(2);
			return true;
		}

		/// <summary>
		/// http://thirdpartyninjas.com/blog/2008/10/07/line-segment-intersection/
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="p3"></param>
		/// <param name="p4"></param>
		/// <returns></returns>
		private static bool Intersect(HyperPoint<float> p1, HyperPoint<float> p2, HyperPoint<float> p3, HyperPoint<float> p4, out HyperPoint<float> crossing)
		{
			float Ua = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / ((p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y));
			float Ub = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / ((p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y));
			crossing = new HyperPoint<float>(p1.X + Ua * (p2.X - p1.X), p1.Y + Ub * (p2.Y - p1.Y));
			return 0 <= Ua && Ua <= 1 && 0 <= Ub && Ub <= 1;
		}

		public static void WriteToStream(Stream stream, List<HyperPoint<float>> poly)
		{
			BinaryToStream.WriteToStream(poly.Count, stream);
			for (int i = 0; i < poly.Count; i++)
			{
				WriteToStream(stream, poly[i]);
			}
		}

		public static List<HyperPoint<float>> ReadPolyFromStream(Stream stream)
		{
			int count = BinaryToStream.ReadIntFromStream(stream);
			List<HyperPoint<float>> poly = new List<HyperPoint<float>>(count);
			for (int i = 0; i < count; i++)
			{
				poly.Add(ReadHyperPointFromStream(stream));
			}
			return poly;
		}

		public static void WriteToStream(Stream stream, HyperPoint<float> hyperPoint)
		{
			BinaryToStream.WriteToStream(hyperPoint.Dim, stream);
			for (int i = 0; i < hyperPoint.Dim; i++)
			{
				BinaryToStream.WriteToStream(hyperPoint[i], stream);
			}
		}

		public static HyperPoint<float> ReadHyperPointFromStream(Stream stream)
		{
			int dim = BinaryToStream.ReadIntFromStream(stream);
			float[] p = new float[dim];
			for (int i = 0; i < dim; i++)
			{
				p[i] = BinaryToStream.ReadFloatFromStream(stream);
			}
			return new HyperPoint<float>(p);
		}

		public static List<HyperPoint<float>> CreateConvexHull(List<HyperPoint<float>> p)
		{
			List<int> cnvxhll = new List<int>();

			int b = 0;
			for (int i = 0; i < p.Count; i++)
			{
				if (p[i].Y < p[b].Y || p[i].Y == p[b].Y && p[i].X < p[b].X)
					b = i;
			}
			cnvxhll.Add(b);
			int first = b;
			int cur = b;
			int next = 0; //
			do
			{
				bool f = true;
				for (int i = 0; i < p.Count; i++)
				{
					if (f)
					{
						next = i;
						f = false;
					}
					HyperPoint<float> v1 = p[i] - p[cur];
					HyperPoint<float> v2 = p[next] - p[cur];
					float c = HyperPoint<float>.Cross2D(v1.GetLowerDim(2), v2.GetLowerDim(2));
					if (c > 0) next = i;
					if ((c == 0) && (distanceSquared(p[cur], p[i]) > distanceSquared(p[cur], p[next]))) next = i;
				}
				cur = next;
				cnvxhll.Add(next);
			}
			while (cur != first);
			return cnvxhll.ConvertAll(x => p[x]).Take(cnvxhll.Count - 1).ToList();
		}

		private static double distanceSquared(HyperPoint<float> p1, HyperPoint<float> p2)
		{
			return (p1 - p2).GetLengthSquared();
		}
	}
}
