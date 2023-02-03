//Blizzless Project 2022
using DiIiS_NA.Core.Discord.Modules;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.GameServer.AchievementSystem;
using DiIiS_NA.GameServer.CommandManager;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.LoginServer;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Base;
using DiIiS_NA.LoginServer.Battle;
using DiIiS_NA.LoginServer.GuildSystem;
using DiIiS_NA.LoginServer.Toons;
using DiIiS_NA.REST;
using DiIiS_NA.REST.Manager;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Npgsql;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using DiIiS_NA.Core.Extensions;
using Spectre.Console;
using Environment = System.Environment;

namespace DiIiS_NA
{
    public enum TypeBuildEnum
    {
        Alpha,
        Beta,
        Test,
        Release
    }
    class Program
    {
        private static readonly Logger Logger = LogManager.CreateLogger("BZ.Net");
        public static readonly DateTime StartupTime = DateTime.Now;
        public static BattleBackend BattleBackend { get; set; }
        public bool GameServersAvailable = true;

        public const int MaxLevel = 70;

        public static GameServer.ClientSystem.GameServer GameServer;
        public static Watchdog Watchdog;

        public static Thread GameServerThread;
        public static Thread WatchdogThread;

        public static string LoginServerIp = DiIiS_NA.LoginServer.Config.Instance.BindIP;
        public static string GameServerIp = DiIiS_NA.GameServer.Config.Instance.BindIP;
        public static string RestServerIp = REST.Config.Instance.IP;
        public static string PublicGameServerIp = DiIiS_NA.GameServer.NATConfig.Instance.PublicIP;

        public static int Build => 30;
        public static int Stage => 2;
        public static TypeBuildEnum TypeBuild => TypeBuildEnum.Beta;
        private static bool DiabloCoreEnabled = DiIiS_NA.GameServer.Config.Instance.CoreActive;
        
    static async Task LoginServer()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            string name = $"Blizzless: Build {Build}, Stage: {Stage} - {TypeBuild}";
            SetTitle(name);
            Maximize();
            AnsiConsole.Write(new Rule("[dodgerblue1]Blizz[/][deepskyblue2]less[/]").RuleStyle("steelblue1"));
            AnsiConsole.Write(new Rule($"[dodgerblue3]Build [/][deepskyblue3]{Build}[/]").RightJustified().RuleStyle("steelblue1_1"));
            AnsiConsole.Write(new Rule($"[dodgerblue3]Stage [/][deepskyblue3]{Stage}[/]").RightJustified().RuleStyle("steelblue1_1"));
            AnsiConsole.Write(new Rule($"[deepskyblue3]{TypeBuild}[/]").RightJustified().RuleStyle("steelblue1_1"));
            AnsiConsole.Write(new Rule($"Diablo III [red]RoS 2.7.4.84161[/] - [link=https://github.com/blizzless/blizzless-diiis]https://github.com/blizzless/blizzless-diiis[/]").RuleStyle("red"));
        
            AnsiConsole.MarkupLine("");
            Console.WriteLine();
            Console.Title = name;

            InitLoggers();
#if DEBUG
            DiabloCoreEnabled = true;
            Logger.Warn("Diablo III Core forced enable on $[underline white on olive]$ DEBUG $[/]$");
#endif
            
#pragma warning disable CS4014
            Task.Run(async () =>
#pragma warning restore CS4014
            {
                while (true)
                {
                    try
                    {
                        var uptime = (DateTime.Now - StartupTime);
                        // get total memory from process
                        var totalMemory =
                            (double)((double)Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024 / 1024);
                        // get CPU time
                        using var proc = Process.GetCurrentProcess();
                        var cpuTime = proc.TotalProcessorTime;
                        var text =
                            $"{name} | " +
                            $"{PlayerManager.OnlinePlayers.Count} onlines in {PlayerManager.OnlinePlayers.Count(s => s.InGameClient?.Player?.World != null)} worlds | " +
                            $"Memory: {totalMemory:0.000} GB | " +
                            $"CPU Time: {cpuTime.ToSmallText()} | " +
                            $"Uptime: {uptime.ToSmallText()}";
                        if (SetTitle(text))
                            await Task.Delay(1000);
                        else
                        {
                            Logger.Info(text);
                            await Task.Delay(TimeSpan.FromMinutes(1));
                        }
                    }
                    catch (Exception e)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(5));
                    }
                }
            });
            AchievementManager.Initialize();
            Core.Storage.AccountDataBase.SessionProvider.RebuildSchema();
            string GeneratePassword(int size) => new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", size)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
            void LogAccountCreated(string username, string password)
                => Logger.Success($"Created account: $[springgreen4]${username}$[/]$ with password: $[springgreen4]${password}$[/]$");
#if DEBUG
            if (!DBSessions.SessionQuery<DBAccount>().Any())
            {
                var password1 = GeneratePassword(6);
                var password2 = GeneratePassword(6);
                var password3 = GeneratePassword(6);
                var password4 = GeneratePassword(6);
                Logger.Info($"Initializing account database...");
               var account = AccountManager.CreateAccount("owner@", password1, "owner",  Account.UserLevels.Owner);
                var gameAccount = GameAccountManager.CreateGameAccount(account);
                LogAccountCreated("owner@", password1);
                var account1 = AccountManager.CreateAccount("gm@", password2, "gm", Account.UserLevels.GM);
                var gameAccount1 = GameAccountManager.CreateGameAccount(account1);
                LogAccountCreated("gm@", password2);
                var account2 = AccountManager.CreateAccount("tester@", password3, "tester", Account.UserLevels.Tester);
                var gameAccount2 = GameAccountManager.CreateGameAccount(account2);
                LogAccountCreated("tester@", password3);
                var account3 = AccountManager.CreateAccount("user@", password4, "test3", Account.UserLevels.User);
                var gameAccount3 = GameAccountManager.CreateGameAccount(account3);
                LogAccountCreated("user@", password4);
            }
#else
            if (!Enumerable.Any(DBSessions.SessionQuery<DBAccount>()))
            {
                var password = GeneratePassword(6);
                var account = AccountManager.CreateAccount("iwannatry@", password, "iwannatry", Account.UserLevels.User);
                var gameAccount = GameAccountManager.CreateGameAccount(account);
                LogAccountCreated("iwannatry@", password);
            }
#endif
            
            if (DBSessions.SessionQuery<DBAccount>().Any())
            {
                Logger.Success("Database connection has been $[underline bold italic]$successfully established$[/]$.");
            }
            //*/
            StartWatchdog();

            AccountManager.PreLoadAccounts();
            GameAccountManager.PreLoadGameAccounts();
            ToonManager.PreLoadToons();
            GuildManager.PreLoadGuilds();

            Logger.Info("Loading Diablo III - Core..."); 
            if (DiabloCoreEnabled)
            {
                if (!MPQStorage.Initialized)
                {
                    throw new Exception("MPQ archives not found...");
                }
                Logger.Info("Loaded - {0} items.", ItemGenerator.TotalItems); 
                Logger.Info("Diablo III Core - Loaded"); 
            }
            else
            {
                Logger.Fatal("Diablo III Core - Disabled");
                throw new Exception("Diablo III Core - Disabled");
            }
           
            var restSocketServer = new SocketManager<RestSession>();
            if (!restSocketServer.StartNetwork(RestServerIp, REST.Config.Instance.Port))
            {
                throw new Exception("Diablo III Core - Disabled");
            }
            Logger.Success($"$[underline darkgreen]$REST$[/]$ server started - {REST.Config.Instance.IP}:{REST.Config.Instance.Port}");
            
            //BGS
            var loginConfig = DiIiS_NA.LoginServer.Config.Instance;
            ServerBootstrap serverBootstrap = new ServerBootstrap();
            IEventLoopGroup boss = new MultithreadEventLoopGroup(1),
                            worker = new MultithreadEventLoopGroup();
            serverBootstrap.LocalAddress(loginConfig.BindIP, loginConfig.Port);
            Logger.Success(
                $"Blizzless server $[underline]$started$[/]$ - $[lightseagreen]${loginConfig.BindIP}:{loginConfig.Port}$[/]$");
            BattleBackend = new BattleBackend(loginConfig.BindIP, loginConfig.WebPort);

            //Diablo 3 Game-Server
            if (DiabloCoreEnabled) 
                StartGameServer();

            try
            {
                serverBootstrap.Group(boss, worker)
                    .Channel<TcpServerSocketChannel>()
                    .Handler(new LoggingHandler(LogLevel.DEBUG))
                    .ChildHandler(new ConnectHandler());

                IChannel boundChannel = await serverBootstrap.BindAsync(loginConfig.Port);

                Logger.Info("$[bold red3_1]$Tip:$[/]$ graceful shutdown with $[red3_1]$CTRL+C$[/]$ or $[red3_1]$!q[uit]$[/]$ or $[red3_1]$!exit$[/]$.");
                Logger.Info("$[bold red3_1]$" +
                            "Tip:$[/]$ SNO breakdown with $[red3_1]$!sno$[/]$ $[red3_1]$<fullSnoBreakdown(true:false)>$[/]$.");
                while (true)
                {
                    var line = Console.ReadLine();
                    if (line is null or "!q" or "!quit" or "!exit")
                        break;
                    if (line is "!cls" or "!clear" or "cls" or "clear")
                    {
                        AnsiConsole.Clear();
                        AnsiConsole.Cursor.SetPosition(0, 0);
                        continue;
                    }
                    if (line.ToLower().StartsWith("!sno"))
                    {
                        if (IsTargetEnabled("ansi"))
                            Console.Clear();
                        MPQStorage.Data.SnoBreakdown(line.ToLower().Equals("!sno 1") || line.ToLower().Equals("!sno true"));
                        continue;
                    }
                    CommandManager.Parse(line);
                }

                if (PlayerManager.OnlinePlayers.Count > 0)
                {
                    Logger.Info($"Server is shutting down in 1 minute, $[blue]${PlayerManager.OnlinePlayers.Count} players$[/]$ are still online.");
                    PlayerManager.SendWhisper("Server is shutting down in 1 minute.");
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
                
                Shutdown(delay: 25);
            }
            catch (Exception e)
            {
                Shutdown(e, delay: 200);
            }
            finally
            {
                await Task.WhenAll(
                    boss.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    worker.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }

        private static void Shutdown(Exception? exception = null, int delay = 200)
        {
            // if (!IsTargetEnabled("ansi"))
            {
                AnsiTarget.StopIfRunning();
                if (exception is { } ex)
                {
                    AnsiConsole.WriteLine("An unhandled exception occured at initialization. Please report this to the developers.");
                    AnsiConsole.WriteException(ex);
                }
                AnsiConsole.Progress().Start(ctx =>
                {
                    var task = ctx.AddTask("[red]Shutting down...[/]");
                    for (int i = 0; i < 100; i++)
                    {
                        task.Increment(1);
                        Thread.Sleep(delay);
                    }
                });
            }
            Environment.Exit(-1);
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static async Task Main()
        {
            try
            {
                await LoginServer();
            }
            catch (Exception ex)
            {
                Shutdown(ex);
            }
        }

        [SecurityCritical]
        [HandleProcessCorruptedStateExceptionsAttribute]
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            if (e.IsTerminating)
            {
                Shutdown(ex);
            }
            else
                Logger.ErrorException(ex, "A root error of the server was detected but was handled.");
        }

        static int TargetsEnabled(string target) => LogConfig.Instance.Targets.Count(t => t.Target.ToLower() == target && t.Enabled);
        public static bool IsTargetEnabled(string target) => TargetsEnabled(target) > 0;
        private static void InitLoggers()
        {
            LogManager.Enabled = true;
            
            if (TargetsEnabled("ansi") > 1 || (IsTargetEnabled("console") && IsTargetEnabled("ansi")))
            {
                AnsiConsole.MarkupLine("[underline red on white]Fatal:[/] [red]You can't use both ansi and console targets at the same time, nor have more than one ansi target.[/]");
                AnsiConsole.Progress().Start(ctx =>
                {
                    var sd = ctx.AddTask("[red3_1]Shutting down[/]");
                    for (int i = 0; i < 100; i++)
                    {
                        sd.Increment(1);
                        Thread.Sleep(25);
                    }
                });
                Environment.Exit(-1);
            }
            foreach (var targetConfig in LogConfig.Instance.Targets)
            {
                if (!targetConfig.Enabled)
                    continue;

                LogTarget target = null;
                switch (targetConfig.Target.ToLower())
                {
                    case "ansi":
                        target = new AnsiTarget(targetConfig.MinimumLevel, targetConfig.MaximumLevel, targetConfig.IncludeTimeStamps);
                        break;
                    case "console":
                        target = new ConsoleTarget(targetConfig.MinimumLevel, targetConfig.MaximumLevel,
                                                   targetConfig.IncludeTimeStamps);
                        break;
                    case "file":
                        target = new FileTarget(targetConfig.FileName, targetConfig.MinimumLevel,
                                                targetConfig.MaximumLevel, targetConfig.IncludeTimeStamps,
                                                targetConfig.ResetOnStartup);
                        break;
                }

                if (target != null)
                    LogManager.AttachLogTarget(target);
            }
        }
        public static void StartWatchdog()
        {
            Watchdog = new Watchdog();
            WatchdogThread = new Thread(Watchdog.Run) { Name = "Watchdog", IsBackground = true };
            WatchdogThread.Start();
        }
        public static void StartGameServer()
        {
            if (GameServer != null) return;

            GameServer = new DiIiS_NA.GameServer.ClientSystem.GameServer();
            GameServerThread = new Thread(GameServer.Run) { Name = "GameServerThread", IsBackground = true };
            GameServerThread.Start();
            if (Core.Discord.Config.Instance.Enabled)
            {
                Logger.Info("Starting Discord bot handler..");
                GameServer.DiscordBot = new Core.Discord.Bot();
                GameServer.DiscordBot.MainAsync().GetAwaiter().GetResult();
            }
            else
            {
                Logger.Info("Discord bot Disabled..");
            }
            DiIiS_NA.GameServer.GSSystem.GeneratorsSystem.SpawnGenerator.RegenerateDensity();
            DiIiS_NA.GameServer.ClientSystem.GameServer.GSBackend = new GsBackend(DiIiS_NA.LoginServer.Config.Instance.BindIP, DiIiS_NA.LoginServer.Config.Instance.WebPort);
        }

        static bool SetTitle(string text)
        {
            try
            {
                Console.Title = text;
                return true;
            }
            catch (PlatformNotSupportedException)
            {
                return false;
            }
        }
        
        [DllImport("kernel32.dll", ExactSpelling = true)]

        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int HIDE = 0;
        const int MAXIMIZE = 3;
        const int MINIMIZE = 6;
        const int RESTORE = 9;
        private static void Maximize()
        {
            // if it's running on windows
            try
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    ShowWindow(GetConsoleWindow(), MAXIMIZE);
                }
            }
            catch{ /*ignore*/ }
        }
    }
}
