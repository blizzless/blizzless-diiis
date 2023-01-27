using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.CommandManager
{
	public sealed class Config : DiIiS_NA.Core.Config.Config
	{
		public char CommandPrefix { get { return GetString("CommandPrefix", "!")[0]; } set { Set("CommandPrefix", value); } }

		private static readonly Config _instance = new Config();
		public static Config Instance { get { return _instance; } }
		private Config() : base("Commands") { }
	}
}
