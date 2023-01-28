using System;
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
            
        });
    }
    
    public static void StopIfRunning()
    {
        CancellationTokenSource.Cancel();
    }
    
    
    public static string Filter(string text)
    {
        return text
            .Replace("Blizzless", "[dodgerblue1]Blizz[/][deepskyblue2]less[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace("Diablo III", "[red3_1]Diablo[/] [red]III[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace("MPQ", "[underline yellow4]MPQ[/]")
            .Replace("Discord", "[blue]Discord[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace("not null", "[green]is not null[/]", StringComparison.CurrentCultureIgnoreCase)
            .Replace("null", "[red]is null[/]", StringComparison.CurrentCultureIgnoreCase);
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
    string Cleanup(string x) => Filter(x.Replace("[", "[[").Replace("]", "]]").Replace("$[[/]]$", "[/]").Replace("$[[", "[").Replace("]]$", "]"));

    public override void LogMessage(Logger.Level level, string logger, string message)
    {
        if (CancellationTokenSource.IsCancellationRequested)
            return;
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

    public override void LogException(Logger.Level level, string logger, string message, Exception exception)
    {
        if (CancellationTokenSource.IsCancellationRequested)
            return;
        if (IncludeTimeStamps)
            _table.AddRow(
                new Markup(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), GetStyleByLevel(level)), 
                new Markup(level.ToString(), GetStyleByLevel(level)).RightJustified(), 
                new Markup(logger, GetStyleByLevel(level)).LeftJustified(), 
                new Markup(Cleanup(message), GetStyleByLevel(level)).LeftJustified(), 
                new Markup($"[underline red3_1 on white]{exception.GetType().Name}[/]\n" + Cleanup(exception.Message), new Style(foreground: Color.Red3_1)).Centered());
        else
            _table.AddRow(
                new Markup(level.ToString()).RightJustified(), 
                new Markup(logger, GetStyleByLevel(level)).LeftJustified(), 
                new Markup(message, GetStyleByLevel(level)).LeftJustified(),
                new Markup($"[underline red3_1 on white]{exception.GetType().Name}[/]\n" + Cleanup(exception.Message), new Style(foreground: Color.Red3_1)).Centered());
    }

    private static Style GetStyleByLevel(Logger.Level level)
    {
        return level switch
        {
            Logger.Level.RenameAccountLog => new Style(Color.DarkSlateGray3),//
            Logger.Level.ChatMessage => new Style(Color.DarkSlateGray2),//
            Logger.Level.Debug => new Style(Color.Olive),//
            Logger.Level.MethodTrace => new Style(Color.Purple),//
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