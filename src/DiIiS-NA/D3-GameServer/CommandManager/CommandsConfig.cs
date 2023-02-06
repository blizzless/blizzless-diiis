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

		public static CommandsConfig Instance = new();
		private CommandsConfig() : base("Commands") { }
	}
}
