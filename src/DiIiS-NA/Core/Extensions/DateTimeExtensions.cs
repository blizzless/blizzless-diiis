//Blizzless Project 2022
using System;

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
	}
}
