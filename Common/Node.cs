﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CG_2IV05.Common.Element;
using micfort.GHL.Math2;
using micfort.GHL.Logging;

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
			ConstructorFileElementList(elements, parent, textureInfo, SplitDirection.Vertical, 0, out usedList, out height);
		}

		private Node(FileElementList elements, Node parent, TextureInfo textureInfo, SplitDirection split, int depth, out FileElementList usedList, out int height)
		{
			ConstructorFileElementList(elements, parent, textureInfo, split, depth, out usedList, out height);
		}

		private void ConstructorFileElementList(FileElementList elements, Node parent, TextureInfo textureInfo, SplitDirection splitDirection, int depth, out FileElementList usedList, out int height)
		{

			Children = new List<Node>();
			NodeDataFile = CreateNodeDataFilename();
			Parent = parent;
			Tag = elements;
			Min = new HyperPoint<float>(elements.Min - TreeBuildingSettings.CenterDataSet, 0);
			Max = new HyperPoint<float>(elements.Max - TreeBuildingSettings.CenterDataSet, 0);

			if (elements.FinalElement || elements.TriangleCount < TreeBuildingSettings.MaxTriangleCount)
			{
				//Leaf
				Error = 0;
				if (NeedNodeData(elements.Filename, FilenameGenerator.GetOutputPathToFile(NodeDataFile)))
				{
					RemoveFileIfExist(FilenameGenerator.GetOutputPathToFile(NodeDataFile));
					string tmpfilename = FilenameGenerator.CreateTempFilename();
					NodeData data = elements.CreateData(new HyperPoint<float>(TreeBuildingSettings.CenterDataSet, 0), textureInfo); //create data
					using (FileStream file = File.Open(tmpfilename, FileMode.Create, FileAccess.ReadWrite))
					{
						data.SaveToStream(file);
					}
					File.Move(tmpfilename, FilenameGenerator.GetOutputPathToFile(NodeDataFile));
				}
				usedList = elements;
				height = 0;
			}
			else
			{
				//Not a leaf

				//split information
				int splitCount = 2;
				SplitDirection nextSplitDirection = splitDirection == SplitDirection.Horizontal
					                                    ? SplitDirection.Vertical
					                                    : SplitDirection.Horizontal;

				//split the data
				List<string> splitFilenames = CreateSplitFilenames(splitCount);
				List<FileElementList> split;
				ErrorReporting.Instance.ReportInfoT(LoggingTag.CurrentContext, string.Format("Split data"));
				//check if neccesary
				if(NeedSplit(splitFilenames, elements.Filename))
				{
					List<string> tmpFilenames = new List<string>(FilenameGenerator.CreateTempFilenames(splitCount));
					if(splitCount == 2)
					{
						elements.SplitListBinary(tmpFilenames.ToArray(), splitDirection);
					}
					else
					{
						elements.SplitList(tmpFilenames.ToArray());	
					}
					
					for (int i = 0; i < tmpFilenames.Count; i++)
					{
						RemoveFileIfExist(splitFilenames[i]);
						File.Move(tmpFilenames[i], splitFilenames[i]);
					}
				}
				//create the readers
				split = splitFilenames.ConvertAll(x => new FileElementList(x));

				//create subnodes
				FileElementList[] usedVersion = new FileElementList[splitCount];
				int[] heights;
				CreateSubNode(split.ToArray(), usedVersion, textureInfo, nextSplitDirection, depth, out heights);

				//simplify
				ErrorReporting.Instance.ReportInfoT(LoggingTag.CurrentContext, string.Format("Simplify data"));
				string simpFilename = CreateSimplifyFilename();
				FileElementList optimizedVersion;
				Simplification2 simplification = new Simplification2();
				if(NeedSimplify(new List<FileElementList>(usedVersion).ConvertAll(x=> x.Filename), simpFilename)) //check if needed
				{
					RemoveFileIfExist(simpFilename);
					string tmpfilename = FilenameGenerator.CreateTempFilename();
					simplification.CreateDataFromChildren(new List<FileElementList>(usedVersion), tmpfilename, new List<int>(heights), depth); //simplify
					File.Move(tmpfilename, simpFilename);
				}
				optimizedVersion = new FileElementList(simpFilename);//open file element list
				Error = simplification.DetermineError(elements, optimizedVersion, depth); //determine error afterwards

				ErrorReporting.Instance.ReportInfoT(LoggingTag.CurrentContext, string.Format("Create output data"));
				if (NeedNodeData(optimizedVersion.Filename, FilenameGenerator.GetOutputPathToFile(NodeDataFile)))
				{
					RemoveFileIfExist(FilenameGenerator.GetOutputPathToFile(NodeDataFile));
					string tmpfilename = FilenameGenerator.CreateTempFilename();
					NodeData data = optimizedVersion.CreateData(new HyperPoint<float>(TreeBuildingSettings.CenterDataSet, 0), textureInfo); //create data
					using (FileStream file = File.Open(tmpfilename, FileMode.Create, FileAccess.ReadWrite))
					{
						data.SaveToStream(file);
					}
					File.Move(tmpfilename, FilenameGenerator.GetOutputPathToFile(NodeDataFile));
				}
				usedList = optimizedVersion;
				height = heights.Max() + 1;
			}
		}

		private List<string> CreateSplitFilenames(int i)
		{
			List<string> output = new List<string>(i);
			for (int j = 0; j < i; j++)
			{
				output.Add(FilenameGenerator.GetWorkingFilename("split_" + LoggingTag.CurrentContext + "_" + j.ToString()));
			}
			return output;
		}

		private string CreateSimplifyFilename()
		{
			return FilenameGenerator.GetWorkingFilename("simp_" + LoggingTag.CurrentContext);
		}

		private string CreateNodeDataFilename()
		{
			return "data_" + LoggingTag.CurrentContext;
		}

		private bool NeedSplit(List<string> outputFilenames, string inputFilename)
		{
			DateTime inputDate = FileElementList.GetDate(inputFilename);
			return outputFilenames.Any(x => FileElementList.GetDate(x) <= inputDate);
		}

		private bool NeedSimplify(List<string> inputFilenames, string outputFilename)
		{
			DateTime outputDate = FileElementList.GetDate(outputFilename);
			return inputFilenames.Any(x => outputDate <= FileElementList.GetDate(x));
		}

		private bool NeedNodeData(string inputFilenames, string outputFilename)
		{
			return FileElementList.GetDate(outputFilename) <= FileElementList.GetDate(inputFilenames);
		}

		private void RemoveFileIfExist(string filename)
		{
			if(File.Exists(filename))
				File.Delete(filename);
		}

		private void CreateSubNode(FileElementList[] split, FileElementList[] usedVersion, TextureInfo textureInfo, SplitDirection splitDirection, int depth, out int[] heights)
		{
			heights = new int[split.Length];
			if (depth < TreeBuildingSettings.CreateThreadDepth)
			{
				int[] outHeights = new int[split.Length];
				ManualResetEvent[] mres = new ManualResetEvent[split.Length];
				Node[] childs = new Node[split.Length];
				for (int i = 0; i < split.Length; i++)
				{
					string currentLoggingTag = LoggingTag.CurrentContext;
					mres[i] = new ManualResetEvent(false);
					Thread t = new Thread((o) =>
						                      {
												
							                      int index = (int) o;
												  LoggingTag.Push(currentLoggingTag + index.ToString());
												  childs[index] = new Node(split[index], this, textureInfo, splitDirection, depth + 1, out usedVersion[index], out outHeights[index]);
							                      LoggingTag.Pop();
												  mres[index].Set();
						                      });
					t.Start(i);
				}
				ManualResetEvent.WaitAll(mres);
				for (int i = 0; i < split.Length; i++)
				{
					Children.Add(childs[i]);
					heights[i] = outHeights[i];
				}
			}
			else
			{
				for (int i = 0; i < split.Length; i++)
				{
					LoggingTag.Push(LoggingTag.CurrentContext + i.ToString());
					Node child = new Node(split[i], this, textureInfo, splitDirection, depth + 1, out usedVersion[i], out heights[i]);
					LoggingTag.Pop();
					Children.Add(child);
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

		public float DistanceToNode(HyperPoint<float> position)
		{
			return DistanceToSquare(Min, Max, position);
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
