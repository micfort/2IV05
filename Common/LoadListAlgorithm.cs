using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;

namespace CG_2IV05.Common
{
	public struct ReplaceNode<TNodeWithData>
		where TNodeWithData : INodeWithData
	{
		public List<TNodeWithData> OriginalNodes;
		public List<TNodeWithData> ReplaceBy;
	}
	public interface INodeWithData
	{
		Node Node { get; set; }
	}
	public class LoadListAlgorithm<TNodeWithData>
		where TNodeWithData : class, INodeWithData, new()
	{
		public float DistanceModifierConstant { get; set; }
		public float DistanceModifierLinear { get; set; }
		public float DistanceModifierQuadratic { get; set; }
		public float MaxDistanceError { get; set; }

		public List<ReplaceNode<TNodeWithData>> DetermineCompleteLoadList(Tree tree, HyperPoint<float> position, List<TNodeWithData> LoadedList)
		{
			position.Z = 0;
			List<ReplaceNode<TNodeWithData>> replaceList = new List<ReplaceNode<TNodeWithData>>();
			DetermineLoadList(tree.Root, position, replaceList, LoadedList);
			return replaceList;
		}

		private void DetermineLoadList(Node node, HyperPoint<float> position, List<ReplaceNode<TNodeWithData>> loadList, List<TNodeWithData> LoadedList)
		{
			float distanceError = CalculateDistanceError(node, position);
			if (distanceError > MaxDistanceError)
			{
				List<TNodeWithData> unloadList = new List<TNodeWithData>();
				List<TNodeWithData> newLoadList = new List<TNodeWithData>();
				DetermineUnloadListForError(node, unloadList, LoadedList);

				if (unloadList.Any()) loadList.Add(new ReplaceNode<TNodeWithData>() { OriginalNodes = unloadList, ReplaceBy = newLoadList });
			}
			else if (distanceError < node.Error && node.Children != null && node.Children.Count > 0)
			{
				if (LoadedList.Exists(x => x.Node == node))
				{
					List<TNodeWithData> unloadList = new List<TNodeWithData>() { LoadedList.Find(x => x.Node == node) };
					List<TNodeWithData> newLoadList = new List<TNodeWithData>();
					foreach (Node child in node.Children)
					{
						DetermineLoadListForUnloadingParent(child, position, newLoadList, unloadList, LoadedList);
					}
					loadList.Add(new ReplaceNode<TNodeWithData>() { OriginalNodes = unloadList, ReplaceBy = newLoadList });
				}
				else
				{
					foreach (Node child in node.Children)
					{
						DetermineLoadList(child, position, loadList, LoadedList);
					}
				}
			}
			else
			{
				if (!LoadedList.Exists(x => x.Node == node))
				{
					List<TNodeWithData> newLoadList = new List<TNodeWithData>() { new TNodeWithData(){Node = node} };
					List<TNodeWithData> unloadList = new List<TNodeWithData>();
					foreach (Node child in node.Children)
					{
						DetermineUnloadListForLoadingParent(child, unloadList, LoadedList);
					}
					loadList.Add(new ReplaceNode<TNodeWithData>() { OriginalNodes = unloadList, ReplaceBy = newLoadList });
				}
			}
		}

		private void DetermineLoadListForUnloadingParent(Node node, HyperPoint<float> position, List<TNodeWithData> loadList, List<TNodeWithData> unloadList, List<TNodeWithData> LoadedList)
		{
			float distanceError = CalculateDistanceError(node, position);
			if (distanceError > MaxDistanceError)
			{
				DetermineUnloadListForError(node, unloadList, LoadedList);
			}
			else if (distanceError < node.Error && node.Children != null && node.Children.Count > 0)
			{
				foreach (Node child in node.Children)
				{
					DetermineLoadListForUnloadingParent(child, position, loadList, unloadList, LoadedList);
				}
			}
			else
			{
				loadList.Add(new TNodeWithData(){Node = node});
			}
		}

		private void DetermineUnloadListForLoadingParent(Node node, List<TNodeWithData> unLoadList, List<TNodeWithData> LoadedList)
		{
			if (LoadedList.Exists(x => x.Node == node))
			{
				unLoadList.Add(LoadedList.Find(x => x.Node == node));
			}
			foreach (Node child in node.Children)
			{
				DetermineUnloadListForLoadingParent(child, unLoadList, LoadedList);
			}
		}

		private void DetermineUnloadListForError(Node node, List<TNodeWithData> unLoadList, List<TNodeWithData> LoadedList)
		{
			var nwd = LoadedList.Find(x => x.Node == node);
			if (nwd != null)
			{
				unLoadList.Add(nwd);
			}
			node.Children.ForEach(x => DetermineUnloadListForError(x, unLoadList, LoadedList));
		}

		private float CalculateDistanceError(Node node, HyperPoint<float> position)
		{
			float distance = DistanceToNode(node, position);
			float distanceError = distance * distance * DistanceModifierQuadratic + distance * DistanceModifierLinear + DistanceModifierConstant;
			distanceError = Math.Max(0, distanceError);
			return distanceError;
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
			double distance = Math.Sqrt(max.X * max.X + max.Y * max.Y + max.Z * max.Z);
			return Convert.ToSingle(distance);
		}
	}
}
