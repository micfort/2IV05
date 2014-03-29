using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CG_2IV05.Common.Element
{
	public interface IElementFactory
	{
		IElement ReadFromStream(Stream stream);
		int FactoryID { get; }
		IElement Merge(List<IElement> elements);
		bool CanMerge(List<IElement> elements);
		void RemoveDetail(List<IElement> elements, int height);
	}
}
