using System.Collections.Generic;
using System.IO;

namespace CG_2IV05.Common
{
    public class Node
    {
	    public List<Node> Children { get; set; }
	    public Node Parent { get; set; }
	    public string NodeDataFile { get; set; }

		public NodeData ReadData()
		{
			using (FileStream file = File.Open(NodeDataFile, FileMode.Open, FileAccess.Read))
			{
				return Common.NodeData.ReadFromStream(file);
			}
		}
    }
}
