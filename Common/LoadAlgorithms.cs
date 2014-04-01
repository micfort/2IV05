using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;
using micfort.GHL.Logging;

namespace CG_2IV05.Common
{
	public interface ILoadAlgorithm<TNodeWithData>
		where TNodeWithData : class, INodeWithData, new()
	{
		void LoadItems(List<TNodeWithData> loadedList, List<TNodeWithData> releaseNodes, List<ReplaceNode<TNodeWithData>> replaceList, HyperPoint<float> position);
	}
	public class AllLoadAlgorithm<TNodeWithData> : ILoadAlgorithm<TNodeWithData>
		where TNodeWithData : class, INodeWithData, new()
	{
		#region Implementation of ILoadAlgorithm<TNodeWithData>

		public void LoadItems(List<TNodeWithData> loadedList, List<TNodeWithData> releaseNodes, List<ReplaceNode<TNodeWithData>> replaceList, HyperPoint<float> position)
		{
			foreach (ReplaceNode<TNodeWithData> replaceNode in replaceList)
			{
				ErrorReporting.Instance.ReportDebugT("AllLoadAlgorithm",
													 "Load nodes from disc " +
													 replaceNode.ReplaceBy.Aggregate("", (s, data) => s + ", " + data.Node.NodeDataFile));
				foreach (TNodeWithData nodeWithData in replaceNode.ReplaceBy)
				{
					nodeWithData.LoadNodeFromDisc();
				}
			}

			foreach (ReplaceNode<TNodeWithData> replaceNode in replaceList)
			{
				lock (loadedList)
				{
					foreach (TNodeWithData nodeWithData in replaceNode.ReplaceBy)
					{
						loadedList.Add(nodeWithData);
					}
					foreach (TNodeWithData originalNode in replaceNode.OriginalNodes)
					{
						loadedList.Remove(originalNode);
					}
				}
			}

			foreach (ReplaceNode<TNodeWithData> replaceNode in replaceList)
			{
				ErrorReporting.Instance.ReportDebugT("AllLoadAlgorithm",
													 "Unload nodes " +
													 replaceNode.OriginalNodes.Aggregate("", (s, data) => s + ", " + data.Node.NodeDataFile));
				lock (releaseNodes)
				{
					foreach (TNodeWithData originalNode in replaceNode.OriginalNodes)
					{
						releaseNodes.Add(originalNode);
					}
				}
			}
		}

		#endregion
	}

	public class ClosestLoadAlgorithm<TNodeWithData> : ILoadAlgorithm<TNodeWithData>
	where TNodeWithData : class, INodeWithData, new()
	{
		class ReplaceNodeComparer: IComparer<ReplaceNode<TNodeWithData>>
		{
			public ReplaceNodeComparer(HyperPoint<float> position)
			{
				Position = position;
			}

			public HyperPoint<float> Position { get; set; } 

			#region Implementation of IComparer<in ReplaceNode<TNodeWithData>>

			/// <summary>
			/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
			/// </summary>
			/// <returns>
			/// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
			/// </returns>
			/// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
			public int Compare(ReplaceNode<TNodeWithData> x, ReplaceNode<TNodeWithData> y)
			{
				return x.ReplaceBy.Min(a => a.Node.DistanceToNode(Position)).CompareTo(y.ReplaceBy.Min(a => a.Node.DistanceToNode(Position)));
			}

			#endregion
		}

		private void LoadNode(List<TNodeWithData> loadedList, List<TNodeWithData> releaseNodes, ReplaceNode<TNodeWithData> currentReplaceNode)
		{
			ErrorReporting.Instance.ReportDebugT("ClosestLoadAlgorithm",
													 "Load nodes from disc " +
													 currentReplaceNode.ReplaceBy.Aggregate("", (s, data) => s + ", " + data.Node.NodeDataFile));

			foreach (TNodeWithData nodeWithData in currentReplaceNode.ReplaceBy)
			{
				nodeWithData.LoadNodeFromDisc();
			}

			lock (loadedList)
			{
				foreach (TNodeWithData nodeWithData in currentReplaceNode.ReplaceBy)
				{
					loadedList.Add(nodeWithData);
				}
				foreach (TNodeWithData originalNode in currentReplaceNode.OriginalNodes)
				{
					loadedList.Remove(originalNode);
				}
			}

			ErrorReporting.Instance.ReportDebugT("ClosestLoadAlgorithm",
			                                     "Unload nodes " +
			                                     currentReplaceNode.OriginalNodes.Aggregate("",
			                                                                                (s, data) =>
			                                                                                s + ", " + data.Node.NodeDataFile));
			lock (releaseNodes)
			{
				foreach (TNodeWithData originalNode in currentReplaceNode.OriginalNodes)
				{
					releaseNodes.Add(originalNode);
				}
			}
		}

		private void RemoveFromList(List<TNodeWithData> loadedList, ReplaceNode<TNodeWithData> currentReleaseNode)
		{
			loadedList.RemoveAll(x => currentReleaseNode.OriginalNodes.Contains(x));
		}

		private void ReleaseNode(List<TNodeWithData> releaseNodes, ReplaceNode<TNodeWithData> currentReleaseNode)
		{
			ErrorReporting.Instance.ReportDebugT("ClosestLoadAlgorithm",
													"Unload nodes " +
													currentReleaseNode.OriginalNodes.Aggregate("", (s, data) => s + ", " + data.Node.NodeDataFile));
			lock (releaseNodes)
			{
				foreach (TNodeWithData originalNode in currentReleaseNode.OriginalNodes)
				{
					releaseNodes.Add(originalNode);
				}
			}
		}
		
		#region Implementation of ILoadAlgorithm<TNodeWithData>

		public void LoadItems(List<TNodeWithData> loadedList, List<TNodeWithData> releaseNodes, List<ReplaceNode<TNodeWithData>> replaceList, HyperPoint<float> position)
		{
			List<ReplaceNode<TNodeWithData>> loadList = replaceList.FindAll(x => x.ReplaceBy.Any());
			if (loadList.Any())
			{
				loadList.Sort(new ReplaceNodeComparer(position));
				ReplaceNode<TNodeWithData> currentReplaceNode = loadList.First();
				loadList.Remove(currentReplaceNode);

				LoadNode(loadedList, releaseNodes, currentReplaceNode);
			}
			List<ReplaceNode<TNodeWithData>> unloadList = replaceList.FindAll(x => x.ReplaceBy.Count == 0);
			lock (loadedList)
			{
				unloadList.ForEach(x => RemoveFromList(loadedList, x));	
			}
			unloadList.ForEach(x => ReleaseNode(releaseNodes, x));
		}

		#endregion
	}
}
