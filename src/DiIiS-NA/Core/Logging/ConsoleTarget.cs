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
		public ConsoleTarget(Logger.Level minLevel, Logger.Level maxLevel, bool includeTimeStamps, string timeStampFormat)
		{
			MinimumLevel = minLevel;
			MaximumLevel = maxLevel;
			IncludeTimeStamps = includeTimeStamps;
			TimeStampFormat = timeStampFormat;
		}
		
		
		/// <param name="level">Log level.</param>
		/// <param name="logger">Source of the log message.</param>
		/// <param name="message">Log message.</param>
		public override void LogMessage(Logger.Level level, string logger, string message)
		{
			var timeStamp = IncludeTimeStamps ? "[[" + DateTime.Now.ToString(TimeStampFormat) + "]] " : "";
			AnsiConsole.MarkupLine($"{timeStamp}{SetColor(level, true)}[[{level.ToString(),8}]][/] {SetColor(level)}[[{Cleanup(logger),20}]]: {AnsiTarget.Cleanup(message)}[/]");
		}

		/// <param name="level">Log level.</param>
		/// <param name="logger">Source of the log message.</param>
		/// <param name="message">Log message.</param>
		/// <param name="exception">Exception to be included with log message.</param>
		public override void LogException(Logger.Level level, string logger, string message, Exception exception)
		{
			var timeStamp = IncludeTimeStamps ? "[[" + DateTime.Now.ToString(TimeStampFormat) + "]] " : "";
			
			AnsiConsole.MarkupLine(
				$"{timeStamp}{SetColor(level, true)}[[{level.ToString(),8}]][/] {SetColor(level)}[[{Cleanup(logger),20}]]: {Cleanup(message)}[/] - [underline red on white][[{exception.GetType().Name}]][/][red] {Cleanup(exception.Message)}[/]");
			AnsiConsole.WriteException(exception);
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
		string Cleanup(string x) => AnsiTarget.Beautify(x.Replace("[", "[[").Replace("]", "]]").Replace("$[[/]]$", "[/]").Replace("$[[", "[").Replace("]]$", "]"));
		

		/// <param name="level"></param>
		private static string SetColor(Logger.Level level, bool withBackground = false)
        {
            string postfix = withBackground ? " on grey19" : "";
            return level switch
            {
                Logger.Level.PacketDump => $"[grey30{postfix}]",
                Logger.Level.Debug => $"[grey39{postfix}]",
                Logger.Level.Trace => $"[purple{postfix}]",
                Logger.Level.Info => $"[white{postfix}]",
                Logger.Level.QuestLog => $"[darkseagreen2{postfix}]",
                Logger.Level.Success => $"[green3_1{postfix}]",
                Logger.Level.Warn => $"[darkorange3_1{postfix}]",
                Logger.Level.Error => $"[indianred1{postfix}]",
                Logger.Level.Fatal => $"[red3{postfix}]",
                _ => $"[grey54{postfix}]"
            };
        }
	}
}
