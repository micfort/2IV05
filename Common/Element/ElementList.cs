using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using micfort.GHL.Math2;

namespace CG_2IV05.Common.Element
{
	public class ElementList : IListElement
	{
		public List<IElement> Elements { get; set; }

        public ElementList()
        {
			Elements = new List<IElement>();
        } 

		public ElementList[] SplitList()
		{
			HyperPoint<float> min = this.Min;
			HyperPoint<float> max = this.Max;

			HyperPoint<float> center = (max + min) * 0.5f;

			ElementList[] output = new ElementList[4];
			for (int i = 0; i < 4; i++)
			{
				output[i] = new ElementList();
			}
			for (int i = 0; i < Elements.Count; i++)
			{
				if (Elements[i].ReferencePoint.Y < center.Y)
				{
					if (Elements[i].ReferencePoint.X < center.X)
					{
						output[0].Elements.Add(Elements[i]);
					}
					else
					{
						output[1].Elements.Add(Elements[i]);
					}
				}
				else
				{
					if (Elements[i].ReferencePoint.X < center.X)
					{
						output[2].Elements.Add(Elements[i]);
					}
					else
					{
						output[3].Elements.Add(Elements[i]);
					}
				}
			}
			return output;
		}

		public NodeData CreateDataFromChildren(List<Node> Children, HyperPoint<float> centerDataSet, TextureInfo textureInfo, out float error)
		{
			SortedList<ScoreKey, IElement> sortedElements = new SortedList<ScoreKey, IElement>();
		    int triangleCount = 0;
            foreach(Node node in Children)
            {
                var nodeElements = (ElementList) node.Tag;
                foreach (var element in nodeElements.Elements)
                {
                    sortedElements.Add(element.Score, element);
                    triangleCount += element.TriangleCount;
                }
		    }

			int originalTriangleCount = triangleCount;
		    
            while (triangleCount > TreeBuildingSettings.MaxTriangleCount)
            {
				IElement element = sortedElements.First().Value;

                if (element.Score.Score == float.MaxValue)
                    break;

                triangleCount -= element.TriangleCount;
                element = element.GetSimplifiedVersion();
                sortedElements.RemoveAt(0);
                sortedElements.Add(element.Score, element);

                triangleCount += element.TriangleCount;
            }
		    Elements = sortedElements.Values.ToList();
            sortedElements.Clear();

			error = originalTriangleCount - triangleCount;
            return this.CreateData(centerDataSet, textureInfo);
		}

		#region Implementation of IElement

		public bool FinalElement { get { return Elements.Count <= 1; } }

		public int TriangleCount 
		{ 
			get { return Elements.Sum(b => b.TriangleCount); } 
		}

	    public ScoreKey Score { get; private set; }

	    public HyperPoint<float> Min
		{
			get { return new HyperPoint<float>(Elements.Min(x => x.Min.X), Elements.Min(x => x.Min.Y), 0); }
		}

		public HyperPoint<float> Max
		{
			get { return new HyperPoint<float>(Elements.Max(x => x.Max.X), Elements.Max(x => x.Max.Y), 0); }
		}

		public HyperPoint<float> ReferencePoint
		{
			get { return Elements[0].ReferencePoint; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="building"></param>
		/// <returns></returns>
		/// <remarks>O(P)</remarks>
		public NodeData CreateData(HyperPoint<float> centerDataSet, TextureInfo textureInfo)
		{
			NodeData output = new NodeData();
			List<NodeData> nodeDataList = Elements.ConvertAll(b => b.CreateData(centerDataSet, textureInfo));
			int verticesCount = nodeDataList.ConvertAll(x => x.Vertices.Length).Sum();
			int indexesCount = nodeDataList.ConvertAll(x => x.Indexes.Length).Sum();

			output.Vertices = new HyperPoint<float>[verticesCount];
			output.Normals = new HyperPoint<float>[verticesCount];
			output.TextCoord = new HyperPoint<float>[verticesCount];
			output.Indexes = new int[indexesCount];

			int verticesI = 0;
			int indexI = 0;

			for (int i = 0; i < nodeDataList.Count; i++)
			{
				for (int j = 0; j < nodeDataList[i].Indexes.Length; j++)
				{
					nodeDataList[i].Indexes[j] += verticesI;
				}
				Array.Copy(nodeDataList[i].Vertices, 0, output.Vertices, verticesI, nodeDataList[i].Vertices.Length);
				Array.Copy(nodeDataList[i].Normals, 0, output.Normals, verticesI, nodeDataList[i].Normals.Length);
				Array.Copy(nodeDataList[i].TextCoord, 0, output.TextCoord, verticesI, nodeDataList[i].TextCoord.Length);
				Array.Copy(nodeDataList[i].Indexes, 0, output.Indexes, indexI, nodeDataList[i].Indexes.Length);
				verticesI += nodeDataList[i].Vertices.Length;
				indexI += nodeDataList[i].Indexes.Length;
			}

			return output;
		}

	    public IElement GetSimplifiedVersion(HyperPoint<float> centerDataSet, TextureInfo textureInfo)
	    {
	        throw new NotImplementedException();
	    }

		#endregion

		#region Implementation of IEnumerable

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<IElement> GetEnumerator()
		{
			return Elements.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}