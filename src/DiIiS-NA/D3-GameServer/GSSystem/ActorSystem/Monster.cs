
using System;
using System.Linq;
using MonsterFF = DiIiS_NA.Core.MPQ.FileFormats.Monster;
using GameBalance = DiIiS_NA.Core.MPQ.FileFormats.GameBalance;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.D3_GameServer;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using World = DiIiS_NA.GameServer.GSSystem.MapSystem.World;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.LoginServer.Toons;
using static DiIiS_NA.Core.MPQ.FileFormats.Monster;
using D3.Store;
using DiIiS_NA.GameServer.GSSystem.AISystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using Microsoft.EntityFrameworkCore.Metadata;
using static DiIiS_NA.Core.Logging.Logger;
using System.IO;
using System.Net.NetworkInformation;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class Monster : Living, IUpdateable
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public override ActorType ActorType => ActorType.Monster;
		public TickTimer DestroyTimer { get; }

		private int _adjustedPlayers = 1;
		private object _adjustLock = new object();
		private float _nativeHp = 0f;
		private float _nativeDmg = 0f;
		public override int Quality
		{
			get => SNO == ActorSno._x1_lr_boss_mistressofpain ? 7 : (int)SpawnType.Normal;
			set => Logger.Warn("Quality of monster cannot be changed");
		}

		public int LoreSnoId => Monster.IsValid ? ((MonsterFF)Monster.Target).SNOLore : -1;

		public int MonsterTypeValue => Monster.IsValid ? (int)((MonsterFF)Monster.Target).Type : -1;
		public MonsterType MonsterType => (MonsterType)(((MonsterFF)Monster.Target)?.Type ?? MonsterType.Unknown);
        public float HpMultiplier => Monster.IsValid ? (1f + ((MonsterFF)Monster.Target).AttributeModifiers[4]) : 1f;

		public float DmgMultiplier => Monster.IsValid ? (1f + ((MonsterFF)Monster.Target).AttributeModifiers[55]) : 1f;
		public Vector3D BasePoint { get; set; }

		/// <summary>
		/// Gets the Actors summoning fields from the mpq's and returns them in format for Monsters.
		/// Useful for the Monsters spawning/summoning skills.
		/// </summary>
		public ActorSno[] SnoSummons => ((MonsterFF)Monster.Target).SNOSummonActor.Select(x => (ActorSno)x).ToArray();

		public Monster(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			#if DEBUG
			if (this is Boss boss)
			{
				Logger.Info($"Boss $[underline]${boss.SNO}$[/]$ created");
			}
			#endif

			Field2 = 0x8;
			GBHandle.Type = (int)ActorType.Monster; GBHandle.GBID = 1;
			Attributes[GameAttributes.TeamID] = 9;
			if (Monster.Id != -1)
				WalkSpeed = ((MonsterFF)Monster.Target).AttributeModifiers[129];  
			//WalkSpeed /= 2f;
			
			Brain = new MonsterBrain(this);
			Attributes[GameAttributes.Attacks_Per_Second] = GameModsConfig.Instance.Monster.AttacksPerSecond;// 1.2f;

			UpdateStats();
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			#if DEBUG
			string monster = "monster";
			if (this is Boss) monster = "boss";
			Logger.MethodTrace($"Player {player.Name} targeted $[underline]${monster}$[/]$ {GetType().Name}.");
			#endif
		}

		public void UpdateStats()
		{
            // TODO: Level up is getting harder from level 3+. 1 seems stable. check the difficulty.
            // TODO: Level up is getting harder from level 3+. 1 seems stable. check the difficulty.
            // TODO: Level up is getting harder from level 3+. 1 seems stable. check the difficulty.

            var monsterLevels = (GameBalance)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.GameBalance][19760].Data;
			bool fullHp = (Math.Abs(Attributes[GameAttributes.Hitpoints_Cur] - Attributes[GameAttributes.Hitpoints_Max_Total]) < Globals.FLOAT_TOLERANCE);
			Attributes[GameAttributes.Level] = World.Game.MonsterLevel;
			//this.Attributes[GameAttribute.Hitpoints_Max] = (int)monsterLevels.MonsterLevel[this.World.Game.MonsterLevel - 1].HPMin * (int)this.HPMultiplier * (int)this.World.Game.HPModifier;
			int monsterLevel = 1;
			monsterLevel = World.Game.ConnectedPlayers.Length > 1 ? World.Game.ConnectedPlayers[0].Level : World.Game.InitialMonsterLevel;

            var connectedPlayers = World.Game.ConnectedPlayers.ToArray();
			double maxUsersHealth = 1f;
			double deltaDamageUsers = 1f;
			int userLevelAverage = 1;
			
			if (connectedPlayers.Any())
			{
				maxUsersHealth = connectedPlayers.Average(x => x.Attributes[GameAttributes.Hitpoints_Max]);
				deltaDamageUsers = connectedPlayers.Average(x => x.Attributes[GameAttributes.Damage_Delta]);
				userLevelAverage = (int)connectedPlayers.Average(x => x.Level);
				Logger.MethodTrace($"$[yellow]${connectedPlayers.Length}$[/]$ $[green]$players online$[/]$: $[blue dim]${maxUsersHealth}$[/]$ $[bold]$avg. max health$[/]$ / $[blue dim italic]${deltaDamageUsers}$[/]$ $[bold]$avg. delta damage$[/]$");
			}

            var difficulty = World.Game.Difficulty;
            var maxHP = (monsterLevels.MonsterLevel[monsterLevel].HPMin +
                         RandomHelper.NextFloat(0f, monsterLevels.MonsterLevel[monsterLevel].HPDelta)) *
                        HpMultiplier * World.Game.HpModifier;
			var bonus = CalculateLevelAdjustment(LevelAdjustmentEnum.LinearScaling, difficulty, connectedPlayers);
            
			Attributes[GameAttributes.Hitpoints_Max] = maxHP;
            Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus_Multiplicative] = bonus;

            var baseHp = Attributes[GameAttributes.Hitpoints_Max];
            var baseDamage = Attributes[GameAttributes.Damage_Weapon_Min, 0];

			// Apply calculated scaling
            baseHp *= bonus;
            baseDamage *= bonus;

            // Apply configuration modifiers
            baseHp *= GameModsConfig.Instance.Monster.HealthMultiplier;
            baseDamage *= GameModsConfig.Instance.Monster.DamageMultiplier;

            // Assign modified values 
            Attributes[GameAttributes.Hitpoints_Max_Total] = baseHp;
            Attributes[GameAttributes.Damage_Weapon_Min, 0] = baseDamage;
            //if (full_hp)
            Attributes[GameAttributes.Hitpoints_Cur] = Attributes[GameAttributes.Hitpoints_Max_Total];

			Attributes.BroadcastChangedIfRevealed();
		}

		enum LevelAdjustmentEnum { LinearScaling, DiminishedReturns, CurveScaling, LinearScalingAndDiminishedReturnsAfterThreshold }

		private float CalculateLevelAdjustment(LevelAdjustmentEnum levelAdjustment, int difficulty = 0, params Player[] players)
		{
			var playersStats = players.Select(s =>
				new
				{
					s.Attributes,
					TotalLevel = s.Level + s.ParagonLevel * 1.05f,
					Health = s.Attributes[GameAttributes.Hitpoints_Max],
					Damage = s.Attributes[GameAttributes.Damage_Weapon_Min, 0],
					Toughness = s.Attributes[GameAttributes.Armor_Total],
					DPS = s.Attributes[GameAttributes.DPS]
				}
			).ToArray();
			var monstersNearbyStats = players.WhereNearbyOf(World.Monsters.ToArray(), s => s.Visible && s.Alive && s.Attributes[GameAttributes.Hitpoints_Max] * 0.8 > Attributes[GameAttributes.Hitpoints_Cur], 120f, 1f).ToArray();
			var monsterStats = monstersNearbyStats.Select(s =>
					new
					{
						s.Attributes,
						Health = s.Attributes[GameAttributes.Hitpoints_Max],
						Damage = s.Attributes[GameAttributes.Damage_Weapon_Min, 0],
						Toughness = s.Attributes[GameAttributes.Armor_Total],
						DPS = s.Attributes[GameAttributes.DPS]
					}
			).ToArray();

			// Define configuration constants
			// This is the multiplier for linear scaling. It determines how much the monster's level increases for each player level. 
			// If you increase this value, monsters will become stronger faster as player levels increase.
			const float linearMultiplierConfig = 0.025f;

			// This is the multiplier for diminished returns scaling. It determines how much the monster's level increases for each player level, 
			// but the increase becomes smaller as player levels get higher. If you increase this value, monsters will become stronger faster at lower player levels.
			const float diminishedMultiplierConfig = 0.1f;

			// This is the base value for diminished returns scaling. It's the starting point for the monster's level before any scaling is applied. 
			// If you increase this value, monsters will start off stronger before any player level scaling is applied.
			const float diminishedBaseConfig = 1.0f;

			// This is the multiplier for curve scaling. It determines how much the monster's level increases for each player level, 
			// but the increase becomes larger as player levels get higher. If you increase this value, monsters will become stronger faster at higher player levels.
			const float curveMultiplierConfig = 0.1f;

			// This is the base value for curve scaling. It's the starting point for the monster's level before any scaling is applied. 
			// If you increase this value, monsters will start off stronger before any player level scaling is applied.
			const float curveBaseConfig = 30.0f;

			// This is the exponent for curve scaling. It determines the shape of the curve for how much the monster's level increases for each player level. 
			// If you increase this value, the curve will be steeper, meaning monsters will become much stronger at higher player levels.
			const float curveExponentConfig = 0.1f;

			// This is the multiplier for linear scaling after a certain threshold. It determines how much the monster's level increases for each player level 
			// after the player level has reached a certain threshold. If you increase this value, monsters will become stronger faster after player levels reach the threshold.
			const float linearMultiplierThresholdConfig = 0.005f;

			// This is the multiplier for log scaling. It determines how much the monster's level increases for each player level, 
			// but the increase becomes smaller as player levels get higher. If you increase this value, monsters will become stronger faster at lower player levels.
			const float logMultiplierConfig = 0.1f;

			// This is the threshold for linear scaling. It determines the player level at which linear scaling starts to apply. 
			// If you increase this value, linear scaling will start to apply at higher player levels.
			const float thresholdConfig = 40.0f;

			// This is the ratio for DPS (Damage Per Second) scaling. It determines how much the monster's level increases for each unit of player DPS. 
			// If you increase this value, monsters will become stronger faster as player DPS increases.
			const float dpsRatioConfig = 1.2f;

			// This is the ratio for toughness scaling. It determines how much the monster's level increases for each unit of player toughness. 
			// If you increase this value, monsters will become stronger faster as player toughness increases.
			const float toughnessRatioConfig = 0.1f;

			// Define variables for average user and monster stats
			//         float avgUserLevel = playersStats.Average(s => s.TotalLevel);
			//         float avgUserDPS = playersStats.Average(s => s.DPS);
			//         float avgUserToughness = playersStats.Average(s => s.Toughness);
			//float avgMonsterDPS = playersStats.Average(s => s.DPS);
			//         float avgMonsterToughness = monsterStats.Average(s => s.Toughness);
			//var tierMultiplier = GetMonsterTierMultiplier();
			float avgUserLevel = 1f, avgUserDPS = 1f, avgUserToughness = 1f, avgMonsterDPS = 1f, avgMonsterToughness = 1f, tierMultiplier = 1f;

			if (playersStats.Any())
			{
				avgUserLevel = playersStats.Average(s => s.TotalLevel);
				avgUserDPS = playersStats.Average(s => s.DPS);
				avgUserToughness = playersStats.Average(s => s.Toughness);
			}
			if (monsterStats.Any())
			{
                avgMonsterDPS = playersStats.Average(s => s.DPS);
                avgMonsterToughness = monsterStats.Average(s => s.Toughness);
                tierMultiplier = GetMonsterTierMultiplier();
            }
            float LevelScaling() => 1.0f + 0.1f * MathF.Log10(avgUserLevel + 1) * tierMultiplier;
            float DiminishedReturns() => diminishedBaseConfig + diminishedMultiplierConfig * avgUserLevel;
            float CurveScaling() => curveBaseConfig * MathF.Pow(avgUserLevel, curveExponentConfig) * curveMultiplierConfig;
            float LinearScalingAndDiminishedReturnsAfterThreshold() => MathF.Max(1.0f, MathF.Min(1.5f, logMultiplierConfig * MathF.Log10(avgUserLevel + 1) + (avgUserLevel - thresholdConfig) * linearMultiplierThresholdConfig) * tierMultiplier);
			
            return levelAdjustment switch
            {
                LevelAdjustmentEnum.LinearScaling => LevelScaling(),
                LevelAdjustmentEnum.DiminishedReturns => DiminishedReturns(),
                LevelAdjustmentEnum.CurveScaling => CurveScaling(),
                LevelAdjustmentEnum.LinearScalingAndDiminishedReturnsAfterThreshold => LinearScalingAndDiminishedReturnsAfterThreshold(),
                _ => LinearScalingAndDiminishedReturnsAfterThreshold()
            };
        }

        private float GetMonsterTierMultiplier()
		{
            return MonsterType switch
            {
                MonsterType.Beast => 1.1f,
                MonsterType.Demon => 1.15f,
                MonsterType.Human => 1.25f,
                MonsterType.Undead => 1.4f, // Steeper jump here
                _ => 1.0f,
            };
        }


        int _bleedFirstTick = 0;
		int _caltropsFirstTick = 0;

		public void Update(int tickCounter)
		{
			if (DestroyTimer != null)
			{
				Logger.Trace("Killed monster destroy timer update");
				DestroyTimer.Update(tickCounter);
			}

			if (Brain == null)
				return;
			if (World == null)
				return;
			Brain.Update(tickCounter);
			
			if (World.SNO == WorldSno.a4dun_diablo_arena)
				if (SNO == ActorSno._diablo)
					if (Attributes[GameAttributes.Hitpoints_Cur] < (Attributes[GameAttributes.Hitpoints_Max_Total] / 2))
					{
						Attributes[GameAttributes.Hitpoints_Cur] = Attributes[GameAttributes.Hitpoints_Max_Total];
						World.Game.QuestManager.Advance();//advancing United Evil quest
						var nextWorld = World.Game.GetWorld(WorldSno.a4dun_diablo_shadowrealm_01);
						foreach (var plr in World.Players.Values)
							plr.ChangeWorld(nextWorld, nextWorld.GetStartingPointById(172).Position);
					}

			if (this is Boss)
			{
				if (!World.BuffManager.HasBuff<PowerSystem.Implementations.Caltrops.ActiveCalTrops>(this))
					_caltropsFirstTick = tickCounter;

				if ((tickCounter - _caltropsFirstTick) >= 2400)
				{
					var buffOwner = World.BuffManager.GetFirstBuff<PowerSystem.Implementations.Caltrops.ActiveCalTrops>(this).User;
					if (buffOwner is Player player)
						player.GrantAchievement(74987243307067);
				}

			}
			if (!World.BuffManager.HasBuff<PowerSystem.Implementations.Rend.RendDebuff>(this))
				_bleedFirstTick = tickCounter;

			if ((tickCounter - _bleedFirstTick) >= 1200)
			{
				var buffOwner = World.BuffManager.GetFirstBuff<PowerSystem.Implementations.Rend.RendDebuff>(this).User;
				if (buffOwner is Player player)
					player.GrantAchievement(74987243307052);
			}
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			lock (_adjustLock)
			{
				int count = player.World.Game.Players.Count;
				if (count <= 0 || _adjustedPlayers == count) return true;
				Attributes[GameAttributes.Damage_Weapon_Min, 0] = _nativeDmg * (1f + (0.05f * (count - 1) * player.World.Game.Difficulty));
				Attributes[GameAttributes.Hitpoints_Max] = _nativeHp * (1f + ((0.75f + (0.1f * player.World.Game.Difficulty)) * (count - 1)));
				Attributes[GameAttributes.Hitpoints_Cur] = Attributes[GameAttributes.Hitpoints_Max_Total];
				Attributes.BroadcastChangedIfRevealed();
				_adjustedPlayers = count;
			}

			return true;

		}

		public override void EnterWorld(Vector3D position)
		{
			base.EnterWorld(position);
			if (!Spawner)
				if (BasePoint == null)
					BasePoint = position;

			if (SNO == ActorSno._a3_battlefield_demonic_ballista) //ballistas hack
			{
				var ballistas = GetActorsInRange<Monster>(5f).Where(monster => monster.SNO == ActorSno._a3_battlefield_demonic_ballista);
				if (ballistas.Count() >= 2)
				{
					Destroy();
				}
			}
		}

		/// <summary>
		/// Plays lore for first death of this monster's death.
		/// </summary>
		public void PlayLore()
		{
			if (LoreSnoId != -1)
			{
				var players = GetPlayersInRange();
				if (players != null)
				{
					foreach (var player in players.Where(player => !player.HasLore(LoreSnoId)))
					{
						player.PlayLore(LoreSnoId, false);
					}
				}
			}
		}
	}
}
