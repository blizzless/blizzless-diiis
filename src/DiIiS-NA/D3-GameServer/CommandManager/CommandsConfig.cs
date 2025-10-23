using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.CommandManager
{
	public sealed class CommandsConfig : DiIiS_NA.Core.Config.Config
	{
		public char CommandPrefix 
		{
			get => GetString(nameof(CommandPrefix), "!")[0];
			set => Set(nameof(CommandPrefix), value);
		}
		
		public string DisabledGroups 
		{
			get => GetString(nameof(DisabledGroups), "");
			set => Set(nameof(DisabledGroups), value);
		}

		public string[] DisabledGroupsData 
			=> DisabledGroups
				.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
				.Select(s=>s.Replace(CommandPrefix.ToString(), ""))
				.ToArray();

		public static readonly CommandsConfig Instance = new();
		private CommandsConfig() : base("Commands") { }
	}
}
