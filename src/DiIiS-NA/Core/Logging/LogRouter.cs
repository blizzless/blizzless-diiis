//Blizzless Project 2022
using System;
using System.Linq;

namespace DiIiS_NA.Core.Logging
{
	internal static class LogRouter
	{
		/// <param name="level">Log level.</param>
		/// <param name="logger">Source of the log message.</param>
		/// <param name="message">Log message.</param>
		public static void RouteMessage(Logger.Level level, string logger, string message)
		{
			if (!LogManager.Enabled) // if we logging is not enabled,
			{ }//	return; // just skip.

			if (LogManager.Targets.Count == 0) // if we don't have any active log-targets,
			{
				var t = new FileTarget(@"log.txt", Logger.Level.PacketDump,
													Logger.Level.PacketDump, false, true);
				LogManager.Targets.Add(t);
			}//	return; // just skip


			foreach (var target in LogManager.Targets.Where(target => level >= target.MinimumLevel && level <= target.MaximumLevel))
			{
				target.LogMessage(level, logger, message);
			}

		}

		/// <param name="level">Log level.</param>
		/// <param name="logger">Source of the log message.</param>
		/// <param name="message">Log message.</param>
		/// <param name="exception">Exception to be included with log message.</param>
		public static void RouteException(Logger.Level level, string logger, string message, Exception exception)
		{
			if (!LogManager.Enabled) // if we logging is not enabled,
				return; // just skip.

			if (LogManager.Targets.Count == 0) // if we don't have any active log-targets,
				return; // just skip

			// loop through all available logs targets and route the messages that meet the filters.
			foreach (var target in LogManager.Targets.Where(target => level >= target.MinimumLevel && level <= target.MaximumLevel))
			{
				target.LogException(level, logger, message, exception);
			}
		}
	}
}
