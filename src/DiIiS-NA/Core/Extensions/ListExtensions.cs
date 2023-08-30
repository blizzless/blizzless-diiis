using System.Collections.Generic;
using System.Linq;

namespace DiIiS_NA.Core.Extensions
{
	public static class ListExtensions
	{
		public static bool ContainsAtLeastOne<T>(this List<T> list1, List<T> list2)
		{
			return list2.Any(list1.Contains);
		}

		public static bool ContainsAtLeastOne<T>(this List<T> list, T[] array)
		{
			return array.Any(list.Contains);
		}
	}
}
