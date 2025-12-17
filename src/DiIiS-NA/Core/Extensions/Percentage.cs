using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.Core.Extensions
{
    /// <summary>
    /// Represents a percentage value with utility methods for percentage-based calculations.
    /// This struct provides functionality for computing percentages of values, chance calculations, and value formatting.
    /// </summary>
    internal readonly struct Percentage
    {
        private readonly double _value;
        private readonly double _onePercent;
        private static readonly Random _random = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Percentage"/> struct with the specified percentage value.
        /// </summary>
        /// <param name="value">The percentage value (e.g., 25 for 25%).</param>
        public Percentage(double value)
        {
            _value = value;
            _onePercent = value / 100.0;
        }

        /// <summary>
        /// Gets the percentage value.
        /// </summary>
        public double Value => _value;

        /// <summary>
        /// Calculates the percentage of the specified integer total.
        /// </summary>
        /// <param name="total">The total value to calculate the percentage of.</param>
        /// <returns>The calculated percentage value, rounded appropriately.</returns>
        /// <example>
        /// <code>
        /// var percentage = new Percentage(25);
        /// var result = percentage.Of(100); // Returns 25
        /// </code>
        /// </example>
        public double Of(int total) => Of((double)total);

        /// <summary>
        /// Calculates the percentage of the specified double total.
        /// </summary>
        /// <param name="total">The total value to calculate the percentage of.</param>
        /// <returns>The calculated percentage value, rounded appropriately using ceiling for values greater than the percentage and floor otherwise.</returns>
        public double Of(double total)
        {
            var record = _onePercent * total;
            if (record > _value)
                return Math.Ceiling(record);
            else
                return Math.Floor(record);
        }

        /// <summary>
        /// Determines whether a chance event occurs based on the percentage value with a default denominator of 50.
        /// </summary>
        /// <returns>True if the random roll is within the percentage threshold; otherwise, false.</returns>
        /// <remarks>
        /// This method uses a default denominator of 50, making the chance calculation equivalent to (percentage / 50).
        /// For example, a 25% chance with default denominator returns true approximately 50% of the time.
        /// </remarks>
        public bool Chance() => Chance(50);

        /// <summary>
        /// Determines whether a chance event occurs based on the percentage value with a specified denominator.
        /// </summary>
        /// <param name="outOf">The denominator for the chance calculation (e.g., 100 for a 1-in-100 roll).</param>
        /// <returns>True if the random roll is less than or equal to the percentage threshold; otherwise, false.</returns>
        /// <example>
        /// <code>
        /// var percentage = new Percentage(25);
        /// bool success = percentage.Chance(100); // 25% chance of returning true
        /// </code>
        /// </example>
        public bool Chance(int outOf)
        {
            lock (_random)
            {
                var roll = _random.Next(1, outOf + 1);
                return roll <= _value;
            }
        }

        /// <summary>
        /// Calculates the remaining percentage (100% - current percentage).
        /// </summary>
        /// <returns>The remaining percentage as a new <see cref="Percentage"/> instance.</returns>
        /// <example>
        /// <code>
        /// var percentage = new Percentage(30);
        /// var remaining = percentage.Remaining(); // Returns a Percentage with value 70
        /// </code>
        /// </example>
        public Percentage Remaining() => new(100.0 - _value);

        /// <summary>
        /// Determines whether the current percentage value represents a chance with a 50% probability using a fair coin flip.
        /// </summary>
        /// <returns>True with approximately 50% probability; otherwise, false.</returns>
        public bool CoinFlip() => Chance(2);

        /// <summary>
        /// Calculates a percentage value based on the ratio of part to total.
        /// </summary>
        /// <param name="part">The partial value.</param>
        /// <param name="total">The total value.</param>
        /// <returns>A new <see cref="Percentage"/> representing the percentage of part relative to total.</returns>
        /// <example>
        /// <code>
        /// var percentage = Percentage.From(25, 100); // Returns a Percentage with value 25
        /// </code>
        /// </example>
        public static Percentage From(double part, double total)
        {
            if (total == 0)
                return new Percentage(0);
            return new Percentage((part / total) * 100.0);
        }

        /// <summary>
        /// Determines whether the current percentage is greater than or equal to a specified percentage value.
        /// </summary>
        /// <param name="other">The percentage value to compare.</param>
        /// <returns>True if the current percentage is greater than or equal to the specified value; otherwise, false.</returns>
        public bool IsGreaterOrEqual(double other) => _value >= other;

        /// <summary>
        /// Determines whether the current percentage is less than or equal to a specified percentage value.
        /// </summary>
        /// <param name="other">The percentage value to compare.</param>
        /// <returns>True if the current percentage is less than or equal to the specified value; otherwise, false.</returns>
        public bool IsLessOrEqual(double other) => _value <= other;

        /// <summary>
        /// Determines whether the current percentage is between two specified percentage values (inclusive).
        /// </summary>
        /// <param name="min">The minimum percentage value.</param>
        /// <param name="max">The maximum percentage value.</param>
        /// <returns>True if the current percentage is between the specified range; otherwise, false.</returns>
        public bool IsBetween(double min, double max) => _value >= min && _value <= max;

        /// <summary>
        /// Applies the current percentage as a multiplier to the specified value (e.g., a 50% percentage applied to 100 returns 50).
        /// </summary>
        /// <param name="value">The value to apply the percentage multiplier to.</param>
        /// <returns>The value multiplied by the percentage as a decimal (e.g., percentage / 100).</returns>
        /// <example>
        /// <code>
        /// var percentage = new Percentage(50);
        /// var result = percentage.Apply(100); // Returns 50
        /// </code>
        /// </example>
        public double Apply(double value) => value * _onePercent;

        /// <summary>
        /// Applies the current percentage as a multiplier to the specified integer value.
        /// </summary>
        /// <param name="value">The value to apply the percentage multiplier to.</param>
        /// <returns>The value multiplied by the percentage as a decimal.</returns>
        public double Apply(int value) => Apply((double)value);

        /// <summary>
        /// Gets a string representation of the percentage value in the format "X%".
        /// </summary>
        /// <returns>A string representation of the percentage (e.g., "25%").</returns>
        public override string ToString() => $"{_value}%";

        /// <summary>
        /// Gets a formatted string representation of the percentage value with specified precision.
        /// </summary>
        /// <param name="decimals">The number of decimal places to include.</param>
        /// <returns>A formatted string representation of the percentage (e.g., "25.50%").</returns>
        public string ToString(int decimals) => $"{_value.ToString($"F{decimals}")}%";
    }
}
