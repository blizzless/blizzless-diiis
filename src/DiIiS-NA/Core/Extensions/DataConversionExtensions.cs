using System;

namespace DiIiS_NA.Core.Extensions;

public static class MathConversionsOperations
{
    public static int Floor(this float value) => (int) Math.Floor(value);
    public static int Ceiling(this float value) => (int) Math.Ceiling(value);
    public static int Floor(this double value) => (int) Math.Floor(value);
    public static int Ceil(this double value) => (int) Math.Floor(value);
}