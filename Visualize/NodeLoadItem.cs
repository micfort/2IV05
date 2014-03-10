using CG_2IV05.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CG_2IV05.Visualize
{
    struct NodeLoadItem
    {
        VBO vbo;
        Node loadItem;

        public NodeLoadItem(VBO vbo, Node node)
        {
            loadItem = node;
            this.vbo = vbo;
        }

        public VBO loadNodeFromDisc()
        {
            vbo.LoadData(loadItem.ReadRawData());
            return vbo;
        }

    }
}
