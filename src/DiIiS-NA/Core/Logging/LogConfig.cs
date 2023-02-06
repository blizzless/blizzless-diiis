namespace DiIiS_NA.Core.Logging
{
	public sealed class LogConfig : Config.Config
	{
		public string LoggingRoot
		{
			get => GetString("Root", @"logs");
			set => Set("Root", value);
		}

		public LogTargetConfig[] Targets { get; } = new[]
		{
			new LogTargetConfig("ConsoleLog"),
			new LogTargetConfig("AnsiLog"),
			//new LogTargetConfig("ServerLog"),
			//new LogTargetConfig("ChatLog"),
			//new LogTargetConfig("RenameAccountLog"),
			new LogTargetConfig("PacketLog")
		};

		private LogConfig() :
			base(nameof(Logging)) 
		{ }

		public static LogConfig Instance = new();
	}
	public class LogTargetConfig : Config.Config
	{
		public bool Enabled
		{
			get => GetBoolean(nameof(Enabled), true);
			set => Set(nameof(Enabled), value);
		}

		public string Target
		{
			get => GetString(nameof(Target), "Console");
			set => GetString(nameof(Target), value);
		}

		public bool IncludeTimeStamps
		{
			get => GetBoolean(nameof(IncludeTimeStamps), false);
			set => Set(nameof(IncludeTimeStamps), value);
		}

		public string FileName
		{
			get => GetString(nameof(FileName), "");
			set => GetString(nameof(FileName), value);
		}

		public Logger.Level MinimumLevel
		{
			get => (Logger.Level)(GetInt(nameof(MinimumLevel), (int)Logger.Level.Info, true));
			set => Set(nameof(MinimumLevel), (int)value);
		}

		public Logger.Level MaximumLevel
		{
			get => (Logger.Level)(GetInt(nameof(MaximumLevel), (int)Logger.Level.Fatal, true));
			set => Set(nameof(MaximumLevel), (int)value);
		}

		public bool ResetOnStartup
		{
			get => GetBoolean(nameof(ResetOnStartup), false);
			set => Set(nameof(ResetOnStartup), value);
		}

		public LogTargetConfig(string loggerName) : base(loggerName) { }
	}
}
