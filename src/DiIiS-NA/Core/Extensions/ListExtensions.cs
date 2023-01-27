//Blizzless Project 2022
using System.Collections.Generic;

namespace DiIiS_NA.Core.Extensions
{
	public static class ListExtensions
	{
		public static bool ContainsAtLeastOne<T>(this List<T> list1, List<T> list2)
		{
			foreach (T m in list2)
			{
				if (list1.Contains(m))
					return true;
			}
			return false;
		}

		public static bool ContainsAtLeastOne<T>(this List<T> list, T[] array)
		{
			foreach (T m in array)
			{
				if (list.Contains(m))
					return true;
			}
			return false;
		}
	}
}
