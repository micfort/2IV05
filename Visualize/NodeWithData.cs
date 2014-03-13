using CG_2IV05.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CG_2IV05.Visualize
{
    public struct NodeWithData
    {
        public VBO vbo;
        public Node node;

        public NodeWithData(VBO vbo, Node node)
        {
            this.node = node;
            this.vbo = vbo;
        }

        public void loadNodeFromDisc()
        {
			if(vbo == null)
			{
				vbo = new VBO();
			}
            vbo.LoadData(node.ReadRawData());
        }

    }
}
