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
	public struct ReplaceNode
	{
		public List<NodeWithData> OriginalNodes;
		public List<NodeWithData> ReplaceBy;
	}
	public class NodeManager:IDisposable
	{
		private Timer timer;
		private float _maxDistanceError = 1000000;
		private float _distanceModifier = 10;

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
			NodeWithData nodeWithData = new NodeWithData();
			nodeWithData.node = Tree.Root;
			nodeWithData.loadNodeFromDisc();
			lock (VBOList)
			{
				VBOList.Add(nodeWithData);
			}

			timer = new Timer();
			timer.Tick += timer_Tick;
			timer.Interval = 1000/10;
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			List<ReplaceNode> newLoadedList = this.DetermineCompleteLoadList(Tree, Position);
			foreach (ReplaceNode replaceNode in newLoadedList)
			{
				micfort.GHL.Logging.ErrorReporting.Instance.ReportDebugT(this, "Load nodes from disc " + replaceNode.ReplaceBy.Aggregate("", (s, data) => s + ", " + data.node.NodeDataFile));
				foreach (NodeWithData nodeWithData in replaceNode.ReplaceBy)
				{
					nodeWithData.loadNodeFromDisc();
				}
			}
			lock (VBOList)
			{
				foreach (ReplaceNode replaceNode in newLoadedList)
				{
					foreach (NodeWithData nodeWithData in replaceNode.ReplaceBy)
					{
						VBOList.Add(nodeWithData);
					}
					foreach (NodeWithData originalNode in replaceNode.OriginalNodes)
					{
						VBOList.Remove(originalNode);
					}
				}
			}

			foreach (ReplaceNode replaceNode in newLoadedList)
			{
				micfort.GHL.Logging.ErrorReporting.Instance.ReportDebugT(this, "Unload nodes from disc " + replaceNode.OriginalNodes.Aggregate("", (s, data) => s + ", " + data.node.NodeDataFile));
				foreach (NodeWithData originalNode in replaceNode.OriginalNodes)
				{
					originalNode.ReleaseVBO();
				}
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

		public List<ReplaceNode> DetermineCompleteLoadList(Tree tree, HyperPoint<float> position)
		{
			List<ReplaceNode> replaceList = new List<ReplaceNode>();
			DetermineLoadList(tree.Root, position, replaceList);
			return replaceList;
		}

		private void DetermineLoadList(Node node, HyperPoint<float> position, List<ReplaceNode> loadList)
		{
			float distanceError = DistanceToNode(node, position) * DistanceModifier;
			if (distanceError > MaxDistanceError)
			{
				if(VBOList.Exists(x => x.node == node))
				{
					List<NodeWithData> unloadList = new List<NodeWithData>() { VBOList.Find(x => x.node == node) };
					List<NodeWithData> newLoadList = new List<NodeWithData>();
					loadList.Add(new ReplaceNode() { OriginalNodes = unloadList, ReplaceBy = newLoadList });
				}
				return;
			}
			if(distanceError < node.Error && node.Children != null && node.Children.Count > 0)
			{
				if(VBOList.Exists(x => x.node == node))
				{
					List<NodeWithData> unloadList = new List<NodeWithData>() {VBOList.Find(x => x.node == node)};
					List<NodeWithData> newLoadList = new List<NodeWithData>();
					foreach (Node child in node.Children)
					{
						DetermineLoadListForUnloadingParent(child, position, newLoadList);
					}
					loadList.Add(new ReplaceNode(){OriginalNodes = unloadList, ReplaceBy = newLoadList});
				}
				else
				{
					foreach (Node child in node.Children)
					{
						DetermineLoadList(child, position, loadList);
					}
				}
			}
			else
			{
				if(!VBOList.Exists(x => x.node == node))
				{
					List<NodeWithData> newLoadList = new List<NodeWithData>() {new NodeWithData(null, node)};
					List<NodeWithData> unloadList = new List<NodeWithData>();
					foreach (Node child in node.Children)
					{
						DetermineLoadListForLoadingParent(child, unloadList);
					}
					loadList.Add(new ReplaceNode() {OriginalNodes = unloadList, ReplaceBy = newLoadList});
				}
			}
		}

		private void DetermineLoadListForUnloadingParent(Node node, HyperPoint<float> position, List<NodeWithData> loadList)
		{
			float distanceError = DistanceToNode(node, position) * DistanceModifier;
			if (distanceError > MaxDistanceError)
			{
				return;
			}
			if (distanceError < node.Error && node.Children != null && node.Children.Count > 0)
			{
				foreach (Node child in node.Children)
				{
					DetermineLoadListForUnloadingParent(child, position, loadList);
				}
			}
			else
			{
				loadList.Add(new NodeWithData(null, node));
			}
		}

		private void DetermineLoadListForLoadingParent(Node node, List<NodeWithData> unLoadList)
		{
			if(VBOList.Exists(x => x.node == node))
			{
				unLoadList.Add(VBOList.Find(x => x.node == node));
			}
			else
			{
				foreach (Node child in node.Children)
				{
					DetermineLoadListForLoadingParent(child, unLoadList);
				}
			}
		}

		private float DistanceToNode(Node node, HyperPoint<float> position)
		{
			//todo set bounding box of node
			return DistanceToSquare(node.Min, node.Max, position);
		}

		private float DistanceToSquare(HyperPoint<float> p1, HyperPoint<float> p2, HyperPoint<float> position)
		{
			HyperPoint<float> b = p2 - p1;
			HyperPoint<float> p = position - p1;
			HyperPoint<float> abs_p = new HyperPoint<float>(Math.Abs(p.X), Math.Abs(p.Y), Math.Abs(p.Z));
			HyperPoint<float> sub = abs_p - b;
			HyperPoint<float> max = new HyperPoint<float>(Math.Max(sub.X, 0f), Math.Max(sub.Y, 0f), Math.Max(sub.Z, 0f));
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
