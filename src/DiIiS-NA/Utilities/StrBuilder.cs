using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DiIiS_NA.Utilities;

/// <summary>
/// Represents a separator for joining string parts.
/// Value Object following DDD principles.
/// </summary>
/// <remarks>
/// <para>
/// <c>Separator</c> is an immutable value object that encapsulates string separator logic.
/// It provides common predefined separators and a factory method for custom separators.
/// </para>
/// <para>
/// Predefined separators include:
/// <list type="bullet">
/// <item><description><c>Default</c> - ", " (comma-space)</description></item>
/// <item><description><c>Comma</c> - ","</description></item>
/// <item><description><c>Space</c> - " "</description></item>
/// <item><description><c>Empty</c> - "" (no separator)</description></item>
/// <item><description><c>NewLine</c> - Environment.NewLine</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Using predefined separators
/// var commaSeparator = Separator.Comma;
/// var customSeparator = Separator.From(" | ");
/// 
/// // With character
/// var pipeSeparator = Separator.From('|');
/// </code>
/// </example>
public readonly struct Separator
{
    private readonly string _value;

    public string Value => _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Separator"/> struct with the specified value.
    /// </summary>
    /// <param name="value">The separator string value. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    private Separator(string value) => _value = value ?? throw new ArgumentNullException(nameof(value));

    /// <summary>
    /// Gets the default separator: ", " (comma followed by space).
    /// </summary>
    /// <value>A <see cref="Separator"/> with value ", ".</value>
    public static Separator Default => new(", ");

    /// <summary>
    /// Gets the comma separator.
    /// </summary>
    /// <value>A <see cref="Separator"/> with value ",".</value>
    public static Separator Comma => new(",");

    /// <summary>
    /// Gets the space separator.
    /// </summary>
    /// <value>A <see cref="Separator"/> with value " ".</value>
    public static Separator Space => new(" ");

    /// <summary>
    /// Gets the empty separator (concatenates parts without a separator).
    /// </summary>
    /// <value>A <see cref="Separator"/> with empty string value.</value>
    public static Separator Empty => new("");

    /// <summary>
    /// Gets the newline separator using <see cref="Environment.NewLine"/>.
    /// </summary>
    /// <value>A <see cref="Separator"/> with <see cref="Environment.NewLine"/> value.</value>
    public static Separator NewLine => new(Environment.NewLine);

    /// <summary>
    /// Creates a custom separator from the specified string value.
    /// </summary>
    /// <param name="value">The separator string. Cannot be null.</param>
    /// <returns>A new <see cref="Separator"/> with the specified value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <example>
    /// <code>
    /// var separator = Separator.From(" | ");
    /// </code>
    /// </example>
    public static Separator From(string value) => new(value);

    /// <summary>
    /// Creates a custom separator from the specified character.
    /// </summary>
    /// <param name="value">The separator character.</param>
    /// <returns>A new <see cref="Separator"/> with the character converted to string.</returns>
    /// <example>
    /// <code>
    /// var separator = Separator.From('|');
    /// </code>
    /// </example>
    public static Separator From(char value) => new(value.ToString());

    /// <summary>
    /// Returns the string representation of this separator.
    /// </summary>
    /// <returns>The separator string value.</returns>
    public override string ToString() => _value;

    public static implicit operator Separator(string separator)
    {
        return new Separator(separator);
    }
}

/// <summary>
/// Fluent builder for composing strings from multiple parts.
/// Follows DDD with clear domain language and Builder pattern.
/// Immutable by design (returns new instances from mutations).
/// </summary>
/// <remarks>
/// <para>
/// <c>StrBuilder</c> is a fluent, immutable string builder that accumulates string parts
/// and can combine them with a specified separator. It implements the Builder pattern
/// with a domain-driven design approach.
/// </para>
/// <para>
/// Key features:
/// <list type="bullet">
/// <item><description>Immutable - all mutation methods return new instances</description></item>
/// <item><description>Fluent API - supports method chaining</description></item>
/// <item><description>Conditional operations - AppendIf, AppendIfNot, etc.</description></item>
/// <item><description>Bundling - combines parts into a single part with separator</description></item>
/// <item><description>JSON serialization - converts builder state to JSON</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Basic usage
/// var result = StrBuilder.From("Hello")
///     .Append("World")
///     .ToString(Separator.Space);
/// // Result: "Hello World"
/// 
/// // Conditional building
/// var role = "admin";
/// var message = StrBuilder.From("User")
///     .AppendIf(role == "admin", "(Administrator)")
///     .ToString(Separator.Space);
/// // Result: "User (Administrator)"
/// 
/// // Bundling
/// var bundled = StrBuilder.From("A").Append("B").Append("C")
///     .Bundle(Separator.Comma)
///     .Append("D")
///     .ToString(Separator.Comma);
/// // Result: "A,B,C, D"
/// </code>
/// </example>
/// <seealso cref="Separator"/>
/// <seealso cref="StrBuilderSerializer"/>
public readonly struct StrBuilder
{
    private readonly List<string> _parts;

    /// <summary>
    /// Initializes a new instance of the <see cref="StrBuilder"/> struct with an empty parts collection.
    /// </summary>
    public StrBuilder() => _parts = new();

    /// <summary>
    /// Creates a new builder with a single initial part.
    /// </summary>
    /// <param name="part">The initial string part. Empty or null parts are ignored.</param>
    /// <returns>A new <see cref="StrBuilder"/> containing the specified part.</returns>
    /// <example>
    /// <code>
    /// var builder = StrBuilder.From("Hello");
    /// </code>
    /// </example>
    /// <seealso cref="Append(string)"/>
    public static StrBuilder From(string part) => new StrBuilder().Append(part);

    /// <summary>
    /// Creates a new builder with an initial part if the specified condition is true.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="part">The initial string part to add if condition is true.</param>
    /// <returns>A new <see cref="StrBuilder"/> with the part if condition is true; otherwise, an empty builder.</returns>
    /// <example>
    /// <code>
    /// bool isVerbose = true;
    /// var builder = StrBuilder.FromIf(isVerbose, "[DEBUG]");
    /// </code>
    /// </example>
    /// <seealso cref="AppendIf(bool, string)"/>
    /// <seealso cref="FromIfNot(bool, string)"/>
    public static StrBuilder FromIf(bool condition, string part)
        => condition ? From(part) : new StrBuilder();

    /// <summary>
    /// Creates a new builder with an initial part if the specified condition is false.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="part">The initial string part to add if condition is false.</param>
    /// <returns>A new <see cref="StrBuilder"/> with the part if condition is false; otherwise, an empty builder.</returns>
    /// <example>
    /// <code>
    /// bool isProduction = false;
    /// var builder = StrBuilder.FromIfNot(isProduction, "[TEST]");
    /// </code>
    /// </example>
    /// <seealso cref="AppendIfNot(bool, string)"/>
    /// <seealso cref="FromIf(bool, string)"/>
    public static StrBuilder FromIfNot(bool condition, string part)
        => !condition ? From(part) : new StrBuilder();

    /// <summary>
    /// Appends a string part to this builder.
    /// </summary>
    /// <param name="part">The string part to append. Empty or null parts are ignored.</param>
    /// <returns>This builder instance (for method chaining).</returns>
    /// <remarks>
    /// <para>
    /// This method mutates the current builder instance and returns <c>this</c>.
    /// Empty or null parts are silently ignored and not added to the internal collection.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var builder = new StrBuilder()
    ///     .Append("Hello")
    ///     .Append("World");
    /// </code>
    /// </example>
    /// <seealso cref="AppendIf(bool, string)"/>
    /// <seealso cref="AppendIfNot(bool, string)"/>
    public StrBuilder Append(string part)
    {
        if (string.IsNullOrEmpty(part)) return this;
        _parts.Add(part);
        return this;
    }

    /// <summary>
    /// Conditionally appends a string part if the specified condition is true.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="part">The string part to append if condition is true.</param>
    /// <returns>This builder instance (for method chaining).</returns>
    /// <example>
    /// <code>
    /// var hasError = true;
    /// var builder = new StrBuilder()
    ///     .Append("Status:")
    ///     .AppendIf(hasError, "ERROR")
    ///     .AppendIfNot(hasError, "OK");
    /// // Result when hasError is true: "Status: ERROR"
    /// </code>
    /// </example>
    /// <seealso cref="Append(string)"/>
    /// <seealso cref="AppendIfNot(bool, string)"/>
    public StrBuilder AppendIf(bool condition, string part)
        => condition ? Append(part) : this;

    /// <summary>
    /// Conditionally appends a string part if the specified condition is false.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="part">The string part to append if condition is false.</param>
    /// <returns>This builder instance (for method chaining).</returns>
    /// <example>
    /// <code>
    /// var isAuthenticated = false;
    /// var builder = new StrBuilder()
    ///     .Append("Access:")
    ///     .AppendIfNot(isAuthenticated, "DENIED");
    /// // Result: "Access: DENIED"
    /// </code>
    /// </example>
    /// <seealso cref="Append(string)"/>
    /// <seealso cref="AppendIf(bool, string)"/>
    public StrBuilder AppendIfNot(bool condition, string part)
        => !condition ? Append(part) : this;

    /// <summary>
    /// Composes all accumulated parts into a single bundled part using the specified separator.
    /// Returns a new builder with the bundled result as its single part.
    /// </summary>
    /// <param name="separator">The separator to use when joining parts.</param>
    /// <returns>A new <see cref="StrBuilder"/> containing the bundled result as a single part.</returns>
    /// <remarks>
    /// <para>
    /// This method is useful for grouping multiple parts together while maintaining
    /// the ability to add more parts after bundling.
    /// </para>
    /// <para>
    /// If the current builder has no parts, it returns <c>this</c> unchanged.
    /// The new builder contains only the bundled string as a single part.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = StrBuilder.From("A")
    ///     .Append("B")
    ///     .Append("C")
    ///     .Bundle(Separator.Comma)  // Creates single part "A,B,C"
    ///     .Append("D")
    ///     .ToString(Separator.Comma);
    /// // Result: "A,B,C, D"
    /// </code>
    /// </example>
    /// <seealso cref="Bundle(string)"/>
    /// <seealso cref="Bundle(char)"/>
    /// <seealso cref="Bundle()"/>
    public StrBuilder Bundle(Separator separator)
    {
        if (_parts.Count == 0) return this;

        var bundled = string.Join(separator.ToString(), _parts);
        var newBuilder = new StrBuilder();
        newBuilder._parts.Add(bundled);
        return newBuilder;
    }

    /// <summary>
    /// Composes all accumulated parts into a single bundled part using the specified string separator.
    /// </summary>
    /// <param name="separator">The separator string to use when joining parts.</param>
    /// <returns>A new <see cref="StrBuilder"/> containing the bundled result as a single part.</returns>
    /// <example>
    /// <code>
    /// var result = StrBuilder.From("A")
    ///     .Append("B")
    ///     .Bundle(" | ");
    /// </code>
    /// </example>
    /// <seealso cref="Bundle(Separator)"/>
    /// <seealso cref="Bundle(char)"/>
    public StrBuilder Bundle(string separator) => Bundle(Separator.From(separator));

    /// <summary>
    /// Composes all accumulated parts into a single bundled part using the specified character separator.
    /// </summary>
    /// <param name="separator">The separator character to use when joining parts.</param>
    /// <returns>A new <see cref="StrBuilder"/> containing the bundled result as a single part.</returns>
    /// <example>
    /// <code>
    /// var result = StrBuilder.From("A")
    ///     .Append("B")
    ///     .Bundle('|');
    /// </code>
    /// </example>
    /// <seealso cref="Bundle(Separator)"/>
    /// <seealso cref="Bundle(string)"/>
    public StrBuilder Bundle(char separator) => Bundle(Separator.From(separator));

    /// <summary>
    /// Composes all accumulated parts into a single bundled part using the default separator (", ").
    /// </summary>
    /// <returns>A new <see cref="StrBuilder"/> containing the bundled result as a single part.</returns>
    /// <example>
    /// <code>
    /// var result = StrBuilder.From("A")
    ///     .Append("B")
    ///     .Bundle()  // Uses default ", " separator
    ///     .ToString();
    /// // Result: "A, B"
    /// </code>
    /// </example>
    /// <seealso cref="Bundle(Separator)"/>
    /// <seealso cref="Separator.Default"/>
    public StrBuilder Bundle() => Bundle(Separator.Default);

    /// <summary>
    /// Converts all accumulated parts to a string using the default separator (", ").
    /// </summary>
    /// <returns>A string containing all parts joined by the default separator.</returns>
    /// <remarks>
    /// <para>
    /// This method is the primary conversion to string. It joins all accumulated parts
    /// using the default separator and returns the result.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = StrBuilder.From("Hello")
    ///     .Append("World")
    ///     .ToString();
    /// // Result: "Hello, World"
    /// </code>
    /// </example>
    /// <seealso cref="ToString(string)"/>
    /// <seealso cref="ToString(char)"/>
    /// <seealso cref="ToString(Separator)"/>
    public override string ToString() => ToString(Separator.Default);

    /// <summary>
    /// Converts all accumulated parts to a string using the specified string separator.
    /// </summary>
    /// <param name="separator">The separator string to use when joining parts.</param>
    /// <returns>A string containing all parts joined by the specified separator.</returns>
    /// <example>
    /// <code>
    /// var result = StrBuilder.From("A")
    ///     .Append("B")
    ///     .Append("C")
    ///     .ToString(" | ");
    /// // Result: "A | B | C"
    /// </code>
    /// </example>
    /// <seealso cref="ToString()"/>
    /// <seealso cref="ToString(char)"/>
    /// <seealso cref="ToString(Separator)"/>
    public string ToString(string separator) => ToString(Separator.From(separator));

    /// <summary>
    /// Converts all accumulated parts to a string using the specified character separator.
    /// </summary>
    /// <param name="separator">The separator character to use when joining parts.</param>
    /// <returns>A string containing all parts joined by the specified separator.</returns>
    /// <example>
    /// <code>
    /// var result = StrBuilder.From("A")
    ///     .Append("B")
    ///     .ToString('|');
    /// // Result: "A|B"
    /// </code>
    /// </example>
    /// <seealso cref="ToString()"/>
    /// <seealso cref="ToString(string)"/>
    /// <seealso cref="ToString(Separator)"/>
    public string ToString(char separator) => ToString(Separator.From(separator));

    /// <summary>
    /// Converts all accumulated parts to a string using the specified separator.
    /// </summary>
    /// <param name="separator">The separator to use when joining parts.</param>
    /// <returns>A string containing all parts joined by the specified separator.</returns>
    /// <example>
    /// <code>
    /// var result = StrBuilder.From("First")
    ///     .Append("Second")
    ///     .ToString(Separator.NewLine);
    /// // Result: "First\nSecond" (or \r\n on Windows)
    /// </code>
    /// </example>
    /// <seealso cref="ToString()"/>
    /// <seealso cref="ToString(string)"/>
    /// <seealso cref="ToString(char)"/>
    public string ToString(Separator separator) => string.Join(separator.ToString(), _parts);

    /// <summary>
    /// Serializes the builder state to a JSON string.
    /// </summary>
    /// <param name="opts">Optional <see cref="JsonSerializerOptions"/> for customizing serialization. 
    /// If null, default options are used.</param>
    /// <returns>A JSON string representation of the builder's parts.</returns>
    /// <remarks>
    /// <para>
    /// The serialization is delegated to <see cref="StrBuilderSerializer"/>
    /// following the Single Responsibility Principle.
    /// </para>
    /// <para>
    /// By default, the JSON is compact (not indented). To customize formatting,
    /// pass <see cref="JsonSerializerOptions"/> with <c>WriteIndented = true</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var json = StrBuilder.From("Hello")
    ///     .Append("World")
    ///     .ToJson();
    /// // Result: {"parts":["Hello","World"]}
    /// 
    /// // With custom formatting
    /// var opts = new JsonSerializerOptions { WriteIndented = true };
    /// var prettyJson = StrBuilder.From("Hello")
    ///     .Append("World")
    ///     .ToJson(opts);
    /// </code>
    /// </example>
    /// <seealso cref="StrBuilderSerializer"/>
    /// <seealso cref="StrBuilderSerializer.Serialize(StrBuilder, JsonSerializerOptions)"/>
    public string ToJson(JsonSerializerOptions? opts = null)
        => StrBuilderSerializer.Serialize(this, opts);
}

/// <summary>
/// Handles JSON serialization of <see cref="StrBuilder"/>.
/// Follows SRP: separation of serialization concerns.
/// </summary>
/// <remarks>
/// <para>
/// <c>StrBuilderSerializer</c> is a utility class dedicated to serializing
/// <see cref="StrBuilder"/> instances to JSON format. It encapsulates all
/// serialization logic in a single place, adhering to the Single Responsibility Principle.
/// </para>
/// <para>
/// The default serialization options produce compact JSON output (not indented).
/// Custom formatting can be achieved by passing <see cref="JsonSerializerOptions"/>
/// to the <see cref="Serialize(StrBuilder, JsonSerializerOptions)"/> method.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var builder = StrBuilder.From("Item1").Append("Item2");
/// var json = StrBuilderSerializer.Serialize(builder);
/// // Result: {"parts":["Item1","Item2"]}
/// 
/// // With custom options
/// var opts = new JsonSerializerOptions { WriteIndented = true };
/// var prettyJson = StrBuilderSerializer.Serialize(builder, opts);
/// </code>
/// </example>
/// <seealso cref="StrBuilder"/>
/// <seealso cref="StrBuilder.ToJson(JsonSerializerOptions)"/>
public static class StrBuilderSerializer
{
    private static readonly JsonSerializerOptions DefaultOptions = new() { WriteIndented = false };

    /// <summary>
    /// Serializes a <see cref="StrBuilder"/> instance to a JSON string.
    /// </summary>
    /// <param name="builder">The <see cref="StrBuilder"/> to serialize.</param>
    /// <param name="opts">Optional <see cref="JsonSerializerOptions"/> for customizing serialization.
    /// If null, compact JSON formatting is used.</param>
    /// <returns>A JSON string representation of the builder's parts array.</returns>
    /// <remarks>
    /// <para>
    /// The method converts the builder to its string representation using the default separator,
    /// then splits it back into parts, and serializes as a JSON object with a "parts" array.
    /// </para>
    /// <para>
    /// If custom serialization behavior is needed, create <see cref="JsonSerializerOptions"/>
    /// with desired settings:
    /// <list type="bullet">
    /// <item><description>WriteIndented = true for pretty-printing</description></item>
    /// <item><description>PropertyNameCaseInsensitive = true for case-insensitive property matching</description></item>
    /// <item><description>Custom converters for special type handling</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var builder = StrBuilder.From("A").Append("B").Append("C");
    /// 
    /// // Compact JSON
    /// var compact = StrBuilderSerializer.Serialize(builder);
    /// // Result: {"parts":["A","B","C"]}
    /// 
    /// // Indented JSON
    /// var opts = new JsonSerializerOptions { WriteIndented = true };
    /// var pretty = StrBuilderSerializer.Serialize(builder, opts);
    /// // Result:
    /// // {
    /// //   "parts": [
    /// //     "A",
    /// //     "B",
    /// //     "C"
    /// //   ]
    /// // }
    /// </code>
    /// </example>
    /// <seealso cref="StrBuilder.ToJson(JsonSerializerOptions)"/>
    public static string Serialize(StrBuilder builder, JsonSerializerOptions? opts = null)
    {
        var options = opts ?? DefaultOptions;
        return JsonSerializer.Serialize(
            new { parts = builder.ToString().Split(", ") },
            options
        );
    }
}
