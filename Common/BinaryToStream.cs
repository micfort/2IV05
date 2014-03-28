using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CG_2IV05.Common
{
	class BinaryToStream
	{
		public static void WriteToStream(bool o, Stream outputStream)
		{
			byte[] buffer = BitConverter.GetBytes(o);
			outputStream.Write(buffer, 0, buffer.Length);
		}

		public static bool ReadBoolFromStream(Stream inputStream)
		{
			byte[] buffer = new byte[1];
			inputStream.Read(buffer, 0, buffer.Length);
			bool output = BitConverter.ToBoolean(buffer, 0);
			return output;
		}

		public static void WriteToStream(byte o, Stream outputStream)
		{
			byte[] buffer = new byte[] { o };
			outputStream.Write(buffer, 0, buffer.Length);
		}

		public static byte ReadByteFromStream(Stream inputStream)
		{
			byte[] buffer = new byte[1];
			inputStream.Read(buffer, 0, buffer.Length);
			return buffer[0];
		}

		public static void WriteToStream(sbyte o, Stream outputStream)
		{
			byte[] buffer = new byte[] { (byte)o };
			outputStream.Write(buffer, 0, buffer.Length);
		}

		public static sbyte ReadSByteFromStream(Stream inputStream)
		{
			byte[] buffer = new byte[1];
			inputStream.Read(buffer, 0, buffer.Length);
			return (sbyte)buffer[0];
		}

		public static void WriteToStream(char o, Stream outputStream)
		{
			byte[] buffer = BitConverter.GetBytes(o);
			outputStream.Write(buffer, 0, buffer.Length);
		}

		public static char ReadCharFromStream(Stream inputStream)
		{
			byte[] buffer = new byte[sizeof(char)];
			inputStream.Read(buffer, 0, buffer.Length);
			char output = BitConverter.ToChar(buffer, 0);
			return output;
		}

		public static void WriteToStream(decimal o, Stream outputStream)
		{
			int[] data = Decimal.GetBits(o);
			for (int i = 0; i < 4; i++)
			{
				WriteToStream(data[i], outputStream);
			}
		}

		public static decimal ReadDecimalFromStream(Stream inputStream)
		{
			int[] data = new int[4];
			for (int i = 0; i < 4; i++)
			{
				data[i] = ReadIntFromStream(inputStream);
			}
			return new decimal(data);
		}

		public static void WriteToStream(double o, Stream outputStream)
		{
			byte[] buffer = BitConverter.GetBytes(o);
			outputStream.Write(buffer, 0, buffer.Length);
		}

		public static double ReadDoubleFromStream(Stream inputStream)
		{
			byte[] buffer = new byte[sizeof(double)];
			inputStream.Read(buffer, 0, buffer.Length);
			double output = BitConverter.ToDouble(buffer, 0);
			return output;
		}

		public static void WriteToStream(float o, Stream outputStream)
		{
			byte[] buffer = BitConverter.GetBytes(o);
			outputStream.Write(buffer, 0, buffer.Length);
		}

		public static float ReadFloatFromStream(Stream inputStream)
		{
			byte[] buffer = new byte[sizeof(float)];
			inputStream.Read(buffer, 0, buffer.Length);
			float output = BitConverter.ToSingle(buffer, 0);
			return output;
		}

		public static void WriteToStream(int o, Stream outputStream)
		{
			byte[] buffer = BitConverter.GetBytes(o);
			outputStream.Write(buffer, 0, buffer.Length);
		}

		public static int ReadIntFromStream(Stream inputStream)
		{
			byte[] buffer = new byte[sizeof(int)];
			inputStream.Read(buffer, 0, buffer.Length);
			int output = BitConverter.ToInt32(buffer, 0);
			return output;
		}

		public static void WriteToStream(uint o, Stream outputStream)
		{
			byte[] buffer = BitConverter.GetBytes(o);
			outputStream.Write(buffer, 0, buffer.Length);
		}

		public static uint ReadUIntFromStream(Stream inputStream)
		{
			byte[] buffer = new byte[sizeof(uint)];
			inputStream.Read(buffer, 0, buffer.Length);
			uint output = BitConverter.ToUInt32(buffer, 0);
			return output;
		}

		public static void WriteToStream(long o, Stream outputStream)
		{
			byte[] buffer = BitConverter.GetBytes(o);
			outputStream.Write(buffer, 0, buffer.Length);
		}

		public static long ReadLongFromStream(Stream inputStream)
		{
			byte[] buffer = new byte[sizeof(long)];
			inputStream.Read(buffer, 0, buffer.Length);
			long output = BitConverter.ToInt64(buffer, 0);
			return output;
		}

		public static void WriteToStream(ulong o, Stream outputStream)
		{
			byte[] buffer = BitConverter.GetBytes(o);
			outputStream.Write(buffer, 0, buffer.Length);
		}

		public static ulong ReadULongFromStream(Stream inputStream)
		{
			byte[] buffer = new byte[sizeof(ulong)];
			inputStream.Read(buffer, 0, buffer.Length);
			ulong output = BitConverter.ToUInt64(buffer, 0);
			return output;
		}

		public static void WriteToStream(short o, Stream outputStream)
		{
			byte[] buffer = BitConverter.GetBytes(o);
			outputStream.Write(buffer, 0, buffer.Length);
		}

		public static short ReadShortFromStream(Stream inputStream)
		{
			byte[] buffer = new byte[sizeof(short)];
			inputStream.Read(buffer, 0, buffer.Length);
			short output = BitConverter.ToInt16(buffer, 0);
			return output;
		}

		public static void WriteToStream(ushort o, Stream outputStream)
		{
			byte[] buffer = BitConverter.GetBytes(o);
			outputStream.Write(buffer, 0, buffer.Length);
		}

		public static ushort ReadUShortFromStream(Stream inputStream)
		{
			byte[] buffer = new byte[sizeof(ushort)];
			inputStream.Read(buffer, 0, buffer.Length);
			ushort output = BitConverter.ToUInt16(buffer, 0);
			return output;
		}

		public static void WriteToStream(string s, Stream outputStream)
		{
			byte[] nameBuffer = Encoding.Unicode.GetBytes(s);
			int nameBufferLength = nameBuffer.Length;
			byte[] nameBufferLengthBuffer = BitConverter.GetBytes(nameBufferLength);

			outputStream.Write(nameBufferLengthBuffer, 0, nameBufferLengthBuffer.Length);
			outputStream.Write(nameBuffer, 0, nameBuffer.Length);
		}

		public static string ReadStringFromStream(Stream inputStream)
		{
			int bufferLength = ReadIntFromStream(inputStream);
			byte[] buffer = new byte[bufferLength];
			inputStream.Read(buffer, 0, buffer.Length);
			return Encoding.Unicode.GetString(buffer, 0, bufferLength);
		}
	}
}
