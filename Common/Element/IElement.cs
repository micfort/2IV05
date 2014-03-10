using micfort.GHL.Math2;

namespace CG_2IV05.Common.Element
{
	public interface IElement
	{
		bool FinalElement { get; }

		int TriangleCount { get; }

		HyperPoint<float> Min { get; }
		HyperPoint<float> Max { get; }

		HyperPoint<float> ReferencePoint { get; }
		NodeData CreateData(HyperPoint<float> centerDataSet, TextureInfo textureInfo);
	}
}