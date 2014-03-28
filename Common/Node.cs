﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
				using (FileStream file = File.Open(FilenameGenerator.GetOutputPathToFile(NodeDataFile), FileMode.Create, FileAccess.ReadWrite))
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
				using (FileStream file = File.Open(FilenameGenerator.GetOutputPathToFile(NodeDataFile), FileMode.Create, FileAccess.ReadWrite))
                {
                    data.SaveToStream(file);
                }
            }
        }

		/// <summary>
		/// create root node of a tree with elements
		/// </summary>
		/// <param name="elements"></param>
		/// <param name="parent"></param>
		/// <param name="textureInfo"></param>
		public Node(FileElementList elements, Node parent, TextureInfo textureInfo)
		{
			FileElementList usedList;//dummy variable
			int height;//dummy variable
			ConstructorFileElementList(elements, parent, textureInfo, 0, out usedList, out height);
		}

		private Node(FileElementList elements, Node parent, TextureInfo textureInfo, int depth, out FileElementList usedList, out int height)
		{
			ConstructorFileElementList(elements, parent, textureInfo, depth, out usedList, out height);
		}

		private void ConstructorFileElementList(FileElementList elements, Node parent, TextureInfo textureInfo, int depth, out FileElementList usedList, out int height)
		{
			micfort.GHL.Logging.ErrorReporting.Instance.ReportInfoT(LoggingTag.CurrentContext,
			                                                        string.Format("Creating node on depth {0} with file {1}",
			                                                                      depth, Path.GetFileName(elements.Filename)));
			Children = new List<Node>();
			NodeDataFile = FilenameGenerator.CreateFilename();
			Parent = parent;
			Tag = elements;
			Min = new HyperPoint<float>(elements.Min - TreeBuildingSettings.CenterDataSet, 0);
			Max = new HyperPoint<float>(elements.Max - TreeBuildingSettings.CenterDataSet, 0);
			height = 0;

			if (elements.FinalElement || elements.TriangleCount < TreeBuildingSettings.MaxTriangleCount)
			{
				//Leaf
				Error = 0;
				NodeData data = elements.CreateData(new HyperPoint<float>(TreeBuildingSettings.CenterDataSet, 0), textureInfo);
				using (FileStream file = File.Open(FilenameGenerator.GetOutputPathToFile(NodeDataFile), FileMode.Create, FileAccess.ReadWrite))
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
				CreateSubNode(split, usedVersion, textureInfo, depth, out height);
				Simplification2 simplification = new Simplification2();
				FileElementList optimizedVersion = simplification.CreateDataFromChildren(new List<FileElementList>(usedVersion), new List<int>(heights), depth, out _error);
				NodeData data = optimizedVersion.CreateData(new HyperPoint<float>(TreeBuildingSettings.CenterDataSet, 0), textureInfo);
				using (FileStream file = File.Open(FilenameGenerator.GetOutputPathToFile(NodeDataFile), FileMode.Create, FileAccess.ReadWrite))
				{
					data.SaveToStream(file);
				}
				usedList = optimizedVersion;
			}
		}

		private void CreateSubNode(FileElementList[] split, FileElementList[] usedVersion, TextureInfo textureInfo, int depth, out int height)
		{
			height = 0;
			int[] heights = new int[split.Length];
			if (depth < TreeBuildingSettings.CreateThreadDepth)
			{
				ManualResetEvent[] mres = new ManualResetEvent[split.Length];
				Node[] childs = new Node[split.Length];
				for (int i = 0; i < split.Length; i++)
				{
					string currentLoggingTag = LoggingTag.CurrentContext;
					mres[i] = new ManualResetEvent(false);
					Thread t = new Thread((o) =>
						                      {
												
							                      int index = (int) o;
												  LoggingTag.Push(currentLoggingTag + "_" + index.ToString());
												  childs[index] = new Node(split[index], this, textureInfo, depth + 1, out usedVersion[index], out heights[index]);
							                      LoggingTag.Pop();
												  mres[index].Set();
						                      });
					t.Start(i);
				}
				ManualResetEvent.WaitAll(mres);
				for (int i = 0; i < split.Length; i++)
				{
					Children.Add(childs[i]);
					height = Math.Max(heights[i] + 1, height);	
				}
			}
			else
			{
				for (int i = 0; i < split.Length; i++)
				{
					LoggingTag.Push(LoggingTag.CurrentContext + "_" + i.ToString());
					Node child = new Node(split[i], this, textureInfo, depth + 1, out usedVersion[i], out heights[i]);
					LoggingTag.Pop();
					Children.Add(child);
					height = Math.Max(heights[i] + 1, height);
				}
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
			using (FileStream file = File.Open(FilenameGenerator.GetOutputPathToFile(NodeDataFile), FileMode.Open, FileAccess.Read))
            {
                return NodeData.ReadFromStream(file);
            }
        }

        public NodeDataRaw ReadRawData()
        {
			using (FileStream file = File.Open(FilenameGenerator.GetOutputPathToFile(NodeDataFile), FileMode.Open, FileAccess.Read))
            {
                return NodeDataRaw.ReadFromStream(file);
            }
        }
    }
}
