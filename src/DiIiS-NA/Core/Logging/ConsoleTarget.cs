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
			var timeStamp = IncludeTimeStamps ? "[[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + "]] " : "";
			AnsiConsole.MarkupLine($"{timeStamp}{SetColor(level, true)}[[{level.ToString(),8}]][/] {SetColor(level)}[[{Cleanup(logger),20}]]: {Cleanup(message)}[/]");
		}

		/// <param name="level">Log level.</param>
		/// <param name="logger">Source of the log message.</param>
		/// <param name="message">Log message.</param>
		/// <param name="exception">Exception to be included with log message.</param>
		public override void LogException(Logger.Level level, string logger, string message, Exception exception)
		{
			var timeStamp = IncludeTimeStamps ? "[[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + "]] " : "";
			
			AnsiConsole.MarkupLine(
				$"{timeStamp}{SetColor(level, true)}[[{level.ToString(),8}]][/] {SetColor(level)}[[{Cleanup(logger),20}]]: {Cleanup(message)}[/] - [underline red on white][[{exception.GetType().Name}]][/][red] {Cleanup(exception.Message)}[/]");
		}
		
		
		/// <summary>
		/// Performs a cleanup on the target.
		/// All [ becomes [[, and ] becomes ]] (for ignoring ANSI codes)
		/// To use a style, use $[..]$abc$[/]$.
		/// Example:
		/// Logger.Warn("This is a $[red]$red$[/]$ message");
		/// instead of
		/// Logger.Warn("This is a [red]red[/] message");
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		string Cleanup(string x) => AnsiTarget.Filter(x.Replace("[", "[[").Replace("]", "]]").Replace("$[[/]]$", "[/]").Replace("$[[", "[").Replace("]]$", "]"));

		/// <param name="level"></param>
		private static string SetColor(Logger.Level level, bool withBackground = false)
		{
			string postfix = withBackground ? " on grey19" : "";
			switch (level)
			{
				case Logger.Level.PacketDump:
					return $"[grey30{postfix}]";
				case Logger.Level.Debug:
					return $"[grey39{postfix}]";
				case Logger.Level.Trace:
					return $"[purple{postfix}]";
				case Logger.Level.Info:
					return $"[white{postfix}]";
				case Logger.Level.Success:
					return $"[green3_1{postfix}]";
				case Logger.Level.Warn:
					return $"[darkorange3_1{postfix}]";
				case Logger.Level.Error:
					return "[indianred1{postfix}]";
				case Logger.Level.Fatal:
					return $"[red3{postfix}]";
				default:
					return $"[navajowhite3{postfix}]";
			}
		}
	}
}
