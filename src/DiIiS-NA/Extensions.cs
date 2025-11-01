using System;
using System.Configuration;

namespace DiIiS_NA
{
    internal static class Globals
    {
        /// <summary>
        /// The default tolerance for floating point comparisons.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const float FLOAT_TOLERANCE = 0.00001f;

        public const float ZERO = 0.0000000f;

        public static bool IsWithinTolerance(this float source, float comparer, float? customTolerance = null)
        {
            customTolerance ??= FLOAT_TOLERANCE;
            return Math.Abs(source - comparer) < customTolerance;
        }

        public static bool IsZero(this float source)
        {
            return Math.Abs(source - ZERO) < FLOAT_TOLERANCE;
        }
    }

    public static class StringExtensions
    {
        public static bool CompareWith(this string? str, string toCompare)
        {
            if (str == null) return false;
            return str!.Equals(toCompare, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}