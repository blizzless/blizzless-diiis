using System;
using System.Numerics;

namespace DiIiS_NA.Core.Extensions
{
	public static class BigIntegerExtensions
	{
		public static BigInteger ToBigInteger(this byte[] src)
		{
			var dst = new byte[src.Length + 1];
			Array.Copy(src, dst, src.Length);
			return new BigInteger(dst);
		}

		public static byte[] ToArray(this BigInteger b)
		{
			var result = b.ToByteArray();
			if (result[result.Length - 1] == 0 && (result.Length % 0x10) != 0)
				Array.Resize(ref result, result.Length - 1);
			return result;
		}

		public static byte[] ToArray(this BigInteger b, int size)
		{
			byte[] result = b.ToArray();
			if (result.Length > size)
				throw new ArgumentOutOfRangeException("size", size, "must be large enough to convert the BigInteger");

			// If the size is already correct, return the result.
			if (result.Length == size)
				return result;

			// Resize the array.
			int n = size - result.Length;
			Array.Resize(ref result, size);

			// Fill the extra bytes with 0x00.
			while (n > 0)
			{
				result[size - n] = 0x00;
				n--;
			}

			return result;
		}
	}
}
