using System;
using System.IO;
using System.Linq;
using CG_2IV05.Common.Element;
using CG_2IV05.Common.OSM;
using SharpCompress.Common;
using SharpCompress.Reader;

namespace CG_2IV05.PreProcess2
{
	class Program
	{
		static string outputFilename = "output";
		private static string pbfFilename = string.Empty;
		private static string BagFilename = string.Empty;
		private static string Bin1Filename = string.Empty;
		private static string Bin2Filename = string.Empty;
		static void Main(string[] args)
		{
			micfort.GHL.GHLWindowsInit.Init();

			ParseCommandLine(args);

			using (FileElementListWriter writer = new FileElementListWriter(outputFilename))
			{
				if (BagFilename != string.Empty && File.Exists(BagFilename))
				{
					Console.Out.WriteLine("Reading Bag file: {0}", BagFilename);
					using (Stream stream = File.OpenRead(BagFilename))
					{
						var reader = ReaderFactory.Open(stream);
						while (reader.MoveToNextEntry())
						{
							Console.Out.WriteLine("File: {0}", reader.Entry.FilePath);
							using (Stream s = reader.OpenEntryStream())
							{
								BAGXML.ReadBuildings(s, building => writer.WriteElement(building));
							}
						}
					}
				}
				if (pbfFilename != string.Empty && File.Exists(pbfFilename))
				{
					Console.Out.WriteLine("Reading pbf file: {0}", pbfFilename);
					using (FileStream file = File.OpenRead(pbfFilename))
					{
						OSM.Read(file, element => writer.WriteElement(element));
					}
				}
				if (Bin1Filename != string.Empty && File.Exists(Bin1Filename))
				{
					Console.Out.WriteLine("Reading binair file 1: {0}", Bin1Filename);
					FileElementList list = new FileElementList(Bin1Filename);
					int i = 0;
					foreach (IElement element in list)
					{
						if (i % 100000 == 0)
						{
							Console.Out.WriteLine("Processing element {0:N0}", i);
						}
						writer.WriteElement(element);
						i++;
					}
				}
				if (Bin2Filename != string.Empty && File.Exists(Bin2Filename))
				{
					Console.Out.WriteLine("Reading binair file 2: {0}", Bin2Filename);
					FileElementList list = new FileElementList(Bin2Filename);
					int i = 0;
					foreach (IElement element in list)
					{
						if (i % 100000 == 0)
						{
							Console.Out.WriteLine("Processing element {0:N0}", i);
						}
						writer.WriteElement(element);
						i++;
					}
				}
			}
			{
				FileElementList list = new FileElementList(outputFilename);
				int count = 0;
				foreach (IElement element in list)
				{
					if (count%10000 == 0)
					{
						Console.Out.WriteLine("Processing element {0:N0}", count);
					}
					count++;
				}
			}
			Console.ReadKey();
		}

		public static void ParseCommandLine(string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "--output" || args[i] == "-o")
				{
					i++;
					outputFilename = args[i];
				}
				else if (args[i] == "--OSM" || args[i] == "--PBF")
				{
					i++;
					pbfFilename = args[i];
				}
				else if (args[i] == "--BAG")
				{
					i++;
					BagFilename = args[i];
				}
				else if (args[i] == "--BIN1")
				{
					i++;
					Bin1Filename = args[i];
				}
				else if (args[i] == "--BIN2")
				{
					i++;
					Bin2Filename = args[i];
				}
			}
		}
	}
}
