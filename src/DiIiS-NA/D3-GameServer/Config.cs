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

namespace DiIiS_NA.GameServer
{
	public sealed class Config : DiIiS_NA.Core.Config.Config
	{
		public bool Enabled { get { return this.GetBoolean("Enabled", true); } set { this.Set("Enabled", value); } }
		public string BindIP { get { return this.GetString("BindIP", "127.0.0.1"); } set { this.Set("BindIP", value); } }
		public int WebPort { get { return this.GetInt("WebPort", 9001); } set { this.Set("WebPort", value); } }
		public int Port { get { return this.GetInt("Port", 1345); } set { this.Set("Port", value); } }
		public string BindIPv6 { get { return this.GetString("BindIPv6", "::1"); } set { this.Set("BindIPv6", value); } }
		public bool DRLGemu { get { return this.GetBoolean("DRLGemu", true); } set { this.Set("DRLGemu", value); } }
		public bool CoreActive { get { return this.GetBoolean("CoreActive", true); } set { this.Set("CoreActive", value); } }

		//Modding of Game-Server
		public float RateEXP { get { return this.GetFloat("RateExp", 1); } set { this.Set("RateExp", value); } }
		public float RateMoney { get { return this.GetFloat("RateMoney", 1); } set { this.Set("RateMoney", value); } }
		public float RateDrop { get { return this.GetFloat("RateDrop", 1); } set { this.Set("RateDrop", value); } }
		public float RateChangeDrop { get { return this.GetFloat("RateChangeDrop", 1); } set { this.Set("RateChangeDrop", value); } }
		public float RateMonsterHP { get { return this.GetFloat("RateMonsterHP", 1); } set { this.Set("RateMonsterHP", value); } }
		public float RateMonsterDMG { get { return this.GetFloat("RateMonsterHP", 1); } set { this.Set("RateMonsterHP", value); } }


		public bool IWServer { get { return this.GetBoolean("IWServer", true); } set { this.Set("IWServer", value); } }

		private static readonly Config _instance = new Config();
		public static Config Instance { get { return _instance; } }
		private Config() : base("Game-Server") { }
	}
}
