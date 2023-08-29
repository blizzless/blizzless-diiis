
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

		public int MonsterType => Monster.IsValid ? (int)((MonsterFF)Monster.Target).Type : -1;

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
			var monsterLevels = (GameBalance)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.GameBalance][19760].Data;
			bool fullHp = (Math.Abs(Attributes[GameAttributes.Hitpoints_Cur] - Attributes[GameAttributes.Hitpoints_Max_Total]) < Globals.FLOAT_TOLERANCE);
			Attributes[GameAttributes.Level] = World.Game.MonsterLevel;
			//this.Attributes[GameAttribute.Hitpoints_Max] = (int)monsterLevels.MonsterLevel[this.World.Game.MonsterLevel - 1].HPMin * (int)this.HPMultiplier * (int)this.World.Game.HPModifier;
			int monsterLevel = 1;
			monsterLevel = World.Game.ConnectedPlayers.Length > 1 ? World.Game.ConnectedPlayers[0].Level : World.Game.InitialMonsterLevel;


			Attributes[GameAttributes.Hitpoints_Max] = (int)((int)monsterLevels.MonsterLevel[monsterLevel].HPMin + DiIiS_NA.Core.Helpers.Math.RandomHelper.Next(0, (int)monsterLevels.MonsterLevel[monsterLevel].HPDelta) * HpMultiplier * World.Game.HpModifier);
			Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus_Multiplicative] = ((int)World.Game.ConnectedPlayers.Length + 1) * 1.5f;
			Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus_Multiplicative] *= GameModsConfig.Instance.Monster.HealthMultiplier;
			if (World.Game.ConnectedPlayers.Length > 1)
				Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus_Multiplicative] = Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus_Multiplicative];// / 2f;
			var hpMax = Attributes[GameAttributes.Hitpoints_Max];
			var hpTotal = Attributes[GameAttributes.Hitpoints_Max_Total];
			float damageMin = monsterLevels.MonsterLevel[World.Game.MonsterLevel].Dmg * DmgMultiplier;// * 0.5f;
			float damageDelta = damageMin;
			Attributes[GameAttributes.Damage_Weapon_Min, 0] = damageMin * World.Game.DmgModifier * GameModsConfig.Instance.Monster.DamageMultiplier;
			Attributes[GameAttributes.Damage_Weapon_Delta, 0] = damageDelta;

			if (monsterLevel > 30)
			{
				Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus_Multiplicative] = Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus_Multiplicative];// * 0.5f;
				Attributes[GameAttributes.Damage_Weapon_Min, 0] = damageMin * World.Game.DmgModifier * GameModsConfig.Instance.Monster.DamageMultiplier;// * 0.2f;
				Attributes[GameAttributes.Damage_Weapon_Delta, 0] = damageDelta;
			}
			if (monsterLevel > 60)
			{
				Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus_Multiplicative] = Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus_Multiplicative];// * 0.7f;
				Attributes[GameAttributes.Damage_Weapon_Min, 0] = damageMin * World.Game.DmgModifier * GameModsConfig.Instance.Monster.DamageMultiplier;// * 0.15f;
				//this.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = DamageDelta * 0.5f;
			}

			_nativeHp = Attributes[GameAttributes.Hitpoints_Max_Total];
			_nativeDmg = Attributes[GameAttributes.Damage_Weapon_Min, 0];
			//if (full_hp)
			Attributes[GameAttributes.Hitpoints_Cur] = Attributes[GameAttributes.Hitpoints_Max_Total];

			Attributes.BroadcastChangedIfRevealed();
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
