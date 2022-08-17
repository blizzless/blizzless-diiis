//Blizzless Project 2022
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

namespace DiIiS_NA.LoginServer
{
	public sealed class Config : Core.Config.Config
	{
		public bool Enabled { get { return this.GetBoolean("Enabled", true); } set { this.Set("Enabled", value); } }
		public string BindIP { get { return this.GetString("BindIP", "127.0.0.1"); } set { this.Set("BindIP", value); } }
		public int WebPort { get { return this.GetInt("WebPort", 9000); } set { this.Set("WebPort", value); } }
		public int Port { get { return this.GetInt("Port", 1119); } set { this.Set("Port", value); } }
		public string BindIPv6 { get { return this.GetString("BindIPv6", "::1"); } set { this.Set("BindIPv6", value); } }

		private static readonly Config _instance = new Config();
		public static Config Instance { get { return _instance; } }
		private Config() : base("Battle-Server") { }
	}
}
