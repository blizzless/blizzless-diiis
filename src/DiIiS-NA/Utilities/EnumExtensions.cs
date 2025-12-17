using System;
using Spectre.Console;

namespace DiIiS_NA.Utilities;

public static class EnumExtensions
{
    /// <summary>
    /// Translates enum value to its name as string.
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <param name="value">Which enum to apply</param>
    /// <returns>String value from enum.</returns>
    public static string GetName<T>(this T value) where T : Enum
    {
        return Enum.GetName(typeof(T), value) ?? "__NONE";
    }
    public static string FormatWithType<T>(this T value) where T : Enum
    {
        return $"$[darkolivegreen3_1]${typeof(T).Name}$[/]$.$[skyblue1]${Enum.GetName(typeof(T), value) ?? "__NONE"}$[/]$";
    }
    public static string GetNameWithValue<T>(this T value) where T : Enum
    {
        var numericValue = Convert.ToInt32((Enum)value);
        return $"{value.GetName()} ({numericValue})";
    }
    /// <summary>
    /// Translates enum value to its name as string with <param name="markup">Spectre.Console Markup string</param>
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    /// <param name="value">Which enum to apply</param>
    /// <param name="markup">The colors and styles for the <param name="value">Enum Value</param></param>
    /// <returns>String value from enum (<param name="value">Enum Value</param>) with <param name="markup">Spectre.Console Markup string</param>.</returns>
    public static string GetNameMarkup<T>(this T value, string markup = "darkmagenta") where T : Enum
    {
        var name = GetName(value);
        return name.Markup().Color(markup).Bold().ToString();
    }
}

public static class StringExtensions
{
    /// <summary>
    /// Transforms the string by wrapping it with the specified markup for Spectre.Console.
    /// </summary>
    /// <param name="text">Original text (escaped)</param>
    /// <param name="markup">Markup for the original text</param>
    /// <returns>Original text with markup.</returns>
    public static string WithMarkup(this object text, string markup) => $"$[{markup.EscapeMarkup()}]${text.ToString().EscapeMarkup()}$[/]$";
    
    public static IMarkupBuilder Markup(this object text)
    {
        return new MarkupBuilder(text.ToString());
    }
    public interface IMarkupBuilder
    {
        IMarkupBuilder Bold();
        IMarkupBuilder Italic();
        IMarkupBuilder Dim();
        IMarkupBuilder Underline();
        IMarkupBuilderBackground Color(string color);
        IMarkupBuilderBackground Color(Color color);
        IMarkupBuilderBackground Color(byte r, byte g, byte b);
    }

    public interface IMarkupBuilderBackground : IMarkupBuilder
    {
        IMarkupBuilder Background(string color);
        IMarkupBuilder Background(Color color);
        IMarkupBuilder Background(byte r, byte g, byte b);
    }

    public class MarkupBuilder : IMarkupBuilderBackground
    {
        private readonly StrBuilder _markup;
        private readonly string _value;
        public MarkupBuilder(string value)
        {
            _markup = new StrBuilder();
            _value = value.EscapeMarkup();
        }

        public IMarkupBuilder Bold()
        {
            _markup.Append("bold");
            return this;
        }

        public IMarkupBuilder Italic()
        {
            _markup.Append("italic");
            return this;
        }

        public IMarkupBuilder Dim()
        {
            _markup.Append("dim");
            return this;
        }

        public IMarkupBuilder Underline()
        {
            _markup.Append("underline");
            return this;
        }

        public IMarkupBuilderBackground Color(string color)
        {
            _markup.Append(color);
            return this;
        }

        public IMarkupBuilderBackground Color(Color color)
        {
            _markup.Append(color.ToString());
            return this;
        }

        public IMarkupBuilderBackground Color(byte r, byte g, byte b)
        {
            _markup.Append($"#{r:X2}{g:X2}{b:X2}");
            return this;
        }

        public IMarkupBuilder Background(string color)
        {
            _markup.Append("on " + color);
            return this;
        }

        public IMarkupBuilder Background(Color color)
        {
            _markup.Append("on " + color);
            return this;
        }

        public IMarkupBuilder Background(byte r, byte g, byte b)
        {
            _markup.Append($"on #{r:X2}{g:X2}{b:X2}");
            return this;
        }

        public override string ToString()
        {
            return _value.WithMarkup(_markup.ToString(Separator.Space).EscapeMarkup());
        }
    }
}