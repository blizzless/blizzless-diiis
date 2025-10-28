using System;

namespace DiIiS_NA.Utilities;

public static class EnumExtensions
{
    public static string GetName<T>(this T value) where T : Enum
    {
        return Enum.GetName(typeof(T), value) ?? "__NONE";
    }
}