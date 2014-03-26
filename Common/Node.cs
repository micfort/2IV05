using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CG_2IV05.Common.Element;
using micfort.GHL.Math2;

namespace CG_2IV05.Common
{
    [Serializable]
    public class Node
    {
	    private HyperPointSerializable<float> _min;
	    private HyperPointSerializable<float> _max;
	    private float _error;

	    public Node()
        {

        }

        public Node(ElementList elements, Node parent, TextureInfo textureInfo)
        {
            Children = new List<Node>();
            NodeDataFile = FilenameGenerator.CreateFilename();
            Parent = parent;
            Tag = elements;
			Min = elements.Min - TreeBuildingSettings.CenterDataSet;
			Max = elements.Max - TreeBuildingSettings.CenterDataSet;

            if (elements.FinalElement || elements.TriangleCount < TreeBuildingSettings.MaxTriangleCount)
            {
                //Leaf
	            Error = 0;
                NodeData data = elements.CreateData(TreeBuildingSettings.CenterDataSet, textureInfo);
                using (FileStream file = File.Open(NodeDataFile, FileMode.Create, FileAccess.ReadWrite))
                {
                    data.SaveToStream(file);
                }
            }
            else
            {
                //Not a leaf
                ElementList[] split = elements.SplitList();
                for (int i = 0; i < 4; i++)
                {
                    Node child = new Node(split[i], this, textureInfo);
                    Children.Add(child);
                }
                NodeData data = elements.CreateDataFromChildren(Children, TreeBuildingSettings.CenterDataSet,
                                                                textureInfo, out _error);
                using (FileStream file = File.Open(NodeDataFile, FileMode.Create, FileAccess.ReadWrite))
                {
                    data.SaveToStream(file);
                }
            }
        }

		public Node(FileElementList elements, Node parent, TextureInfo textureInfo, int depth, out FileElementList usedList, out int height)
		{
			Children = new List<Node>();
			NodeDataFile = FilenameGenerator.CreateFilename();
			Parent = parent;
			Tag = elements;
			Min = elements.Min - TreeBuildingSettings.CenterDataSet;
			Max = elements.Max - TreeBuildingSettings.CenterDataSet;
			height = 0;

			if (elements.FinalElement || elements.TriangleCount < TreeBuildingSettings.MaxTriangleCount)
			{
				//Leaf
				Error = 0;
				NodeData data = elements.CreateData(TreeBuildingSettings.CenterDataSet, textureInfo);
				using (FileStream file = File.Open(NodeDataFile, FileMode.Create, FileAccess.ReadWrite))
				{
					data.SaveToStream(file);
				}
				usedList = elements;
				
			}
			else
			{
				//Not a leaf
				FileElementList[] split = elements.SplitList(FilenameGenerator.CreateTempFilenames(4));
				FileElementList[] usedVersion = new FileElementList[4];
				int[] heights = new int[4];
				for (int i = 0; i < split.Length; i++)
				{
					Node child = new Node(split[i], this, textureInfo, depth+1, out usedVersion[i], out heights[i]);
					Children.Add(child);
					height = Math.Max(heights[i]+1, height);
				}
				Simplification simplification = new Simplification();
				FileElementList optimizedVersion = simplification.CreateDataFromChildren(new List<FileElementList>(usedVersion), new List<int>(heights), depth, out _error);
				NodeData data = optimizedVersion.CreateData(TreeBuildingSettings.CenterDataSet, textureInfo);
				using (FileStream file = File.Open(NodeDataFile, FileMode.Create, FileAccess.ReadWrite))
				{
					data.SaveToStream(file);
				}
				usedList = optimizedVersion;
			}
		}



        public List<Node> Children { get; set; }
        public Node Parent { get; set; }
        public string NodeDataFile { get; set; }
        public object Tag { get; set; }
	    public float Error
	    {
		    get { return _error; }
		    set { _error = value; }
	    }

	    public HyperPoint<float> Min
	    {
		    get { return _min; }
		    set { _min = value; }
	    }

		public HyperPoint<float> Max
	    {
		    get { return _max; }
		    set { _max = value; }
	    }

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
