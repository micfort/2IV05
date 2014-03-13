using System;
using System.Collections.Generic;
using System.IO;
using CG_2IV05.Common.Element;
using micfort.GHL.Math2;

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
            if (parent == null)
            {
                min = elements.Min;
                max = elements.Max;
            }

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
                    calcMinMaxChildNode(child, i);
                    Children.Add(child);
                }

                NodeData data = elements.CreateDataFromChildren(Children, TreeBuildingSettings.CenterDataSet,
                                                                textureInfo);
                using (FileStream file = File.Open(NodeDataFile, FileMode.Create, FileAccess.ReadWrite))
                {
                    data.SaveToStream(file);
                }
            }
        }

        private void calcMinMaxChildNode(Node child, int i)
        {
            if (i == 0)
            {
                child.min = min;
                child.max = min + ((max - min) / 2);
            }
            else if (i == 1)
            {
                child.min.Y = min.Y;
                child.min.X = min.X + ((max.X - min.X) / 2);
                child.max.X = max.X;
                child.max.Y = min.Y + ((max.Y - min.Y) / 2);
            }
            else if (i == 2)
            {
                child.min.X = min.X;
                child.min.Y = min.Y + ((max.Y - min.Y) / 2);
                child.max.Y = max.Y;
                child.max.X = min.X + ((max.X - min.X) / 2);
            }
            else if (i == 3)
            {
                child.min = min + ((max - min) / 2);
                child.max = max;
            }
        }

        public List<Node> Children { get; set; }
        public Node Parent { get; set; }
        public string NodeDataFile { get; set; }
        public object Tag { get; set; }
        public HyperPoint<float> min;
        public HyperPoint<float> max;

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

        public Node findNodeAtDepth(HyperPoint<float> point, int depth)
        {
            if (depth == 0 || Children.Count == 0)
                return this;

            HyperPoint<float> center = (min + max)/2;
            if (point.Y < center.Y)
            {
                if (point.X < center.X)
                {
                    return Children[0].findNodeAtDepth(point, depth - 1);
                }
                else
                {
                    return Children[1].findNodeAtDepth(point, depth - 1);
                }
            }
            else
            {
                if (point.X < center.X)
                {
                    return Children[2].findNodeAtDepth(point, depth - 1);
                }
                else
                {
                    return Children[3].findNodeAtDepth(point, depth - 1);
                }
            }
        }

        public Node findSiblingWithPoint(HyperPoint<float> point, int depth)
        {
            if (!isPointInNode(point))
            {
                return Parent.findSiblingWithPoint(point, depth + 1);
            }
            return findNodeAtDepth(point, depth);
        }

        public bool isPointInNode(HyperPoint<float> point)
        {
            return (point.X < max.X && point.Y < max.Y) && (point.X > min.X && point.Y > min.Y);
        }
    }
}
