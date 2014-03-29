using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using CG_2IV05.Common;
using OpenTK;
using OpenTK.Graphics;
using micfort.GHL.Collections;
using micfort.GHL.Math2;
using micfort.GHL.Logging;
using Timer = System.Timers.Timer;

namespace CG_2IV05.Visualize
{
	public class NodeManager:IDisposable
	{
		private bool running = false;
		private AutoResetEvent threadFinished = new AutoResetEvent(false);
		private Thread thread;
		private LoadListAlgorithm<NodeWithData> loadListAlgorithm = new LoadListAlgorithm<NodeWithData>()
			                                                            {
				                                                            DistanceModifier = 1000,
																			MaxDistanceError = 10000000
			                                                            };

		public HyperPoint<float> Position { get; set; }
		public Tree Tree { get; set; }
		public List<NodeWithData> VBOList { get; set; }
		public List<NodeWithData> ReleaseNodes { get; set; }
		public VBOLoader Loader { get; set; }
		public float DistanceModifier
		{
			get { return loadListAlgorithm.DistanceModifier; }
			set { loadListAlgorithm.DistanceModifier = value; }
		}

		public float MaxDistanceError
		{
			get { return loadListAlgorithm.MaxDistanceError; }
			set { loadListAlgorithm.MaxDistanceError = value; }
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

			running = true;
			thread = new Thread(threadMethod);
			thread.Name = "Node manager";
			thread.IsBackground = true;
			thread.Start();
		}

		private void threadMethod()
		{
			while (running)
			{
				List<ReplaceNode<NodeWithData>> newLoadedList = loadListAlgorithm.DetermineCompleteLoadList(Tree, Position, VBOList);
				foreach (ReplaceNode<NodeWithData> replaceNode in newLoadedList)
				{
					ErrorReporting.Instance.ReportDebugT(this,
					                                     "Load nodes from disc " +
					                                     replaceNode.ReplaceBy.Aggregate("", (s, data) => s + ", " + data.node.NodeDataFile));
					foreach (NodeWithData nodeWithData in replaceNode.ReplaceBy)
					{
						nodeWithData.loadNodeFromDisc();
					}
				}

				foreach (ReplaceNode<NodeWithData> replaceNode in newLoadedList)
				{
					lock (VBOList)
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

				foreach (ReplaceNode<NodeWithData> replaceNode in newLoadedList)
				{
					ErrorReporting.Instance.ReportDebugT(this,
					                                     "Unload nodes " +
					                                     replaceNode.OriginalNodes.Aggregate("", (s, data) => s + ", " + data.node.NodeDataFile));
					lock (ReleaseNodes)
					{
						foreach (NodeWithData originalNode in replaceNode.OriginalNodes)
						{
							ReleaseNodes.Add(originalNode);
						}
					}
				}
				Thread.Sleep(1000/10);
			}
			threadFinished.Set();
		}

		public void Stop()
		{
			running = false;
			threadFinished.WaitOne();
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
