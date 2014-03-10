using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using micfort.GHL.Math2;

namespace CG_2IV05.Common
{
	public class TextureInfo
	{
		private HyperPoint<int> ItemCount = new HyperPoint<int>(2048 / 256, 2048 / 256);
		private HyperPoint<float> ItemSize = new HyperPoint<float>(256f / 2048f, 256f / 2048f);
		private HyperPoint<float> TextureSize = new HyperPoint<float>(1f, 1f);

		private HyperPoint<float> GetItem(int i)
		{
			return new HyperPoint<float>((i % ItemCount.X) * ItemSize.X, (i / ItemCount.X) * ItemSize.Y);
		}

		#region Texture elements

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

		public List<HyperPoint<float>> Buildings
		{
			get
			{
				return new List<HyperPoint<float>>()
					       {
						       GetItem(3),
						       GetItem(0),
						       GetItem(1),
						       GetItem(2),
					       };
			}
		}

		public List<HyperPoint<float>> Roof
		{
			get
			{
				return new List<HyperPoint<float>>()
					       {
						       GetItem(4),
						       GetItem(4),
						       GetItem(4),
						       GetItem(4),
					       };
			}
		}

		public HyperPoint<float> Forest
		{
			get { return GetItem(4); }
		}

		#endregion


		#region Helper methods

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

		#endregion

	}
}
