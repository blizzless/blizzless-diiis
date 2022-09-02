//Blizzless Project 2022
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.AchievementSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.CommandManager;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Base;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Battle;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.GuildSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022 
using DiIiS_NA.REST;
//Blizzless Project 2022 
using DiIiS_NA.REST.Manager;
//Blizzless Project 2022 
using DotNetty.Handlers.Logging;
//Blizzless Project 2022 
using DotNetty.Transport.Bootstrapping;
//Blizzless Project 2022 
using DotNetty.Transport.Channels;
//Blizzless Project 2022 
using DotNetty.Transport.Channels.Sockets;
//Blizzless Project 2022 
using Npgsql;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Data.Common;
//Blizzless Project 2022 
using System.Globalization;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Runtime.ExceptionServices;
//Blizzless Project 2022 
using System.Security;
//Blizzless Project 2022 
using System.Security.Permissions;
//Blizzless Project 2022 
using System.Threading;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA
{
    
    class Program
    {
        private static readonly Logger Logger = LogManager.CreateLogger("BZ.Net"); 
        public static readonly DateTime StartupTime = DateTime.Now;
        public static BattleBackend BattleBackend { get; set; }
        public bool GameServersAvailable = true;

        public static int MaxLevel = 70;

        public static GameServer.ClientSystem.GameServer GameServer;
        public static Watchdog Watchdog;

		public static Thread GameServerThread;
		public static Thread WatchdogThread;
        
        public static string LOGINSERVERIP = DiIiS_NA.LoginServer.Config.Instance.BindIP;
        public static string GAMESERVERIP = DiIiS_NA.GameServer.Config.Instance.BindIP;
        public static string RESTSERVERIP = DiIiS_NA.REST.Config.Instance.IP;
        public static string PUBLICGAMESERVERIP = DiIiS_NA.GameServer.NATConfig.Instance.PublicIP;

        public static int Build = 29;
        public static int Stage = 4;
        public static string TypeBuild = "Beta";
        public static bool D3CoreEnabled = DiIiS_NA.GameServer.Config.Instance.CoreActive;

        static async Task LoginServer()
        {
#if DEBUG
            D3CoreEnabled = true;
#endif
            DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            string Name = $"Blizzless: Build {Build}, Stage: {Stage} - {TypeBuild}";
            Console.WriteLine("                 " + Name);
            Console.WriteLine("Connected Module: 0x00 - Diablo 3 RoS 2.7.4.84161");
            Console.WriteLine("------------------------------------------------------------------------");
            Console.ResetColor();
            Console.Title = Name;

            InitLoggers();
            AchievementManager.Initialize();
            Core.Storage.AccountDataBase.SessionProvider.RebuildSchema();
#if DEBUG
            if (!Enumerable.Any(DBSessions.SessionQuery<DBAccount>()))
            {
                Logger.Info("Initing new database, creating (test@,123456), (test1@,123456), (test2@,123456),(test3@,123456),(test4@,123456)");
                var account = AccountManager.CreateAccount("test@", "123456", "test",  Account.UserLevels.Owner);
                var gameAccount = GameAccountManager.CreateGameAccount(account);
                var account1 = AccountManager.CreateAccount("test1@", "123456", "test1", Account.UserLevels.Owner);
                var gameAccount1 = GameAccountManager.CreateGameAccount(account1);
                var account2 = AccountManager.CreateAccount("test2@", "123456", "test2", Account.UserLevels.Owner);
                var gameAccount2 = GameAccountManager.CreateGameAccount(account2);
                var account3 = AccountManager.CreateAccount("test3@", "123456", "test3", Account.UserLevels.Owner);
                var gameAccount3 = GameAccountManager.CreateGameAccount(account3);
                var account4 = AccountManager.CreateAccount("test4@", "123456", "test4", Account.UserLevels.Owner);
                var gameAccount4 = GameAccountManager.CreateGameAccount(account4);
            }
#else
            if (!Enumerable.Any(DBSessions.SessionQuery<DBAccount>()))
            {
                var account = AccountManager.CreateAccount("iwannatry@", "iJustWannaTry", "iwannatry", Account.UserLevels.User);
                var gameAccount = GameAccountManager.CreateGameAccount(account);
            }
            if (DBSessions.SessionQuery<DBAccount>().Any())
			{
				Logger.Info("Connect with base established.");
			}
#endif
            //*/
            StartWatchdog();

            AccountManager.PreLoadAccounts();
            GameAccountManager.PreLoadGameAccounts();
            ToonManager.PreLoadToons();
            GuildManager.PreLoadGuilds();

            Logger.Info("Loading Diablo 3 - Core..."); 
            if (D3CoreEnabled)
            {
                if (!MPQStorage.Initialized)
                {
                    Logger.Fatal("MPQ archives not founded...");
                    Console.ReadLine();
                    return;
                }
                Logger.Info("Loaded - {0} items.", ItemGenerator.TotalItems); Logger.Info("Diablo 3 Core - Loaded"); 
            }
            else Logger.Info("Diablo 3 Core - Disabled");
           
            var restSocketServer = new SocketManager<RestSession>();
            if (!restSocketServer.StartNetwork(RESTSERVERIP, REST.Config.Instance.PORT))
            {
                Logger.Error("REST Socket server can't start.");
            }
            else
            {
                Logger.Info("REST server started - " + REST.Config.Instance.IP + ":{0}", REST.Config.Instance.PORT);
            }

            //BGS
            ServerBootstrap b = new ServerBootstrap();
            IEventLoopGroup boss = new MultithreadEventLoopGroup(1);
            IEventLoopGroup worker = new MultithreadEventLoopGroup();
            b.LocalAddress(DiIiS_NA.LoginServer.Config.Instance.BindIP, DiIiS_NA.LoginServer.Config.Instance.Port);
            Logger.Info("Server BlizzLess.Net started - " + DiIiS_NA.LoginServer.Config.Instance.BindIP + ":{0}", DiIiS_NA.LoginServer.Config.Instance.Port);
            BattleBackend = new BattleBackend(DiIiS_NA.LoginServer.Config.Instance.BindIP, DiIiS_NA.LoginServer.Config.Instance.WebPort);

            //Diablo 3 Game-Server
            if (D3CoreEnabled) StartGS();

            try
            {
                b.Group(boss, worker)
                    .Channel<TcpServerSocketChannel>()
                    .Handler(new LoggingHandler(LogLevel.DEBUG))
                    .ChildHandler(new ConnectHandler());

                IChannel boundChannel = await b.BindAsync(DiIiS_NA.LoginServer.Config.Instance.Port);

                while (true)
                {
                    var line = Console.ReadLine();
                    CommandManager.Parse(line);
                }
            }
            catch (Exception e)
            {
                Logger.Info(e.Message);
            }
            finally
            {
                await Task.WhenAll(
                boss.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                worker.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main() 
        {
            LoginServer().Wait(); 
        }
         
        [SecurityCritical]
        [HandleProcessCorruptedStateExceptionsAttribute]
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            if (e.IsTerminating)
            {
                Logger.Error(ex.StackTrace);
                Logger.FatalException(ex, "Была обнаружена корневая ошибка сервера, отключение.");
            }
            else
                Logger.ErrorException(ex, "Перехвачена корневая ошибка, но была предотвращена.");

            Console.ReadLine();
        }


        private static void InitLoggers()
        {
            LogManager.Enabled = true;

            foreach (var targetConfig in LogConfig.Instance.Targets)
            {
                if (!targetConfig.Enabled)
                    continue;

                LogTarget target = null;
                switch (targetConfig.Target.ToLower())
                {
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
        public static bool StartWatchdog()
        {
            Watchdog = new Watchdog();
            WatchdogThread = new Thread(Watchdog.Run) { Name = "Watchdog", IsBackground = true };
            WatchdogThread.Start();
            return true;
        }
        public static bool StartGS()
        {
            if (GameServer != null) return false;

            GameServer = new DiIiS_NA.GameServer.ClientSystem.GameServer();
            GameServerThread = new Thread(GameServer.Run) { Name = "GameServerThread", IsBackground = true };
            GameServerThread.Start();

            DiIiS_NA.GameServer.GSSystem.GeneratorsSystem.SpawnGenerator.RegenerateDensity();
            DiIiS_NA.GameServer.ClientSystem.GameServer.GSBackend = new GSBackend(DiIiS_NA.LoginServer.Config.Instance.BindIP, DiIiS_NA.LoginServer.Config.Instance.WebPort);
            return true;
        }

    }
}
