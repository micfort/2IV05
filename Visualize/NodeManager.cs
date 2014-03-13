using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CG_2IV05.Common;
using micfort.GHL.Collections;
using micfort.GHL.Math2;

namespace CG_2IV05.Visualize
{
	public class NodeManager:IDisposable
	{
		private Timer timer;
		private List<Node> CurrentLoadedList = new List<Node>();
		private float _maxDistanceError = 1000000;
		private float _distanceModifier = 1;

		public HyperPoint<float> Position { get; set; }
		public Tree Tree { get; set; }
		public List<NodeWithData> VBOList { get; set; }
		public VBOLoader Loader { get; set; }
		public float DistanceModifier
		{
			get { return _distanceModifier; }
			set { _distanceModifier = value; }
		}

		public float MaxDistanceError
		{
			get { return _maxDistanceError; }
			set { _maxDistanceError = value; }
		}

		public void Start()
		{
			timer = new Timer();
			timer.Tick += timer_Tick;
			timer.Interval = 1000/10;
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			List<Node> newLoadedList = this.DetermineCompleteLoadList(Tree, Position);
			List<Node> newItems = DiffNewItems(CurrentLoadedList, newLoadedList);
			List<Node> removedItems = DiffOldItems(CurrentLoadedList, newLoadedList);
			CurrentLoadedList = newLoadedList;

			for (int i = 0; i < newItems.Count; i++)
			{
				if (removedItems.Count > 0)
				{
					Node ReplacedNode = removedItems[0];
					removedItems.RemoveAt(0);
					NodeWithData replaceData = new NodeWithData();
					int index = -1;
					lock (VBOList)
					{
						index = VBOList.FindIndex(x => x.node == ReplacedNode);
						if(index >= 0)
						{
							replaceData = VBOList[index];
							VBOList.RemoveAt(index);
						}
					}
					if (index >= 0)
					{
						NodeWithData item = new NodeWithData();
						item.vbo = replaceData.vbo;
						item.node = newItems[i];
						Loader.enqueueNode(item);
					}
				}
				else
				{
					NodeWithData item = new NodeWithData();
					item.vbo = null;
					item.node = newItems[i];
					Loader.enqueueNode(item);
				}
				
			}
			for (int i = 0; i < removedItems.Count; i++)
			{
				int index = VBOList.FindIndex(x => x.node == removedItems[i]);
				NodeWithData replaceData = VBOList[index];
				lock (VBOList)
				{
					VBOList.RemoveAt(index);
				}
				replaceData.vbo.Dispose();
			}
		}

		public void Stop()
		{
			timer.Stop();
		}

		public List<Node> DiffNewItems(List<Node> oldList, List<Node> newList)
		{
			List<Node> diffList = new List<Node>();
			foreach (Node node in newList)
			{
				if (!oldList.Contains(node))
				{
					diffList.Add(node);
				}
			}
			return diffList;
		} 

		public List<Node> DiffOldItems(List<Node> oldList, List<Node> newList)
		{
			return DiffNewItems(newList, oldList);
		}

		public List<Node> DetermineCompleteLoadList(Tree tree, HyperPoint<float> position)
		{
			List<Node> loadList = new List<Node>();
			DetermineLoadList(tree.Root, position, loadList);
			return loadList;
		} 

		private void DetermineLoadList(Node node, HyperPoint<float> position, List<Node> loadList)
		{
			float distanceError = DistanceToNode(node, position) * DistanceModifier;
			if (distanceError > MaxDistanceError)
			{
				return;
			}
			if(distanceError < node.Error && node.Children != null && node.Children.Count > 0)
			{
				foreach (Node child in node.Children)
				{
					DetermineLoadList(child, position, loadList);
				}
			}
			else
			{
				loadList.Add(node);
			}
		}

		private float DistanceToNode(Node node, HyperPoint<float> position)
		{
			//todo set bounding box of node
			return DistanceToSquare(node.Min, node.Max,  position);
		}

		private float DistanceToSquare(HyperPoint<float> p1, HyperPoint<float> p2, HyperPoint<float> position)
		{
			HyperPoint<float> b = p2 - p1;
			HyperPoint<float> p = position - p1;
			HyperPoint<float> abs_p = new HyperPoint<float>(Math.Abs(p.X), Math.Abs(p.Y));
			HyperPoint<float> sub = abs_p - b;
			HyperPoint<float> max = new HyperPoint<float>(Math.Max(sub.X, 0f), Math.Max(sub.Y, 0f));
			return max.GetLength();
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
		}

		#endregion
	}
}
