using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using micfort.GHL.Math2;

namespace CG_2IV05.Common.Element
{
	public class FileElementListWriter:IDisposable
	{
		private readonly FileStream _stream;

		private HyperPoint<float> _min = new HyperPoint<float>(float.PositiveInfinity, float.PositiveInfinity);
		private HyperPoint<float> _max = new HyperPoint<float>(float.NegativeInfinity, float.NegativeInfinity);
		private int _triangleCount = 0;

		public FileElementListWriter(string filename, bool append = false)
		{
			if(append && File.Exists(filename))
			{
				_stream = new FileStream(filename, FileMode.Append, FileSystemRights.Modify, FileShare.None, 1024*1024,
				                         FileOptions.None);
				ReadMetaData();
				_stream.Position = _stream.Length;
			}
			else
			{
				_stream = new FileStream(filename, FileMode.Create, FileSystemRights.Modify, FileShare.None, 1024*1024,
				                         FileOptions.None);
				WriteMetadata();
			}
			
		}

		public static void CreateEmptyFile(string filename)
		{
			FileElementListWriter writer = new FileElementListWriter(filename);
			writer.Dispose();
		}

		private void WriteMetadata()
		{
			_stream.Position = 0;
			BinaryToStream.WriteToStream(_triangleCount, _stream);
			BinaryToStream.WriteToStream(_min.X, _stream);
			BinaryToStream.WriteToStream(_min.Y, _stream);
			BinaryToStream.WriteToStream(_max.X, _stream);
			BinaryToStream.WriteToStream(_max.Y, _stream);
		}

		private void ReadMetaData()
		{
			_stream.Position = 0;
			_triangleCount = BinaryToStream.ReadIntFromStream(_stream);
			_min = new HyperPoint<float>(BinaryToStream.ReadFloatFromStream(_stream), BinaryToStream.ReadFloatFromStream(_stream));
			_max = new HyperPoint<float>(BinaryToStream.ReadFloatFromStream(_stream), BinaryToStream.ReadFloatFromStream(_stream));
		}

		public void WriteElement(IElement element)
		{
			_triangleCount += element.TriangleCount;
			_min.X = Math.Min(_min.X, element.Min.X);
			_min.Y = Math.Min(_min.Y, element.Min.Y);
			_max.X = Math.Max(_max.X, element.Max.X);
			_max.Y = Math.Max(_max.Y, element.Max.Y);

			_stream.Position = _stream.Length;

			BinaryToStream.WriteToStream(element.FactoryID, _stream);
			element.SaveToStream(_stream);
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			WriteMetadata();
			_stream.Close();
			_stream.Dispose();
		}

		#endregion
	}
}
