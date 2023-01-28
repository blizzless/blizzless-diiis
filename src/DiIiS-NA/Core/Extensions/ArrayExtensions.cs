//Blizzless Project 2022
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiIiS_NA.Core.Extensions
{
	public static class ArrayExtensions
	{
		public static IEnumerable<T> EnumerateFrom<T>(this T[] array, int start)
		{
			if (array == null)
				throw new ArgumentNullException("array");

			return Enumerate<T>(array, start, array.Length);
		}

		public static IEnumerable<T> Enumerate<T>(this T[] array, int start, int count)
		{
			if (array == null)
				throw new ArgumentNullException("array");

			for (int i = 0; i < count; i++)
				yield return array[start + i];
		}

		public static byte[] Append(this byte[] a, byte[] b)
		{
			var result = new byte[a.Length + b.Length];

			a.CopyTo(result, 0);
			b.CopyTo(result, a.Length);

			return result;
		}

		public static bool CompareTo(this byte[] byteArray, byte[] second)
		{
			if (byteArray.Length != second.Length)
				return false;

			return !byteArray.Where((t, i) => second[i] != t).Any();
		}

		public static string Dump(this byte[] array)
		{
			return EnumerableExtensions.Dump(array);
		}

		public static string ToHexString(this byte[] byteArray)
		{
			return byteArray.Aggregate("", (current, b) => current + b.ToString("X2"));
		}

		public static string ToFormatedHexString(this byte[] byteArray)
		{
			return byteArray.Aggregate("", (current, b) => current + " " + b.ToString("X2"));
		}

		public static byte[] ToByteArray(this string str)
		{
			str = str.Replace(" ", String.Empty);

			var res = new byte[str.Length / 2];
			for (int i = 0; i < res.Length; ++i)
			{
				string temp = String.Concat(str[i * 2], str[i * 2 + 1]);
				res[i] = Convert.ToByte(temp, 16);
			}
			return res;
		}

		public static void ForEach(this Array array, Action<Array, int[]> action)
		{
			if (array.LongLength == 0) return;
			ArrayTraverse walker = new ArrayTraverse(array);
			do action(array, walker.Position);
			while (walker.Step());
		}

		public static int FindIndex<T>(this T[] source, Predicate<T> match)
		{
			return Array.FindIndex(source, match);
		}
	}

	internal class ArrayTraverse
	{
		public int[] Position;
		private int[] maxLengths;

		public ArrayTraverse(Array array)
		{
			maxLengths = new int[array.Rank];
			for (int i = 0; i < array.Rank; ++i)
			{
				maxLengths[i] = array.GetLength(i) - 1;
			}
			Position = new int[array.Rank];
		}

		public bool Step()
		{
			for (int i = 0; i < Position.Length; ++i)
			{
				if (Position[i] < maxLengths[i])
				{
					Position[i]++;
					for (int j = 0; j < i; j++)
					{
						Position[j] = 0;
					}
					return true;
				}
			}
			return false;
		}
	}
}
