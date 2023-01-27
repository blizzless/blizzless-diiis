using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace DiIiS_NA.Core.Logging
{
	public class ConsoleTarget : LogTarget
	{
		/// <param name="minLevel">Minimum level of messages to emit</param>
		/// <param name="maxLevel">Maximum level of messages to emit</param>
		/// <param name="includeTimeStamps">Include timestamps in log?</param>
		public ConsoleTarget(Logger.Level minLevel, Logger.Level maxLevel, bool includeTimeStamps)
		{
			MinimumLevel = minLevel;
			MaximumLevel = maxLevel;
			IncludeTimeStamps = includeTimeStamps;
		}
		
		/// <param name="level">Log level.</param>
		/// <param name="logger">Source of the log message.</param>
		/// <param name="message">Log message.</param>
		public override void LogMessage(Logger.Level level, string logger, string message)
		{
			var timeStamp = IncludeTimeStamps ? "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "] " : "";
			SetConsoleForegroundColor(level);
			Console.WriteLine($"{timeStamp}[{level.ToString(),8}] [{logger,20}]: {message}");
		}

		/// <param name="level">Log level.</param>
		/// <param name="logger">Source of the log message.</param>
		/// <param name="message">Log message.</param>
		/// <param name="exception">Exception to be included with log message.</param>
		public override void LogException(Logger.Level level, string logger, string message, Exception exception)
		{
			var timeStamp = IncludeTimeStamps ? "[" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "] " : "";
			SetConsoleForegroundColor(level);
			Console.WriteLine(
				$"{timeStamp}[{level.ToString(),8}] [{logger,20}]: {message} - [Exception] {exception}");
		}

		/// <param name="level"></param>
		private static void SetConsoleForegroundColor(Logger.Level level)
		{
			switch (level)
			{
				case Logger.Level.PacketDump:
					Console.ForegroundColor = ConsoleColor.DarkGray;
					break;
				case Logger.Level.Debug:
					Console.ForegroundColor = ConsoleColor.DarkCyan;
					break;
				case Logger.Level.Trace:
					Console.ForegroundColor = ConsoleColor.Cyan;
					break;
				case Logger.Level.Info:
					Console.ForegroundColor = ConsoleColor.White;
					break;
				case Logger.Level.Success:
					Console.ForegroundColor = ConsoleColor.DarkGreen;
					break;
				case Logger.Level.Warn:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case Logger.Level.Error:
					Console.ForegroundColor = ConsoleColor.Magenta;
					break;
				case Logger.Level.Fatal:
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				default:
					break;
			}
		}
	}
}
