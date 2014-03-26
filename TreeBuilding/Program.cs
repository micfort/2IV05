using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using CG_2IV05.Common;
using CG_2IV05.Common.BAG;
using CG_2IV05.Common.Element;
using CG_2IV05.Common.OSM;
using micfort.GHL.Math2;
using micfort.GHL.Serialization;

namespace CG_2IV05.TreeBuilding
{
    class Program
    {
		private static TextureInfo textureInfo = new TextureInfo();

        static void Main(string[] args)
        {
            micfort.GHL.GHLWindowsInit.Init();

            ParseCommandLine(args);

			if (!Directory.Exists(TreeBuildingSettings.DirectoryOutput))
            {
				Directory.CreateDirectory(TreeBuildingSettings.DirectoryOutput);
            }

			if (!Directory.Exists(TreeBuildingSettings.TmpDirectory))
			{
				Directory.CreateDirectory(TreeBuildingSettings.TmpDirectory);
			}

            FileElementList list = new FileElementList(TreeBuildingSettings.InputFilename);

			SetCenterDateSet(list);

            Console.Out.WriteLine("Create Nodes");
			Node root = new Node(list, null, textureInfo);
            CleanTag(root);

            Tree tree = new Tree();
            tree.Root = root;
            
			Console.Out.WriteLine("Writing Tree");
			using (FileStream file = File.Open(string.Format(TreeBuildingSettings.TreeOutputFileFormat, TreeBuildingSettings.DirectoryOutput), FileMode.Create, FileAccess.ReadWrite))
            {
                SerializableType<Tree>.SerializeToStream(tree, file, BinarySerializableTypeEngine.BinairSerializer);
            }

			if (Directory.Exists(TreeBuildingSettings.TmpDirectory))
			{
				Directory.Delete(TreeBuildingSettings.TmpDirectory, true);
			}

	        Console.Out.WriteLine("Done. Press any key to close.");
	        Console.ReadKey();

        }

        public static void CleanTag(Node node)
        {
            node.Tag = null;
            node.Children.ForEach(CleanTag);
        }

		public static void SetCenterDateSet(FileElementList list)
        {
			if (TreeBuildingSettings.FindCenterDataSet)
			{
				HyperPoint<float> min = list.Min;
				HyperPoint<float> max = list.Max;

				TreeBuildingSettings.CenterDataSet = (max - min);
				TreeBuildingSettings.CenterDataSet = TreeBuildingSettings.CenterDataSet * (1f / 2f);
				TreeBuildingSettings.CenterDataSet = TreeBuildingSettings.CenterDataSet + min;
            }
            else
            {
				TreeBuildingSettings.CenterDataSet = new HyperPoint<float>(0, 0);
            }
        }

        public static void ParseCommandLine(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--triangles" || args[i] == "-t")
                {
                    i++;
					TreeBuildingSettings.MaxTriangleCount = int.Parse(args[i]);
                }
                else if (args[i] == "--max-triangles" || args[i] == "-m")
                {
					TreeBuildingSettings.MaxTriangleCount = int.MaxValue;
                }
                else if (args[i] == "--output" || args[i] == "-o")
                {
                    i++;
					TreeBuildingSettings.DirectoryOutput = args[i];
                }
                else if (args[i] == "--input" || args[i] == "-i")
                {
                    i++;
					TreeBuildingSettings.InputFilename = args[i];
                }
                else if (args[i] == "--generate-data" || args[i] == "-g")
                {
					TreeBuildingSettings.Generate = true;
                    i++;
					TreeBuildingSettings.generateSizeX = int.Parse(args[i]);
                    i++;
					TreeBuildingSettings.generateSizeY = int.Parse(args[i]);
                }
                else if (args[i] == "--center-data" || args[i] == "-c")
                {
					TreeBuildingSettings.FindCenterDataSet = true;
                }
				else if(args[i] == "--min-depth" || args[i] == "-d")
	            {
					i++;
					TreeBuildingSettings.MinCurrentDepthForData = int.Parse(args[i]);
	            }
            }
        }
    }
}
