//Blizzless Project 2022 
using System.Text;

namespace DiIiS_NA.GameServer
{
	public sealed class NATConfig : DiIiS_NA.Core.Config.Config
	{
		public bool Enabled { get { return this.GetBoolean("Enabled", false); } set { this.Set("Enabled", value); } }
		public string PublicIP { get { return this.GetString("PublicIP", "0.0.0.0"); } set { this.Set("PublicIP", value); } }

		private static readonly NATConfig _instance = new NATConfig();
		public static NATConfig Instance { get { return _instance; } }
		private NATConfig() : base("NAT") { }
	}
}
