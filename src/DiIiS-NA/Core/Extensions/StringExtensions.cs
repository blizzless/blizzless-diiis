using System;
using System.Collections.Generic;
using System.Text;

namespace DiIiS_NA.Core.Extensions
{
	public static class StringExtensions
	{
		public static string ZipCompress(this string value)
		{
			byte[] byteArray = new byte[value.Length];
			int indexBA = 0;
			foreach (char item in value.ToCharArray())
			{
				byteArray[indexBA++] = (byte)item;
			}

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			System.IO.Compression.GZipStream sw = new System.IO.Compression.GZipStream(ms,
				System.IO.Compression.CompressionMode.Compress);

			sw.Write(byteArray, 0, byteArray.Length);
			sw.Close();

			byteArray = ms.ToArray();
			System.Text.StringBuilder sB = new System.Text.StringBuilder(byteArray.Length);
			foreach (byte item in byteArray)
			{
				sB.Append((char)item);
			}

			ms.Close();
			sw.Dispose();
			ms.Dispose();
			return sB.ToString();
		}

		public static string UnZipCompress(this string value)
		{
			byte[] byteArray = new byte[value.Length];
			int indexBA = 0;
			foreach (char item in value.ToCharArray())
			{
				byteArray[indexBA++] = (byte)item;
			}

			System.IO.MemoryStream ms = new(byteArray);
			System.IO.Compression.GZipStream sr = new(ms,
				System.IO.Compression.CompressionMode.Decompress);

			byteArray = new byte[byteArray.Length];

			int rByte = sr.Read(byteArray, 0, byteArray.Length);

			System.Text.StringBuilder sB = new(rByte);
			for (int i = 0; i < rByte; i++)
			{
				sB.Append((char)byteArray[i]);
			}

			sr.Close();
			ms.Close();
			sr.Dispose();
			ms.Dispose();
			return sB.ToString();
		}
		public static byte[] ToBytes(this string bytes, Encoding encoding) => encoding.GetBytes(bytes);
		public static byte[] ToBytes(this string bytes) => bytes.ToBytes(Encoding.UTF8);
	}
}
