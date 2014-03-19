using System;
using System.Collections.Generic;
using System.IO;
using micfort.GHL.Math2;

namespace CG_2IV05.Common.Element
{
	public interface IElement
	{
		bool FinalElement { get; }

		int TriangleCount { get; }

        ScoreKey Score { get; }

		HyperPoint<float> Min { get; }
		HyperPoint<float> Max { get; }

		HyperPoint<float> ReferencePoint { get; }
		NodeData CreateData(HyperPoint<float> centerDataSet, TextureInfo textureInfo);
        IElement GetSimplifiedVersion(HyperPoint<float> centerDataSet, TextureInfo textureInfo);
	}

	public interface IFinalElement : IElement
	{
		void SaveToStream(Stream stream);
		int FactoryID { get; }
	}

	public interface IListElement : IElement, IEnumerable<IFinalElement>
	{
	}

    public class ScoreKey : IComparable<ScoreKey>
    {
        private static int nextUid = 0;
        public readonly int UID;
        public float Score { get; set; }

        public ScoreKey(float score)
        {
            if (score == float.MaxValue)
                UID = 0;
            else 
                UID = nextUid++;
            Score = score;
        }

        public int CompareTo(ScoreKey other)
        {
            if (Score > other.Score)
            {
                return 1;
            }
            if (Score < other.Score)
            {
                return -1;
            }

            return UID > other.UID ? 1 : -1;
        }
    }
}