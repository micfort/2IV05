using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG_2IV05.Common;

namespace TreeViewer
{
	class NodeWithData: INodeWithData
	{
		public NodeWithData()
		{
			
		}

		#region Implementation of INodeWithData

		public Node Node { get; set; }
		public void LoadNodeFromDisc()
		{
		}

		#endregion
	}
}
