using System;
using System.Configuration;

namespace DiIiS_NA
{
    internal static class Globals
    {
        public const float FLOAT_TOLERANCE = 0.0001f;
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