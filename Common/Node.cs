using System;
using System.Collections.Generic;
using System.IO;

namespace CG_2IV05.Common
{
	[Serializable]
    public class Node
    {
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
