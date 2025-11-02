using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Utilities;
using Spectre.Console;

namespace DiIiS_NA.GameServer.CommandManager
{
	public sealed class CommandsConfig : DiIiS_NA.Core.Config.Config
	{
		private readonly Logger _logger = LogManager.CreateLogger<CommandsConfig>();
        public string CommandPrefix
        {
            get => GetString(nameof(CommandPrefix), "!");
            set
            {
                if (value.Length > 0)
                {
                    _logger.Warn("CommandPrefix".Markup().Bold().Color(Color.Red3_1) +
                                 $" must be only 1 character. Defaulting to '{'!'.Markup().Bold().Color(Color.Yellow3_1)}'.");
					Set(nameof(CommandPrefix), "!");
                }
				else
					Set(nameof(CommandPrefix), value.Trim());
            }
        }

        public string DisabledGroups 
		{
			get => GetString(nameof(DisabledGroups), "");
			set => Set(nameof(DisabledGroups), value);
		}

		public string[] DisabledGroupsData 
			=> DisabledGroups
				.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
				.Select(s=>s.Replace(CommandPrefix, ""))
				.ToArray();

		public static readonly CommandsConfig Instance = new();
		private CommandsConfig() : base("Commands") { }
	}
}
