using System;
using System.Linq;
using System.Collections.Generic;
using DiIiS_NA.Core.Logging;

namespace DiIiS_NA.Core.Helpers.Math;

public static class RandomHelper
{
	private static readonly Random Random = new();

	public static int Next()
	{
		return Random.Next();
	}

	public static int Next(int maxValue)
	{
		return Random.Next(maxValue);
	}

	public static int Next(int minValue, int maxValue)
	{
		return Random.Next(minValue, maxValue);
	}

	public static void NextBytes(byte[] buffer)
	{
		Random.NextBytes(buffer);
	}

	public static double NextDouble()
	{
		return Random.NextDouble();
	}

	public static T RandomItem<T>(IEnumerable<T> source)
	{
		var collection = source as IReadOnlyCollection<T> ?? source?.ToArray();
		if (collection == null || collection.Count == 0)
		{
			throw new ArgumentException("Cannot be null or empty", nameof(source));
		}

		var randomIndex = Next(collection.Count);
		return collection.ElementAt(randomIndex);
	}

	/// <summary>
	/// Picks a random item from a list
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source"></param>
	/// <param name="probability">A function that assigns each item a probability. If the probabilities dont sum up to 1, they are normalized</param>
	/// <returns></returns>
	public static T RandomItem<T>(IEnumerable<T> source, Func<T, float> probability)
	{
		var collection = source as IReadOnlyCollection<T> ?? source.ToArray();
		int cumulative = (int)collection.Select(probability).Sum();

		int randomRoll = Next(cumulative);
		float cumulativePercentages = 0;

		foreach (T element in collection)
		{
			cumulativePercentages += probability(element);
			if (cumulativePercentages > randomRoll)
				return element;
		}

		return collection.First();
	}
}

public class ItemRandomHelper
{
	uint a;
	uint b;
	public ItemRandomHelper(int seed)
	{
		a = (uint)seed;
		b = 666;
	}

	public void ReinitSeed()
	{
		b = 666;
	}

	public uint Next()
	{
		ulong temp = 1791398085UL * a + b;
		a = (uint)temp;
		b = (uint)(temp >> 32);

		return a;
	}

	public float Next(float min, float max)
	{
		return min + (Next() % (uint)(max - min + 1));
	}
}