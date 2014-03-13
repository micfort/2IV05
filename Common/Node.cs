using System;
using System.Collections.Generic;
using System.IO;
using CG_2IV05.Common.Element;

namespace CG_2IV05.Common
{
	[Serializable]
    public class Node
    {
		public Node()
		{
			
		}

		public Node(ElementList elements, Node parent, TextureInfo textureInfo)
		{
			Children = new List<Node>();
			NodeDataFile = FilenameGenerator.CreateFilename();
			Parent = parent;
			Tag = elements;
			if (elements.FinalElement || elements.TriangleCount < TreeBuildingSettings.MaxTriangleCount)
			{
				//Leave
				NodeData data = elements.CreateData(TreeBuildingSettings.CenterDataSet, textureInfo);
				using (FileStream file = File.Open(NodeDataFile, FileMode.Create, FileAccess.ReadWrite))
				{
					data.SaveToStream(file);
				}
			}
			else
			{
				//Not a leave
				ElementList[] split = elements.SplitList();
				for (int i = 0; i < 4; i++)
				{
					Node child = new Node(split[i], this, textureInfo);
					Children.Add(child);
				}

				NodeData data = elements.CreateDataFromChildren(Children, TreeBuildingSettings.CenterDataSet, textureInfo);
				using (FileStream file = File.Open(NodeDataFile, FileMode.Create, FileAccess.ReadWrite))
				{
					data.SaveToStream(file);
				}
			}
		}

	    public List<Node> Children { get; set; }
	    public Node Parent { get; set; }
	    public string NodeDataFile { get; set; }
	    public object Tag { get; set; }

		public NodeData ReadData()
		{
			using (FileStream file = File.Open(NodeDataFile, FileMode.Open, FileAccess.Read))
			{
				return NodeData.ReadFromStream(file);
			}
		}

		public NodeDataRaw ReadRawData()
		{
			using (FileStream file = File.Open(NodeDataFile, FileMode.Open, FileAccess.Read))
			{
				return NodeDataRaw.ReadFromStream(file);
			}
		}
    }
}
