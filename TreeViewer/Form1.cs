﻿using System;
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
			OpenFileDialog dialog = new OpenFileDialog();
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				using (FileStream file = File.OpenRead(dialog.FileName))
				{
					this.tree = SerializableType<Tree>.DeserializeFromStream(file, BinarySerializableTypeEngine.BinairSerializer);
					TreeBuildingSettings.DirectoryOutput = Path.GetDirectoryName(dialog.FileName);
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
			sb.AppendFormat("=Tree information=\r\n");
			sb.AppendFormat("Total tree size: {0}\r\n", GetNodeSize(tree.Root) / 1024 / 1024);
			sb.AppendFormat("Total leaf size: {0}\r\n", GetLeafSize(tree.Root) / 1024 / 1024);
			sb.AppendLine();
			sb.AppendFormat("=Current node=\r\n");
			sb.AppendFormat("Min: {0}; {1}\r\n", CurrentNode.Min.X, CurrentNode.Min.Y);
			sb.AppendFormat("Max: {0}; {1}\r\n", CurrentNode.Max.X, CurrentNode.Max.Y);
			sb.AppendFormat("Error: {0}\r\n", CurrentNode.Error);
			sb.AppendFormat("Number of Childeren: {0}\r\n", CurrentNode.Children.Count);
			sb.AppendFormat("Node data: {0}\r\n", CurrentNode.NodeDataFile);
			sb.AppendLine();
			sb.AppendFormat("=Current Node Data file=\r\n");
			sb.AppendFormat("Size: {0}\r\n", fileInfo.Length / 1024 / 1024);

			tbInfo.Text = sb.ToString();
		}

		private void UpdateImage()
		{
			Image img = GetImage();
			pictureBox1.Image = img;
		}

		private Image GetImage(bool fileOutput = false, string filename = "")
		{
			AdjacencyGraph<Node, Edge> adjacencyGraph = new AdjacencyGraph<Node, Edge>();
			adjacencyGraph.AddVertex(this.tree.Root);
			AddEdges(adjacencyGraph, this.tree.Root);

			GraphVisualiser visualiser = new GraphVisualiser()
			{
				OutputType = fileOutput?"pdf":"png",
				RendererType = Renderer.Default,
				OutputFile = fileOutput
			};

			GraphvizAlgorithm<Node, Edge> graphviz = new GraphvizAlgorithm<Node, Edge>(adjacencyGraph);
			graphviz.CommonEdgeFormat.Dir = GraphvizEdgeDirection.None;

			graphviz.FormatEdge += graphviz_FormatEdge;
			graphviz.FormatVertex += graphviz_FormatVertex;

			// render
			graphviz.Generate(visualiser, filename);

			if(fileOutput == false)
			{
				return visualiser.OutputImage;
			}
			else
			{
				return null;
			}
		}

		private void btnUpdateLoadedItems_Click(object sender, EventArgs e)
		{
			HyperPoint<float> position = new HyperPoint<float>(3);
			position.X = float.Parse(tbPosX.Text);
			position.Y = float.Parse(tbPosY.Text);
			position.Z = float.Parse(tbPosZ.Text);

			this.LoadedData = new List<NodeWithData>();

			LoadListAlgorithm<NodeWithData> loadListAlgorithm = new LoadListAlgorithm<NodeWithData>();
			loadListAlgorithm.DistanceModifierLinear = float.Parse(tbErrorPerMeter.Text);
			loadListAlgorithm.MaxDistanceError = float.Parse(tbMaxError.Text);
			List<ReplaceNode<NodeWithData>> loadItems = loadListAlgorithm.DetermineCompleteLoadList(this.tree, position, LoadedData);
			foreach (ReplaceNode<NodeWithData> replaceNode in loadItems)
			{
				LoadedData.RemoveAll(x => replaceNode.OriginalNodes.Contains(x));
				LoadedData.AddRange(replaceNode.ReplaceBy);
			}
			UpdateImage();
		}

		private void btnChild0_Click(object sender, EventArgs e)
		{
			if(CurrentNode.Children.Count > 0)
			{
				CurrentNode = CurrentNode.Children[0];
				UpdateChilds();
				UpdateInformation();
				UpdateImage();
			}
		}

		private void btnChild1_Click(object sender, EventArgs e)
		{
			if(CurrentNode.Children.Count > 1)
			{
				CurrentNode = CurrentNode.Children[1];
				UpdateChilds();
				UpdateInformation();
				UpdateImage();
			}
		}

		private void btnSaveImage_Click(object sender, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			//dialog.Filter = "Image Files(*.png;*.bmp;*.jpg;*.gif)|*.png;*.bmp;*.jpg;*.gif|All files (*.*)|*.*";
			dialog.Filter = "*.pdf|*.pdf";
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				GetImage(true, dialog.FileName);
			}
		}

		private long GetNodeSize(Node node)
		{
			long size = 0;
			FileInfo fileInfo = new FileInfo(FilenameGenerator.GetOutputPathToFile(node.NodeDataFile));
			size += fileInfo.Length;
			size += node.Children.Aggregate(0l, (l, child) => l + GetNodeSize(child));
			return size;
		}

		private long GetLeafSize(Node node)
		{
			long size = 0;
			if(node.Children.Count == 0)
			{
				FileInfo fileInfo = new FileInfo(FilenameGenerator.GetOutputPathToFile(node.NodeDataFile));
				size += fileInfo.Length;
			}
			size += node.Children.Aggregate(0l, (l, child) => l + GetLeafSize(child));
			return size;
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
