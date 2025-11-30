using System;
using System.Linq;
using System.Collections.Generic;

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

	public static float NextFloat()
	{
        return (float)Random.NextDouble();
    }

	public static float NextFloat(float min, float max)
	{
        return min + (float)Random.NextDouble() * (max - min);
    }

	public static float NextFloat(float max)
	{
        return (float)Random.NextDouble() * max;
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
	
	public static bool TryGetRandomItem<T>(IEnumerable<T> source, out T randomItem)
	{
		var collection = source as IReadOnlyCollection<T> ?? source?.ToArray();
		if (collection == null)
		{
			throw new ArgumentException("Cannot be null", nameof(source));
		}

		if (collection.Count == 0)
		{
			randomItem = default;
			return false;
		}
		
		var randomIndex = Next(collection.Count);
		randomItem = collection.ElementAt(randomIndex);

		return true;
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
	uint _a;
	uint _b;
	public ItemRandomHelper(int seed)
	{
		_a = (uint)seed;
		_b = 666;
	}

	public void ReinitSeed()
	{
		_b = 666;
	}

	public uint Next()
	{
		ulong temp = 1791398085UL * _a + _b;
		_a = (uint)temp;
		_b = (uint)(temp >> 32);

		return _a;
	}

	public float Next(float min, float max)
	{
		return min + (Next() % (uint)(max - min + 1));
	}
}