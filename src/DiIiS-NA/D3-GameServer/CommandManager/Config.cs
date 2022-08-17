//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.CommandManager
{
	public sealed class Config : DiIiS_NA.Core.Config.Config
	{
		public char CommandPrefix { get { return this.GetString("CommandPrefix", "!")[0]; } set { this.Set("CommandPrefix", value); } }

		private static readonly Config _instance = new Config();
		public static Config Instance { get { return _instance; } }
		private Config() : base("Commands") { }
	}
}
