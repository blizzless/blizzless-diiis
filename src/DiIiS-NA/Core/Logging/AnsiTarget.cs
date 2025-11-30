using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace DiIiS_NA.Core.Logging;

public class AnsiTarget : LogTarget
{
    private readonly Table _table;
    private static CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
    private static bool _shutdown = true;
    
    public AnsiTarget(Logger.Level minLevel, Logger.Level maxLevel, bool includeTimeStamps, string timeStampFormat)
    {
        _shutdown = false;
        MinimumLevel = minLevel;
        MaximumLevel = maxLevel;
        IncludeTimeStamps = includeTimeStamps;
        TimeStampFormat = timeStampFormat;

        _table = new Table().Expand().ShowFooters().ShowHeaders().Border(TableBorder.Rounded);

        if (IncludeTimeStamps)
            _table.AddColumn("Time").Centered();
        _table
            .AddColumn("Level").RightAligned()
            .AddColumn("Message").Centered()
            .AddColumn("Logger").LeftAligned()
            .AddColumn("Error").RightAligned();

        AnsiConsole.Live(_table).StartAsync(async ctx =>
        {
            int lastCount = 0;
            int consoleSize = 0;
            while (!CancellationTokenSource.Token.IsCancellationRequested)
            {
                int currentConsoleSize = 0;
                try
                {
                    currentConsoleSize = Console.WindowHeight * Console.WindowWidth;
                }
                catch {}
                if (lastCount != _table.Rows.Count || consoleSize != currentConsoleSize)
                {
                    lastCount = _table.Rows.Count;
                    consoleSize = currentConsoleSize;
                    ctx.UpdateTarget(_table);
                }
                await Task.Delay(100);
            }

            _shutdown = true;
        });
    }
    
    public static void StopIfRunning(bool clear = false)
    {
        CancellationTokenSource.Cancel();
        while(!_shutdown)
            Thread.Sleep(100);
        Thread.Sleep(1000);
        if (clear)
        {
            AnsiConsole.Clear();
            AnsiConsole.Cursor.SetPosition(0, 0);
        }
    }
    
    /// <summary>
    /// Logging keywords to beautify the output.
    /// It's ugly, I know.
    /// Changes are welcome - @iamdroppy
    /// </summary>
    /// <param name="text">Text to "beautify"</param>
    /// <returns>Replaced with color changes</returns>
    public static string Beautify(string text)
    {
        const string blizz = "dodgerblue1";
        const string less = "deepskyblue2";
        const string diablo = "red3_1";
        const string d3 = "red";
        const string sql = "underline dodgerblue1";
        const string discord = "underline blue";
        const string notNull = "green";
        const string @null = "underline red";
        const string unkNull = "underline yellow";
        return text
            .Replace("Blizzless", $"[{blizz}]Blizz[/][{less}]less[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace("Diablo III", $"[{diablo}]Diablo[/] [{d3}]III[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace(@"D3\.", $"[{diablo}]D[/][{d3}]3[/]", StringComparison.CurrentCultureIgnoreCase) //D3.*
            
            .Replace("SQL", $"[{sql}]SQL[/]")
            .Replace("Discord", $"[{discord}]Discord[/]", StringComparison.CurrentCultureIgnoreCase)
            
            .Replace("null", $"[{unkNull}]null[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace($"not [{unkNull}]null[/]", $"[{notNull}]is not null[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace($"is [{unkNull}]null[/]", $"[{@null}]is null[/]", StringComparison.CurrentCultureIgnoreCase);
    }

    
    private static Dictionary<string, string> _replacements = new () 
    {
        ["["] = "[[",
        ["]"] = "]]",
        ["$[[/]]$"] = "[/]",
        ["$[["] = "[",
        ["]]$"] = "]"
    };
    
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
    public static string Cleanup(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return Beautify(_replacements.Aggregate(input, (current, replacement) => current.Replace(replacement.Key, replacement.Value)));
    }
    public override void LogMessage(Logger.Level level, string logger, string message)
    {
        if (CancellationTokenSource.IsCancellationRequested) return;

        try
        {
            AddRow(level, logger, message, "");
        }
        catch (Exception ex)
        {
            AddRow(level, logger, Cleanup(StripMarkup(message)), ex.Message, true);
        }
    }

    public override void LogException(Logger.Level level, string logger, string message, Exception exception)
    {
        if (CancellationTokenSource.IsCancellationRequested) return;

        try
        {
            AddRow(level, logger, message, $"[underline red3_1 on white]{exception.GetType().Name}[/]\n" + Cleanup(exception.Message), exFormat: true);
        }
        catch (Exception ex)
        {
            AddRow(level, logger, Cleanup(StripMarkup(message)), ex.Message, true);
        }
    }

    private void AddRow(Logger.Level level, string logger, string message, string exMessage, bool isError = false,
        bool exFormat = false)
    {
        Style messageStyle = GetStyleByLevel(level);
        Style exStyle = exFormat ? new Style(foreground: Color.Red3_1) : new Style(foreground: Color.Green3_1);
        var colTimestamp = new Markup(DateTime.Now.ToString(TimeStampFormat), messageStyle).Centered();
        var colLevel = new Markup(level.ToString(), messageStyle).RightJustified();
        var colMessage = new Markup(Cleanup(message), messageStyle).Centered();
        var colLogger = new Markup(logger, new Style(messageStyle.Foreground, messageStyle.Background, messageStyle.Decoration
        #if DEBUG
        //, link = ...
        #endif
        )).LeftJustified();
        var colError = new Markup(isError ? exMessage : "", exStyle).RightJustified();
        if (IncludeTimeStamps) _table.AddRow(colTimestamp, colLevel, colMessage, colLogger, colError);
        else _table.AddRow(colLevel, colMessage, colLogger, colError);
    }

    private string StripMarkup(string message)
    {
        var regex = new Regex(@"\$\[.*?\]\$");
        var matches = regex.Matches(message);
        foreach (Match match in matches)
        {
            message = message.Replace(match.Value, "");
        }

        return message;
    }

    private static Style GetStyleByLevel(Logger.Level level)
    {
        return level switch
        {
            Logger.Level.RenameAccountLog => new Style(Color.Gold1),//
            Logger.Level.ChatMessage => new Style(Color.Plum2),//
            Logger.Level.Debug => new Style(Color.Grey62),//
            Logger.Level.MethodTrace => new Style(Color.Grey74, decoration: Decoration.Dim | Decoration.Italic),//
            Logger.Level.Trace => new Style(Color.Grey82),//
            Logger.Level.Info => new Style(Color.SteelBlue),
            Logger.Level.Success => new Style(Color.DarkOliveGreen3_2),
            Logger.Level.Warn => new Style(Color.DarkOrange),//
            Logger.Level.Error => new Style(Color.IndianRed1),//
            Logger.Level.Fatal => new Style(Color.Red3_1),//
            Logger.Level.QuestInfo => new Style(Color.Plum2),
            Logger.Level.QuestStep => new Style(Color.Plum3, decoration: Decoration.Dim),
            Logger.Level.PacketDump => new Style(Color.Maroon),//
            _ => new Style(Color.White)
        };
    }
}

public static class AnsiTargetExtensions
{
    public static string SafeAnsi(this string text)
    {
        return text.Replace("$[", "").Replace("]$", "").Replace("[", "[[").Replace("]", "]]");
    }

    public static string StyleAnsi(this object obj, string style) =>
        $"$[{style}]$" + obj.ToString().EscapeMarkup() + "$[/]$";
}