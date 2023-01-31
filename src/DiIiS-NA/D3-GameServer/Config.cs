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
			get => GetBoolean(nameof(Enabled), true);
			set => Set(nameof(Enabled), value);
		}

		public string BindIP
		{
			get => GetString(nameof(BindIP), "127.0.0.1");
			set => Set(nameof(BindIP), value);
		}

		public int WebPort
		{
			get => GetInt(nameof(WebPort), 9001);
			set => Set(nameof(WebPort), value);
		}

		public int Port
		{
			get => GetInt(nameof(Port), 1345);
			set => Set(nameof(Port), value);
		}

		public string BindIPv6
		{
			get => GetString(nameof(BindIPv6), "::1");
			set => Set(nameof(BindIPv6), value);
		}

		public bool DRLGemu
		{
			get => GetBoolean(nameof(DRLGemu), true);
			set => Set(nameof(DRLGemu), value);
		}

		public bool CoreActive
		{
			get => GetBoolean(nameof(CoreActive), true);
			set => Set(nameof(CoreActive), value);
		}

		/// <summary>
		/// Rate of experience gain.
		/// </summary>
		public float RateExp
		{
			get => GetFloat(nameof(RateExp), 1);
			set => Set(nameof(RateExp), value);
		}

		/// <summary>
		/// Rate of gold gain.
		/// </summary>
		public float RateMoney
		{
			get => GetFloat(nameof(RateMoney), 1);
			set => Set(nameof(RateMoney), value);
		}

		/// <summary>
		/// Rate of item drop.
		/// </summary>
		public float RateDrop
		{
			get => GetFloat(nameof(RateDrop), 1);
			set => Set(nameof(RateDrop), value);
		}

		public float RateChangeDrop
		{
			get => GetFloat(nameof(RateChangeDrop), 1);
			set => Set(nameof(RateChangeDrop), value);
		}

		/// <summary>
		/// Rate of monster's HP.
		/// </summary>
		public float RateMonsterHP
		{
			get => GetFloat(nameof(RateMonsterHP), 1);
			set => Set(nameof(RateMonsterHP), value);
		}

		/// <summary>
		/// Rate of monster's damage.
		/// </summary>
		public float RateMonsterDMG
		{
			get => GetFloat(nameof(RateMonsterDMG), 1);
			set => Set(nameof(RateMonsterDMG), value);
		}
		
		public bool IWServer
		{
			get => GetBoolean(nameof(IWServer), true);
			set => Set(nameof(IWServer), value);
		}

		/// <summary>
		/// Percentage that a unique, legendary, set or special item created is unidentified
		/// </summary>
		public float ChanceHighQualityUnidentified
		{
			get => GetFloat(nameof(ChanceHighQualityUnidentified), 30f);
			set => Set(nameof(ChanceHighQualityUnidentified), value);
		}
		
		/// <summary>
		/// Percentage that a normal item created is unidentified
		/// </summary>
		public float ChanceNormalUnidentified
		{
			get => GetFloat(nameof(ChanceNormalUnidentified), 5f);
			set => Set(nameof(ChanceNormalUnidentified), value);
		}

		/// <summary>
		/// Resurrection charges on changing worlds
		/// </summary>
		public int ResurrectionCharges
		{
			get => GetInt(nameof(ResurrectionCharges), 3);
			set => Set(nameof(ResurrectionCharges), value);
		}

		/// <summary>
		/// Boss Health Multiplier
		/// </summary>
		public float BossHealthMultiplier
		{
			get => GetFloat(nameof(BossHealthMultiplier), 6f);
			set => Set(nameof(BossHealthMultiplier), value);
		}
		
		/// <summary>
		/// Boss Damage Multiplier
		/// </summary>
		public float BossDamageMultiplier
		{
			get => GetFloat(nameof(BossDamageMultiplier), 3f);
			set => Set(nameof(BossDamageMultiplier), value);
		}

		/// <summary>
		/// Whether to bypass the quest's settings of "Saveable" to TRUE (unless in OpenWorld)
		/// </summary>
		public bool AutoSaveQuests
		{
			get => GetBoolean(nameof(AutoSaveQuests), false);
			set => Set(nameof(AutoSaveQuests), value);
		}
		
		public static Config Instance { get; } = new();

		private Config() : base("Game-Server")
		{
		}
	}
}
