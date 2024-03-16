using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DiIiS_NA.Core.Helpers.Math;

namespace DiIiS_NA.Core.Extensions;

public static class EnumerableExtensions
{
	public static string ToHex(this byte b) => b.ToString("X2");
	public static string HexDump(this byte[] collection, bool skipSpace = false)
	{
		var sb = new StringBuilder();
		foreach (byte value in collection)
		{
			sb.Append(value.ToHex());
			if (!skipSpace)
				sb.Append(' ');
		}
		if (!skipSpace && sb.Length > 0)
			sb.Remove(sb.Length - 1, 1);
		return sb.ToString();
	}
	
	public static string HexDump(this IEnumerable<byte> collection, bool skipSpace = false)
	{
		return collection.ToArray().HexDump(skipSpace);
	}

	public static string ToEncodedString(this IEnumerable<byte> collection, Encoding encoding = null)
	{
		encoding ??= Encoding.UTF8;
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
			if (i > 0 && i % 16 == 0)
			{
				output.Append(hex);
				output.Append(' ');
				output.Append(text);
				output.Append(Environment.NewLine);
				hex.Clear(); text.Clear();
			}
			hex.Append(value.ToString("X2"));
			hex.Append(' ');
			text.Append($"{(char.IsWhiteSpace((char)value) && (char)value != ' ' ? '.' : (char)value)}"); // prettify text
			++i;
		}
		var hexRepresentation = hex.ToString();
		if (text.Length < 16)
		{
			hexRepresentation = hexRepresentation.PadRight(48); // pad the hex representation in-case it's smaller than a regular 16 value line.
		}
		output.Append(hexRepresentation);
		output.Append(' ');
		output.Append(text);
		return output.ToString();
	}

	public static TItem PickRandom<TItem>(this IEnumerable<TItem> source) => RandomHelper.RandomItem(source);

	public static bool TryPickRandom<TItem>(this IEnumerable<TItem> source, out TItem randomItem) => RandomHelper.TryGetRandomItem(source, out randomItem);
}