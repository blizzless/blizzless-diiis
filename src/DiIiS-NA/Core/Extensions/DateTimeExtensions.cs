using System;
using System.Collections.Generic;

namespace DiIiS_NA.Core.Extensions
{
	public static class DateTimeExtensions
	{
		public static uint ToUnixTime(this DateTime time)
		{
			return (uint)((time.ToUniversalTime().Ticks - 621355968000000000L) / 10000000L);
		}

		public static ulong ToExtendedEpoch(this DateTime time)
		{
			return (ulong)((time.ToUniversalTime().Ticks - 621355968000000000L) / 10L);
		}

		public static ulong ToBlizzardEpoch(this DateTime time)
		{
			TimeSpan diff = time.ToUniversalTime() - DateTime.UnixEpoch;
			return (ulong)((diff.TotalSeconds - 946695547L) * 1000000000L);
		}
		
		
        /// <summary>
        /// Transforms a timespan to a readable text.
        /// E.g.:
        /// 1 day, 2 hours, 3 minutes and 4 seconds
        /// 5 hours, 6 minutes and 7 seconds
        ///
        /// If over certain threshold (millennium or more) it will only return the number of days.
        /// </summary>
        /// <param name="span">The timespan to be converted</param>
        /// <returns>The readable text</returns>
        public static string ToText(this TimeSpan span)
        {
            List<string> parts = new();

            // if days are divided by 365, we have years, otherwise we have months or days
            if (span.Days / 365 > 0)
            {
                // if days are divided by 365, we have years
                parts.Add($"{((double)span.Days / 365):F} year{(span.Days / 365 > 1 ? "s" : "")}");
                // get months from the remaining days
                int months = span.Days % 365 / 30;
                if (months > 0)
                    parts.Add($"{months} month{(months > 1 ? "s" : "")}");
                
                // get remaining days
                int days = span.Days % 365 % 30;
                if (days > 0)
                    parts.Add($"{days} day{(days > 1 ? "s" : "")}");
            }
            else if (span.Days / 30 > 0)
            {
                // if days are divided by 30, we have months
                parts.Add($"{((double)span.Days / 30):F} month{(span.Days / 30 > 1 ? "s" : "")}");
                // get remaining days
                int days = span.Days % 30;
                if (days > 0)
                    parts.Add($"{days} day{(days > 1 ? "s" : "")}");
            }
            else if (span.Days > 0)
                // if days are not divided by 365 or 30, we have days
                parts.Add( $"{span.Days} day{(span.Days > 1 ? "s" : "")}");
            if (span.Hours > 0)
                parts.Add($"{span.Hours} hour{(span.Hours > 1 ? "s" : "")}");
            if (span.Minutes > 0)
                parts.Add($"{span.Minutes} minute{(span.Minutes > 1 ? "s" : "")}");
            if (span.Seconds > 0)
                parts.Add($"{span.Seconds} second{(span.Seconds > 1 ? "s" : "")}");

            var result = parts.ToArray();
            return string.Join(", ", result[..^1]) + (result.Length > 1 ? " and " : "") + parts[^1];
        }

        public static string ToSmallText(this TimeSpan span)
        {
            List<string> parts = new();
            if (span.Days / 365 > 0)
            {
                parts.Add($"{((double)span.Days / 365):F}y");
                int months = span.Days % 365 / 30;
                if (months > 0)
                    parts.Add($"{months}m");
                int days = span.Days % 365 % 30;
                if (days > 0)
                    parts.Add($"{days}d");
            }
            else if (span.Days / 30 > 0)
            {
                parts.Add($"{((double)span.Days / 30):F}m");
                int days = span.Days % 30;
                if (days > 0)
                    parts.Add($"{days}d");
            }
            else if (span.Days > 0)
                parts.Add($"{span.Days}d");
            if (span.Hours > 0)
                parts.Add($"{span.Hours}h");
            if (span.Minutes > 0)
                parts.Add($"{span.Minutes}m");
            if (span.Seconds > 0)
                parts.Add($"{span.Seconds}s");
            return string.Join(" ", parts);
        }
	}
}
