using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CG_2IV05.Common;
using micfort.GHL.Collections;
using micfort.GHL.Math2;

namespace CG_2IV05.Visualize
{
	class NodeManager:IDisposable
	{
		private const float DistanceModifier = 1;
		private const float MaxDistanceError = 10000;

		private Timer timer;
		private List<Node> CurrentLoadedList = new List<Node>();  

		public HyperPoint<float> Position { get; set; }
		public ConcurrentPriorityQueue<NodeLoadItem, int> Queue { get; set; }
		public Tree Tree { get; set; }
		public ConcurrentBag<VBO> VBOBag { get; set; }

		private List<VBO> UnusedVBO = new List<VBO>();

		private void timerCallback(object state)
		{
			List<Node> newLoadedList = this.DetermineCompleteLoadList(Tree, Position);
			List<Node> newItems = DiffNewItems(CurrentLoadedList, newLoadedList);
			List<Node> removedItems = DiffOldItems(CurrentLoadedList, newLoadedList);
			
		}

		public void Start()
		{
			timer = new Timer(timerCallback);
		}

		public void Stop()
		{
			Timer t = timer;
			timer = null;
			if(t != null)
			{
				t.Dispose();
			}
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
			float nodeError = 0;
			if (distanceError > MaxDistanceError)
			{
				return;
			}
			if(distanceError < nodeError && node.Children != null && node.Children.Count > 0)
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
			return DistanceToSquare(new HyperPoint<float>(), new HyperPoint<float>(),  position);
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
			Timer t = timer;
			timer = null;
			if (t != null)
			{
				t.Dispose();
			}
		}

		#endregion
	}
}
