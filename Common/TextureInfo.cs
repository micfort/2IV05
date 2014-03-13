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

		private const string localKey = "2IV05";
		private static Tuple<string, string> UnknownKey = CreateKey("unknown");
		private static Tuple<string, string> CreateKey(string name)
		{
			return new Tuple<string, string>(localKey, name);
		}

		private static Tuple<string, string> CreateKey(string tag, string name)
		{
			return new Tuple<string, string>(tag, name);
		} 

		private Dictionary<Tuple<string, string>, int> textureInfos = new Dictionary<Tuple<string, string>, int>()
			                                               {	
																//unknown
				                                               {UnknownKey, 0},
															   {CreateKey("highway", "construction"), 0},
															   {CreateKey("highway", "unsurfaced"), 0},
															   //asfalt
				                                               {CreateKey("highway", "motorway"), 1},
															   {CreateKey("highway", "trunk"), 1},
															   {CreateKey("highway", "primary"), 1},
															   {CreateKey("highway", "secondary"), 1},
															   {CreateKey("highway", "tertiary"), 1},
															   {CreateKey("highway", "unclassified"), 1},
															   {CreateKey("highway", "residential"), 1},
															   {CreateKey("highway", "service"), 1},
															   {CreateKey("highway", "motorway_link"), 1},
															   {CreateKey("highway", "trunk_link"), 1},
															   {CreateKey("highway", "primary_link"), 1},
															   {CreateKey("highway", "secondary_link"), 1},
															   {CreateKey("highway", "tertiary_link"), 1},
															   {CreateKey("highway", "living_street"), 1},

															   //grass
				                                               {CreateKey("landuse", "meadow"), 2},
															   {CreateKey("landuse", "grass"), 2},
															   {CreateKey("landuse", "pasture"), 2},
															   {CreateKey("landcover", "grass"), 2},
															   //water
															   {CreateKey("water"), 3},
															   {CreateKey("natural", "water"), 3},
															   //bricks
															   {CreateKey("bricks"), 4},
															   //trees
															   {CreateKey("trees"), 5},
															   //cycle way
															   {CreateKey("highway", "cycleway"), 6},
															   //roof
															   {CreateKey("roof"), 7},
															   //looppad
															   {CreateKey("highway", "footway"), 8},
															   {CreateKey("highway", "pedestrian"), 8},
															   //track
															   {CreateKey("highway", "track"), 9},
															   {CreateKey("highway", "path"), 9},
															   {CreateKey("highway", "bridleway"), 9},
															   //steps
															   {CreateKey("highway", "steps"), 10},

			                                               };

		private HyperPoint<float> GetItem(int i)
		{
			return new HyperPoint<float>((i % ItemCount.X) * ItemSize.X, (i / ItemCount.X) * ItemSize.Y);
		}

		public HyperPoint<float> GetTexture(string name)
		{
			if (textureInfos.ContainsKey(CreateKey(name)))
			{
				return GetItem(textureInfos[CreateKey(name)]);
			}
			else
			{
				return GetItem(textureInfos[UnknownKey]);
			}
		}

		public HyperPoint<float> GetTexture(string tag, string name)
		{
			if (textureInfos.ContainsKey(CreateKey(tag, name)))
			{
				return GetItem(textureInfos[CreateKey(tag, name)]);
			}
			else
			{
				return GetItem(textureInfos[UnknownKey]);
			}
		}

		#region Texture elements

		public HyperPoint<float> Road
		{
			get { return GetTexture("highway", "motorway"); }
		}

		public HyperPoint<float> Grass
		{
			get { return GetTexture("landcover", "grass"); }
		}

		public HyperPoint<float> Water
		{
			get { return GetTexture("water"); }
		}

		public List<HyperPoint<float>> Buildings
		{
			get
			{
				return new List<HyperPoint<float>>()
					       {
						       GetTexture("bricks")
					       };
			}
		}

		public List<HyperPoint<float>> Roof
		{
			get
			{
				return new List<HyperPoint<float>>()
					       {
						       GetTexture("roof")
					       };
			}
		}

		public HyperPoint<float> Forest
		{
			get { return GetTexture("trees"); }
		}

		public HyperPoint<float> Unknown
		{
			get { return GetItem(textureInfos[UnknownKey]); }
		} 

		#endregion

		#region Helper methods

		public HyperPoint<float> GetLeftTop(HyperPoint<float> item)
		{
			return item;
		}

		public HyperPoint<float> GetLeftBottom(HyperPoint<float> item)
		{
			return GetPoint(item, new HyperPoint<float>(0, 1));
		}

		public HyperPoint<float> GetRightTop(HyperPoint<float> item)
		{
			return GetPoint(item, new HyperPoint<float>(1, 0));
		}

		public HyperPoint<float> GetRightBottom(HyperPoint<float> item)
		{
			return GetPoint(item, new HyperPoint<float>(1, 1));
		}

		/// <summary>
		/// gets a coordinate within the texture
		/// </summary>
		/// <param name="item">The texture coordinate</param>
		/// <param name="position">The position [0..1, 0..1] within the texture</param>
		/// <returns>The texture coordinates</returns>
		public HyperPoint<float> GetPoint(HyperPoint<float> item, HyperPoint<float> position)
		{
			return item + new HyperPoint<float>(ItemSize.X*position.X, ItemSize.Y*position.Y);
		}

		#endregion

	}
}
