using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using micfort.GHL.Serialization;

namespace CG_2IV05.Common
{
	public class Tree: SerializableType<Tree>
	{
		public Node Root { get; set; }
	}
}
