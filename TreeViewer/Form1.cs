using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CG_2IV05.Common;
using micfort.GHL.Math2;
using micfort.GHL.Serialization;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace TreeViewer
{
	public partial class Form1 : Form
	{
		private Tree tree;
		private Node CurrentNode;
		private List<NodeWithData> LoadedData;

		public Form1()
		{
			InitializeComponent();
			LoadedData = new List<NodeWithData>();
		}

		private void btnOpen_Click(object sender, EventArgs e)
		{
			//OpenFileDialog dialog = new OpenFileDialog();
			//if(dialog.ShowDialog() == DialogResult.OK)
			{
				string treePath = @"D:\S120397\School\2IV05 ACCG\2IV05\TreeBuilding\bin\Debug\output\tree";
				using (FileStream file = File.OpenRead(treePath))
				{
					this.tree = SerializableType<Tree>.DeserializeFromStream(file, BinarySerializableTypeEngine.BinairSerializer);
					TreeBuildingSettings.DirectoryOutput = Path.GetDirectoryName(treePath);
				}

				this.CurrentNode = this.tree.Root;
				UpdateChilds();
				UpdateInformation();
				UpdateImage();
			}
		}

		void AddEdges(AdjacencyGraph<Node, Edge> tree, Node node)
		{
			foreach (Node child in node.Children)
			{
				tree.AddVertex(child);
				tree.AddEdge(new Edge(node, child));
				AddEdges(tree, child);
			}
		}

		void graphviz_FormatEdge(object sender, FormatEdgeEventArgs<Node, Edge> e)
		{
		}

		void graphviz_FormatVertex(object sender, FormatVertexEventArgs<Node> e)
		{
			e.VertexFormatter.Label = "";
			byte b = 0;
			if(CurrentNode == e.Vertex)
			{
				b = 255;
			}
			if (LoadedData.Exists(x => x.Node == e.Vertex))
			{
				e.VertexFormatter.FillColor = new GraphvizColor(255, 0, 255, b);
			}
			else
			{
				e.VertexFormatter.FillColor = new GraphvizColor(255, 255, 0, b);
			}
			e.VertexFormatter.Style = GraphvizVertexStyle.Filled;
		}

		private void btnParent_Click(object sender, EventArgs e)
		{
			if(CurrentNode.Parent != null)
			{
				CurrentNode = CurrentNode.Parent;
				UpdateChilds();
				UpdateInformation();
				UpdateImage();
			}
		}

		private void btnChild_Click(object sender, EventArgs e)
		{
			if(cbChilds.SelectedIndex >= 0)
			{
				CurrentNode = CurrentNode.Children[cbChilds.SelectedIndex];
				UpdateChilds();
				UpdateInformation();
				UpdateImage();
			}
		}

		private void UpdateChilds()
		{
			cbChilds.Items.Clear();
			for (int i = 0; i < this.CurrentNode.Children.Count; i++)
			{
				cbChilds.Items.Add(i);
			}
		}

		private void UpdateInformation()
		{
			FileInfo fileInfo = new FileInfo(FilenameGenerator.GetOutputPathToFile(CurrentNode.NodeDataFile));

			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Min: {0}\r\n", CurrentNode.Min);
			sb.AppendFormat("Max: {0}\r\n", CurrentNode.Max);
			sb.AppendFormat("Error: {0}\r\n", CurrentNode.Error);
			sb.AppendFormat("Number of Childeren: {0}\r\n", CurrentNode.Children.Count);
			sb.AppendFormat("Node data: {0}\r\n", CurrentNode.NodeDataFile);
			sb.AppendLine();
			sb.AppendFormat("=Node Data file=\r\n");
			sb.AppendFormat("Size: {0}\r\n", fileInfo.Length / 1024 / 1024);

			tbInfo.Text = sb.ToString();
		}

		private void UpdateImage()
		{
			AdjacencyGraph<Node, Edge> adjacencyGraph = new AdjacencyGraph<Node, Edge>();
			adjacencyGraph.AddVertex(this.tree.Root);
			AddEdges(adjacencyGraph, this.tree.Root);

			GraphVisualiser visualiser = new GraphVisualiser()
			{
				OutputType = "png",
				RendererType = Renderer.Default
			};

			GraphvizAlgorithm<Node, Edge> graphviz = new GraphvizAlgorithm<Node, Edge>(adjacencyGraph);
			graphviz.CommonEdgeFormat.Dir = GraphvizEdgeDirection.None;

			graphviz.FormatEdge += graphviz_FormatEdge;
			graphviz.FormatVertex += graphviz_FormatVertex;

			// render
			graphviz.Generate(visualiser, "");

			Image img = visualiser.OutputImage;
			pictureBox1.Image = img;
		}

		private void btnUpdateLoadedItems_Click(object sender, EventArgs e)
		{
			HyperPoint<float> position = new HyperPoint<float>(3);
			position.X = float.Parse(tbPosX.Text);
			position.Y = float.Parse(tbPosY.Text);
			position.Z = float.Parse(tbPosZ.Text);

			LoadListAlgorithm<NodeWithData> loadListAlgorithm = new LoadListAlgorithm<NodeWithData>();
			loadListAlgorithm.DistanceModifier = float.Parse(tbErrorPerMeter.Text);
			loadListAlgorithm.MaxDistanceError = float.Parse(tbMaxError.Text);
			List<ReplaceNode<NodeWithData>> loadItems = loadListAlgorithm.DetermineCompleteLoadList(this.tree, position, LoadedData);
			foreach (ReplaceNode<NodeWithData> replaceNode in loadItems)
			{
				LoadedData.RemoveAll(x => replaceNode.OriginalNodes.Contains(x));
				LoadedData.AddRange(replaceNode.ReplaceBy);
			}
			UpdateImage();
		}
	}

	public class Edge : IEdge<Node>
	{
		public Edge(Node first, Node second)
		{
			this.Source = first;
			this.Target = second;
		}

		#region Implementation of IEdge<Edge>

		/// <summary>
		/// Gets the source vertex
		/// </summary>
		/// <getter><ensures>Contract.Result&lt;TVertex&gt;() != null</ensures></getter>
		public Node Source { get; private set; }

		/// <summary>
		/// Gets the target vertex
		/// </summary>
		/// <getter><ensures>Contract.Result&lt;TVertex&gt;() != null</ensures></getter>
		public Node Target { get; private set; }

		#endregion
	}
}
