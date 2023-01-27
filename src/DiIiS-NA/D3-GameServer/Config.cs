using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer
{
	public sealed class Config : DiIiS_NA.Core.Config.Config
	{
		public bool Enabled
		{
			get => GetBoolean("Enabled", true);
			set => Set("Enabled", value);
		}

		public string BindIP
		{
			get => GetString("BindIP", "127.0.0.1");
			set => Set("BindIP", value);
		}

		public int WebPort
		{
			get => GetInt("WebPort", 9001);
			set => Set("WebPort", value);
		}

		public int Port
		{
			get => GetInt("Port", 1345);
			set => Set("Port", value);
		}

		public string BindIPv6
		{
			get => GetString("BindIPv6", "::1");
			set => Set("BindIPv6", value);
		}

		public bool DRLGemu
		{
			get => GetBoolean("DRLGemu", true);
			set => Set("DRLGemu", value);
		}

		public bool CoreActive
		{
			get => GetBoolean("CoreActive", true);
			set => Set("CoreActive", value);
		}

		//Modding of Game-Server
		public float RateEXP
		{
			get => GetFloat("RateExp", 1);
			set => Set("RateExp", value);
		}

		public float RateMoney
		{
			get => GetFloat("RateMoney", 1);
			set => Set("RateMoney", value);
		}

		public float RateDrop
		{
			get => GetFloat("RateDrop", 1);
			set => Set("RateDrop", value);
		}

		public float RateChangeDrop
		{
			get => GetFloat("RateChangeDrop", 1);
			set => Set("RateChangeDrop", value);
		}

		public float RateMonsterHP
		{
			get => GetFloat("RateMonsterHP", 1);
			set => Set("RateMonsterHP", value);
		}

		public float RateMonsterDMG
		{
			get => GetFloat("RateMonsterHP", 1);
			set => Set("RateMonsterHP", value);
		}


		public bool IWServer
		{
			get => GetBoolean("IWServer", true);
			set => Set("IWServer", value);
		}

		public static Config Instance { get; } = new();

		private Config() : base("Game-Server")
		{
		}
	}
}
