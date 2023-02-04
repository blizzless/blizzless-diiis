using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

namespace DiIiS_NA.Core.Logging;

public class AnsiTarget : LogTarget
{
    private readonly Table _table;
    private static CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();
    private static bool _shutdown = true;
    
    public AnsiTarget(Logger.Level minLevel, Logger.Level maxLevel, bool includeTimeStamps)
    {
        _shutdown = false;
        MinimumLevel = minLevel;
        MaximumLevel = maxLevel;
        IncludeTimeStamps = includeTimeStamps;

        _table = new Table().Expand().ShowFooters().ShowHeaders().Border(TableBorder.Rounded);

        if (IncludeTimeStamps)
            _table.AddColumn("Time");
        _table
            .AddColumn("Level")
            .AddColumn("Logger")
            .AddColumn("Message")
            .AddColumn("Error");

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
    
    public static void StopIfRunning()
    {
        CancellationTokenSource.Cancel();
        while(!_shutdown)
            Thread.Sleep(100);
        Thread.Sleep(1000);
        AnsiConsole.Clear();
        AnsiConsole.Cursor.SetPosition(0,0);
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
        const string mpq = "underline deepskyblue2";
        const string sql = "underline dodgerblue1";
        const string discord = "underline blue";
        const string notNull = "green";
        const string @null = "underline red";
        const string unkNull = "underline yellow";
        return text
            .Replace("Blizzless", $"[{blizz}]Blizz[/][{less}]less[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace("Diablo III", $"[{diablo}]Diablo[/] [{d3}]III[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace(@"D3\.", $"[{diablo}]D[/][{d3}]3[/]", StringComparison.CurrentCultureIgnoreCase) //D3.*
            
            .Replace("MPQ", $"[{mpq}]MPQ[/]")
            .Replace("SQL", $"[{sql}]SQL[/]")
            .Replace("Discord", $"[{discord}]Discord[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace("not null", $"[{notNull}]is not null[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace("!= null", $"[{notNull}]!= null[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace("is null", $"[{@null}]is null[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace("= null", $"[{@null}]= null[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace("null", $"[{unkNull}]null[/]", StringComparison.CurrentCultureIgnoreCase);
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
    string Cleanup(string x) => Beautify(x.Replace("[", "[[").Replace("]", "]]").Replace("$[[/]]$", "[/]").Replace("$[[", "[").Replace("]]$", "]"));

    public override void LogMessage(Logger.Level level, string logger, string message)
    {
        if (CancellationTokenSource.IsCancellationRequested)
            return;
        try
        {
            if (IncludeTimeStamps)
                _table.AddRow(
                    new Markup(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), GetStyleByLevel(level)),
                    new Markup(level.ToString(), GetStyleByLevel(level)).RightJustified(),
                    new Markup(logger, GetStyleByLevel(level)).LeftJustified(),
                    new Markup(Cleanup(message), GetStyleByLevel(level)).LeftJustified(),
                    new Markup("", new Style(foreground: Color.Green3_1)).Centered());
            else
                _table.AddRow(
                    new Markup(level.ToString()).RightJustified(),
                    new Markup(logger, GetStyleByLevel(level)).LeftJustified(),
                    new Markup(Cleanup(message), GetStyleByLevel(level)).LeftJustified(),
                    new Markup("", new Style(foreground: Color.Green3_1)).Centered());
        }
        catch (Exception ex)
        {
            Debugger.Break();
        }
    }

    public override void LogException(Logger.Level level, string logger, string message, Exception exception)
    {
        if (CancellationTokenSource.IsCancellationRequested)
            return;
        try
        {
            if (IncludeTimeStamps)
                _table.AddRow(
                    new Markup(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), GetStyleByLevel(level)),
                    new Markup(level.ToString(), GetStyleByLevel(level)).RightJustified(),
                    new Markup(logger, GetStyleByLevel(level)).LeftJustified(),
                    new Markup(Cleanup(message), GetStyleByLevel(level)).LeftJustified(),
                    new Markup(
                        $"[underline red3_1 on white]{exception.GetType().Name}[/]\n" + Cleanup(exception.Message),
                        new Style(foreground: Color.Red3_1)).Centered());
            else
                _table.AddRow(
                    new Markup(level.ToString()).RightJustified(),
                    new Markup(logger, GetStyleByLevel(level)).LeftJustified(),
                    new Markup(message, GetStyleByLevel(level)).LeftJustified(),
                    new Markup(
                        $"[underline red3_1 on white]{exception.GetType().Name}[/]\n" + Cleanup(exception.Message),
                        new Style(foreground: Color.Red3_1)).Centered());
        }
        catch (Exception ex)
        {
            Debugger.Break();
        }
}

    private static Style GetStyleByLevel(Logger.Level level)
    {
        return level switch
        {
            Logger.Level.RenameAccountLog => new Style(Color.DarkSlateGray3),//
            Logger.Level.ChatMessage => new Style(Color.DarkSlateGray2),//
            Logger.Level.Debug => new Style(Color.Olive),//
            Logger.Level.MethodTrace => new Style(Color.DarkOliveGreen1_1),//
            Logger.Level.Trace => new Style(Color.BlueViolet),//
            Logger.Level.Info => new Style(Color.White),
            Logger.Level.Success => new Style(Color.Green3_1),
            Logger.Level.Warn => new Style(Color.Yellow),//
            Logger.Level.Error => new Style(Color.IndianRed1),//
            Logger.Level.Fatal => new Style(Color.Red3_1),//
            Logger.Level.PacketDump => new Style(Color.Maroon),//
            _ => new Style(Color.White)
        };
    }
}