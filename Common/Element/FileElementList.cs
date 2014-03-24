using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CG_2IV05.Common.BAG;
using CG_2IV05.Common.OSM;
using micfort.GHL.Math2;

namespace CG_2IV05.Common.Element
{
	public enum SplitDirection : int
	{
		Vertical = 0,
		Horizontal = 1,
	}

	public class FileElementList : IListElement
	{
		public string Filename { get; private set; }

		public FileElementList(string filename)
		{
			this.Filename = filename;
			ReadMetaData();
		}

		public FileElementList[] SplitListBinary(string[] filenames, SplitDirection direction)
		{
			HyperPoint<float> min = this.Min;
			HyperPoint<float> max = this.Max;
			HyperPoint<float> center = (max + min) * 0.5f;

			//todo calculate approximate median
			float split = center[(int)direction];

			FileElementListWriter[] output = new FileElementListWriter[2];
			for (int i = 0; i < output.Length; i++)
			{
				output[i] = new FileElementListWriter(filenames[i]);
			}
			foreach (IElement element in this)
			{
				if (element.ReferencePoint[(int)direction] < split)
				{
					output[0].WriteElement(element);
				}
				else
				{
					output[1].WriteElement(element);
				}
			}
			for (int i = 0; i < output.Length; i++)
			{
				output[i].Dispose();
			}
			FileElementList[] lists = new FileElementList[2];
			for (int i = 0; i < lists.Length; i++)
			{
				lists[i] = new FileElementList(filenames[i]);
			}
			return lists;
		}

		public FileElementList[] SplitList(string[] filenames)
		{
			HyperPoint<float> min = this.Min;
			HyperPoint<float> max = this.Max;

			HyperPoint<float> center = (max + min) * 0.5f;

			FileElementListWriter[] output = new FileElementListWriter[4];
			for (int i = 0; i < 4; i++)
			{
				output[i] = new FileElementListWriter(filenames[i]);
			}
			foreach (IElement element in this)
			{
				if (element.ReferencePoint.Y < center.Y)
				{
					if (element.ReferencePoint.X < center.X)
					{
						output[0].WriteElement(element);
					}
					else
					{
						output[1].WriteElement(element);
					}
				}
				else
				{
					if (element.ReferencePoint.X < center.X)
					{
						output[2].WriteElement(element);
					}
					else
					{
						output[3].WriteElement(element);
					}
				}
			}
			for (int i = 0; i < output.Length; i++)
			{
				output[i].Dispose();
			}
			FileElementList[] lists = new FileElementList[4];
			for (int i = 0; i < 4; i++)
			{
				lists[i] = new FileElementList(filenames[i]);
			}
			return lists;
		}

		public NodeData CreateDataFromChildren(List<Node> Children, HyperPoint<float> centerDataSet, TextureInfo textureInfo,
		                                       out float error)
		{
			throw new NotImplementedException();
		}

		public ElementList ToElementList()
		{
			ElementList output = new ElementList();
			foreach (IElement element in this)
			{
				output.Elements.Add(element);
			}
			return output;
		}

		#region Implementation of IElement

		public bool FinalElement
		{
			get { return false; }
		}

		public int TriangleCount { get; private set; }

		public ScoreKey Score { get; private set; }

		public HyperPoint<float> Min { get; private set; }

		public HyperPoint<float> Max { get; private set; }

		public HyperPoint<float> ReferencePoint { get; private set; }

		public NodeData CreateData(HyperPoint<float> centerDataSet, TextureInfo textureInfo)
		{
			NodeData output = new NodeData();
			List<NodeData> nodeDataList = this.Select(element => element.CreateData(centerDataSet, textureInfo)).ToList();
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
			return new FileElementListEnumerator(Filename);
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

		private void ReadMetaData()
		{
			using (FileStream file = File.OpenRead(Filename))
			{
				TriangleCount = BinaryToStream.ReadIntFromStream(file);
				Min = new HyperPoint<float>(BinaryToStream.ReadFloatFromStream(file), BinaryToStream.ReadFloatFromStream(file));
				Max = new HyperPoint<float>(BinaryToStream.ReadFloatFromStream(file), BinaryToStream.ReadFloatFromStream(file));
			}

			using (var enumerator = this.GetEnumerator())
			{
				enumerator.MoveNext();
				ReferencePoint = enumerator.Current.ReferencePoint;
			}
		}
	}

	class FileElementListEnumerator : IEnumerator<IElement>
	{
		private static List<IElementFactory> factories = new List<IElementFactory>()
			                                                 {
				                                                 new BuildingFactory(),
																 new LandUseFactory(),
																 new RoadFactory()
			                                                 }; 

		private FileStream _s;
		public FileElementListEnumerator(string filename)
		{
			this._s = File.OpenRead(filename);
			Reset();
			Current = null;
		}


		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			_s.Dispose();
		}

		#endregion

		#region Implementation of IEnumerator

		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
		public bool MoveNext()
		{
			if(_s.Position == _s.Length)
			{
				Current = null;
				return false;
			}
			int factoryID = BinaryToStream.ReadIntFromStream(_s);
			IElementFactory factory = factories.Find(x => x.FactoryID == factoryID);
			if(factory == null)
				throw new ArgumentException("Incorrect format file.");
			Current = factory.ReadFromStream(_s);
			return true;
		}

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
		public void Reset()
		{
			Current = null;
			_s.Position = sizeof (float)*4 + sizeof (int);
		}

		/// <summary>
		/// Gets the element in the collection at the current position of the enumerator.
		/// </summary>
		/// <returns>
		/// The element in the collection at the current position of the enumerator.
		/// </returns>
		public IElement Current { get; private set; }

		/// <summary>
		/// Gets the current element in the collection.
		/// </summary>
		/// <returns>
		/// The current element in the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		object IEnumerator.Current
		{
			get { return Current; }
		}

		#endregion
	}
}