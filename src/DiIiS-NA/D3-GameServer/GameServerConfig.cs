using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer
{
	public sealed class GameServerConfig : DiIiS_NA.Core.Config.Config
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
		
		public bool IWServer
		{
			get => GetBoolean(nameof(IWServer), true);
			set => Set(nameof(IWServer), value);
		}
		
		public bool AfkDisconnect
		{
#if DEBUG
			get => GetBoolean(nameof(AfkDisconnect), false);
#else
			get => GetBoolean(nameof(AfkTimeoutEnabled), true);
#endif
			set => Set(nameof(AfkDisconnect), value);
		}
		
		#region Game Mods

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

		/// <summary>
		/// Progress gained when killing a monster in Nephalem Rifts
		/// </summary>
		public float NephalemRiftProgressMultiplier
		{
			get => GetFloat(nameof(NephalemRiftProgressMultiplier), 1f);
			set => Set(nameof(NephalemRiftProgressMultiplier), value);
		}
		
		/// <summary>
		///	How much a health potion heals in percentage
		/// </summary>
		public float HealthPotionRestorePercentage
		{
			get => GetFloat(nameof(HealthPotionRestorePercentage), 60f);
			set => Set(nameof(HealthPotionRestorePercentage), value);
		}
		
		/// <summary>
		/// Cooldown (in seconds) to use a health potion again.
		/// </summary>
		public float HealthPotionCooldown
		{
			get => GetFloat(nameof(HealthPotionCooldown), 30f);
			set => Set(nameof(HealthPotionCooldown), value);
		}
		
		/// <summary>
		/// Unlocks all waypoints in the campaign.
		/// </summary>
		public bool UnlockAllWaypoints
		{
			get => GetBoolean(nameof(UnlockAllWaypoints), false);
			set => Set(nameof(UnlockAllWaypoints), value);
		}

		/// <summary>
		/// Strength multiplier when you're not a paragon.
		/// </summary>
		public float StrengthMultiplier
		{
			get => GetFloat(nameof(StrengthMultiplier), 1f);
			set => Set(nameof(StrengthMultiplier), value);
		}
		
		/// <summary>
		/// Strength multiplier when you're a paragon.
		/// </summary>
		public float StrengthParagonMultiplier
		{
			get => GetFloat(nameof(StrengthParagonMultiplier), 1f);
			set => Set(nameof(StrengthParagonMultiplier), value);
		}
		
		/// <summary>
		/// Dexterity multiplier when you're not a paragon.
		/// </summary>
		public float DexterityMultiplier
		{
			get => GetFloat(nameof(DexterityMultiplier), 1f);
			set => Set(nameof(DexterityMultiplier), value);
		}
		
		/// <summary>
		/// Dexterity multiplier when you're a paragon.
		/// </summary>
		public float DexterityParagonMultiplier
		{
			get => GetFloat(nameof(DexterityParagonMultiplier), 1f);
			set => Set(nameof(DexterityParagonMultiplier), value);
		}
		
		/// <summary>
		/// Intelligence multiplier when you're not a paragon.
		/// </summary>
		public float IntelligenceMultiplier
		{
			get => GetFloat(nameof(IntelligenceMultiplier), 1f);
			set => Set(nameof(IntelligenceMultiplier), value);
		}
		
		/// <summary>
		/// Intelligence multiplier when you're a paragon.
		/// </summary>
		public float IntelligenceParagonMultiplier
		{
			get => GetFloat(nameof(IntelligenceParagonMultiplier), 1f);
			set => Set(nameof(IntelligenceParagonMultiplier), value);
		}
		
		/// <summary>
		/// Vitality multiplier when you're not a paragon.
		/// </summary>
		public float VitalityMultiplier
		{
			get => GetFloat(nameof(VitalityMultiplier), 1f);
			set => Set(nameof(VitalityMultiplier), value);
		}
		
		/// <summary>
		/// Vitality multiplier when you're a paragon.
		/// </summary>
		public float VitalityParagonMultiplier
		{
			get => GetFloat(nameof(VitalityParagonMultiplier), 1f);
			set => Set(nameof(VitalityParagonMultiplier), value);
		}
		
		/// <summary>
		/// Auto finishes nephalem rift when there's <see cref="NephalemRiftAutoFinishThreshold"></see> or less monsters left.
		/// </summary>
		public bool NephalemRiftAutoFinish
		{
			get => GetBoolean(nameof(NephalemRiftAutoFinish), false);
			set => Set(nameof(NephalemRiftAutoFinish), value);
		}
		
		/// <summary>
		/// If <see cref="NephalemRiftAutoFinish"></see> is enabled, this is the threshold.
		/// </summary>
		public int NephalemRiftAutoFinishThreshold
		{
			get => GetInt(nameof(NephalemRiftAutoFinishThreshold), 2);
			set => Set(nameof(NephalemRiftAutoFinishThreshold), value);
		}
		
		/// <summary>
		/// Nephalem Rifts chance of spawning a orb.
		/// </summary>
		public float NephalemRiftOrbsChance
		{
			get => GetFloat(nameof(NephalemRiftOrbsChance), 0f);
			set => Set(nameof(NephalemRiftOrbsChance), value);
		}

		/// <summary>
		/// Forces the game to reveal all the map.
		/// </summary>
		public bool ForceMinimapVisibility
		{
			get => GetBoolean(nameof(ForceMinimapVisibility), false);
			set => Set(nameof(ForceMinimapVisibility), value);
		}

		#endregion
		public static GameServerConfig Instance { get; } = new();

		private GameServerConfig() : base("Game-Server")
		{
		}
	}
}
