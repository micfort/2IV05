using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;

namespace CG_2IV05.Common
{
	class TextureInfo
	{
		private HyperPoint<int> ItemSize = new HyperPoint<int>(64, 64);
		private HyperPoint<int> TextureSize = new HyperPoint<int>(512, 512);

		private HyperPoint<float> GetItem(int i)
		{
			int ItemsPerRow = TextureSize.X/ItemSize.X;
			return new HyperPoint<float>((i%ItemsPerRow)*TextureSize.X, (i/ItemsPerRow)*ItemSize.Y);
		}

		public HyperPoint<float> Road
		{
			get { return GetItem(0); }
		}
		public HyperPoint<float> Grass
		{
			get { return GetItem(1); }
		}
		public HyperPoint<float> Water
		{
			get { return GetItem(2); }
		}
		public HyperPoint<float> Building
		{
			get { return GetItem(3); }
		}
		public HyperPoint<float> Forest
		{
			get { return GetItem(4); }
		}

		public HyperPoint<float> GetLeftTop(HyperPoint<float> item)
		{
			return item;
		}

		public HyperPoint<float> GetLeftBottom(HyperPoint<float> item)
		{
			return new HyperPoint<float>(item.X, item.Y + ItemSize.Y);
		} 

		public HyperPoint<float> GetRightTop(HyperPoint<float> item)
		{
			return new HyperPoint<float>(item.X + ItemSize.X, item.Y);
		}

		public HyperPoint<float> GetRightBottom(HyperPoint<float> item)
		{
			return new HyperPoint<float>(item.X + ItemSize.X, item.Y + ItemSize.Y);
		}
	}
}
