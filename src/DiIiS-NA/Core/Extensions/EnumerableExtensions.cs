//Blizzless Project 2022
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.Core.Extensions
{
	public static class EnumerableExtensions
	{
		public static string HexDump(this IEnumerable<byte> collection)
		{
			var sb = new StringBuilder();
			foreach (byte value in collection)
			{
				sb.Append(value.ToString("X2"));
				sb.Append(' ');
			}
			if (sb.Length > 0)
				sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}

		public static string ToEncodedString(this IEnumerable<byte> collection, Encoding encoding)
		{
			return encoding.GetString(collection.ToArray());
		}

		public static string Dump(this IEnumerable<byte> collection)
		{
			var output = new StringBuilder();
			var hex = new StringBuilder();
			var text = new StringBuilder();
			int i = 0;
			foreach (byte value in collection)
			{
				if (i > 0 && ((i % 16) == 0))
				{
					output.Append(hex);
					output.Append(' ');
					output.Append(text);
					output.Append(Environment.NewLine);
					hex.Clear(); text.Clear();
				}
				hex.Append(value.ToString("X2"));
				hex.Append(' ');
				text.Append(string.Format("{0}", (char.IsWhiteSpace((char)value) && (char)value != ' ') ? '.' : (char)value)); // prettify text
				++i;
			}
			var hexstring = hex.ToString();
			if (text.Length < 16)
			{
				hexstring = hexstring.PadRight(48); // pad the hex representation in-case it's smaller than a regular 16 value line.
			}
			output.Append(hexstring);
			output.Append(' ');
			output.Append(text);
			return output.ToString();
		}
	}
}
