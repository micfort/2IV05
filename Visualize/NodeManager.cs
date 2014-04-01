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
																			//DistanceModifierConstant = -1000,
				                                                            DistanceModifierLinear = 1000,
																			//DistanceModifierQuadratic = 0.5f,
																			MaxDistanceError = 10000000
			                                                            };

		private ILoadAlgorithm<NodeWithData> loadAlgorithm = new ClosestLoadAlgorithm<NodeWithData>(); 

		public HyperPoint<float> Position { get; set; }
		public Tree Tree { get; set; }
		public List<NodeWithData> VBOList { get; set; }
		public List<NodeWithData> ReleaseNodes { get; set; }
		public VBOLoader Loader { get; set; }
		public float DistanceModifier
		{
			get { return loadListAlgorithm.DistanceModifierLinear; }
			set { loadListAlgorithm.DistanceModifierLinear = value; }
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
			nodeWithData.LoadNodeFromDisc();
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
				List<ReplaceNode<NodeWithData>> replaceList = loadListAlgorithm.DetermineCompleteLoadList(Tree, Position, VBOList);
				loadAlgorithm.LoadItems(VBOList, ReleaseNodes, replaceList, Position);
				Thread.Sleep(1000 / 10);
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
