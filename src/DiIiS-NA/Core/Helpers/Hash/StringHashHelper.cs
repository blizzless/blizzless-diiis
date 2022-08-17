//Blizzless Project 2022
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;

namespace DiIiS_NA.Core.Helpers.Hash
{
	public class StringHashHelper
	{
		public static uint HashIdentity(string input)
		{
			var bytes = Encoding.ASCII.GetBytes(input);
			return bytes.Aggregate(0x811C9DC5, (current, t) => 0x1000193 * (t ^ current));
		}

		public static int HashItemName(string input)
		{
			int hash = 0;
			input = input.ToLower();
			for (int i = 0; i < input.Length; ++i)
				hash = (hash << 5) + hash + input[i];
			return hash;
		}

		public static int HashNormal(string input)
		{
			int hash = 0;
			for (int i = 0; i < input.Length; ++i)
				hash = (hash << 5) + hash + input[i];
			return hash;
		}

	}
}
