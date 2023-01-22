﻿
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using MonsterFF = DiIiS_NA.Core.MPQ.FileFormats.Monster;
//Blizzless Project 2022 
using GameBalance = DiIiS_NA.Core.MPQ.FileFormats.GameBalance;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class Monster : Living, IUpdateable
	{
		public override ActorType ActorType { get { return ActorType.Monster; } }

		static readonly Logger Logger = LogManager.CreateLogger();

		public TickTimer DestroyTimer;

		private int AdjustedPlayers = 1;
		private object adjustLock = new object();
		private float _nativeHP = 0f;
		private float _nativeDmg = 0f;
		public Vector3D CorrectedPosition = null;
		public override int Quality
		{
			get
			{
				if(SNO == ActorSno._x1_lr_boss_mistressofpain)
					return 7;
				return (int)DiIiS_NA.Core.MPQ.FileFormats.SpawnType.Normal;
				//return (int)Mooege.Common.MPQ.FileFormats.SpawnType.Champion;
				//return (int)Mooege.Common.MPQ.FileFormats.SpawnType.Rare;
				//return (int)Mooege.Common.MPQ.FileFormats.SpawnType.Minion;
				//return (int)Mooege.Common.MPQ.FileFormats.SpawnType.Unique;
				//return (int)Mooege.Common.MPQ.FileFormats.SpawnType.Hireling;
				//return (int)Mooege.Common.MPQ.FileFormats.SpawnType.Clone;
				//return (int)Mooege.Common.MPQ.FileFormats.SpawnType.Boss;
			}
			set
			{
			}
		}

		public int LoreSNOId
		{
			get
			{
				return Monster.IsValid ? (Monster.Target as MonsterFF).SNOLore : -1;
			}
		}

		public int MonsterType
		{
			get
			{
				return Monster.IsValid ? (int)(Monster.Target as MonsterFF).Type : -1;
			}
		}

		public float HPMultiplier
		{
			get
			{
				return Monster.IsValid ? (1f + (Monster.Target as MonsterFF).AttributeModifiers[4]) : 1f;
			}
		}

		public float DmgMultiplier
		{
			get
			{
				return Monster.IsValid ? (1f + (Monster.Target as MonsterFF).AttributeModifiers[55]) : 1f;
			}
		}

		/// <summary>
		/// Gets the Actors summoning fields from the mpq's and returns them in format for Monsters.
		/// Useful for the Monsters spawning/summoning skills.
		/// </summary>
		public ActorSno[] SNOSummons
		{
			get
			{
				return (Monster.Target as MonsterFF).SNOSummonActor.Select(x => (ActorSno)x).ToArray();
			}
		}

		public Monster(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Field2 = 0x8;
			GBHandle.Type = (int)ActorType.Monster; GBHandle.GBID = 1;
			Attributes[GameAttribute.TeamID] = 9;
			if (Monster.Id != -1)
				WalkSpeed = (Monster.Target as MonsterFF).AttributeModifiers[129];  
			//WalkSpeed /= 2f;
			
			Brain = new MonsterBrain(this);
			Attributes[GameAttribute.Attacks_Per_Second] = 1.2f;

			UpdateStats();
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{

		}

		public void UpdateStats()
		{
			var monsterLevels = (GameBalance)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.GameBalance][19760].Data;
			bool full_hp = (Attributes[GameAttribute.Hitpoints_Cur] == Attributes[GameAttribute.Hitpoints_Max_Total]);
			Attributes[GameAttribute.Level] = World.Game.MonsterLevel;
			//this.Attributes[GameAttribute.Hitpoints_Max] = (int)monsterLevels.MonsterLevel[this.World.Game.MonsterLevel - 1].HPMin * (int)this.HPMultiplier * (int)this.World.Game.HPModifier;
			int MonsterLevel = 1;
			if (World.Game.ConnectedPlayers.Count > 1)
				MonsterLevel = World.Game.ConnectedPlayers[0].Level;
			else
				MonsterLevel = World.Game.InitialMonsterLevel;


			Attributes[GameAttribute.Hitpoints_Max] = (int)((int)monsterLevels.MonsterLevel[MonsterLevel].HPMin + DiIiS_NA.Core.Helpers.Math.RandomHelper.Next(0, (int)monsterLevels.MonsterLevel[MonsterLevel].HPDelta) * HPMultiplier * World.Game.HPModifier);
			Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = ((int)World.Game.ConnectedPlayers.Count + 1) * 1.5f;
			Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] *= Config.Instance.RateMonsterHP;
			if (World.Game.ConnectedPlayers.Count > 1)
				Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative];// / 2f;
			var HPM = Attributes[GameAttribute.Hitpoints_Max];
			var HPMT = Attributes[GameAttribute.Hitpoints_Max_Total];
			float DamageMin = monsterLevels.MonsterLevel[World.Game.MonsterLevel].Dmg * DmgMultiplier;// * 0.5f;
			float DamageDelta = DamageMin;
			Attributes[GameAttribute.Damage_Weapon_Min, 0] = DamageMin * World.Game.DmgModifier * Config.Instance.RateMonsterDMG;
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = DamageDelta;

			if (MonsterLevel > 30)
			{
				Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative];// * 0.5f;
				Attributes[GameAttribute.Damage_Weapon_Min, 0] = DamageMin * World.Game.DmgModifier * Config.Instance.RateMonsterDMG;// * 0.2f;
				Attributes[GameAttribute.Damage_Weapon_Delta, 0] = DamageDelta;
			}
			if (MonsterLevel > 60)
			{
				Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative];// * 0.7f;
				Attributes[GameAttribute.Damage_Weapon_Min, 0] = DamageMin * World.Game.DmgModifier * Config.Instance.RateMonsterDMG;// * 0.15f;
				//this.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = DamageDelta * 0.5f;
			}

			_nativeHP = Attributes[GameAttribute.Hitpoints_Max_Total];
			_nativeDmg = Attributes[GameAttribute.Damage_Weapon_Min, 0];
			//if (full_hp)
			Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];

			Attributes.BroadcastChangedIfRevealed();
		}

		int _BleedFirstTick = 0;
		int _CaltropsFirstTick = 0;

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
					if (Attributes[GameAttribute.Hitpoints_Cur] < (Attributes[GameAttribute.Hitpoints_Max_Total] / 2))
					{
						Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];
						World.Game.QuestManager.Advance();//advancing United Evil quest
						var nextWorld = World.Game.GetWorld(WorldSno.a4dun_diablo_shadowrealm_01);
						foreach (var plr in World.Players.Values)
							plr.ChangeWorld(nextWorld, nextWorld.GetStartingPointById(172).Position);
					}

			if (this is Boss)
			{
				if (!World.BuffManager.HasBuff<PowerSystem.Implementations.Caltrops.ActiveCalTrops>(this))
					_CaltropsFirstTick = tickCounter;

				if ((tickCounter - _CaltropsFirstTick) >= 2400)
				{
					var buff_owner = World.BuffManager.GetFirstBuff<PowerSystem.Implementations.Caltrops.ActiveCalTrops>(this).User;
					if (buff_owner is Player)
						(buff_owner as Player).GrantAchievement(74987243307067);
				}

			}
			if (!World.BuffManager.HasBuff<PowerSystem.Implementations.Rend.RendDebuff>(this))
				_BleedFirstTick = tickCounter;

			if ((tickCounter - _BleedFirstTick) >= 1200)
			{
				var buff_owner = World.BuffManager.GetFirstBuff<PowerSystem.Implementations.Rend.RendDebuff>(this).User;
				if (buff_owner is Player)
					(buff_owner as Player).GrantAchievement(74987243307052);
			}
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			

			lock (adjustLock)
			{
				int count = player.World.Game.Players.Count;
				if (count > 0 && AdjustedPlayers != count)
				{
					Attributes[GameAttribute.Damage_Weapon_Min, 0] = _nativeDmg * (1f + (0.05f * (count - 1) * player.World.Game.Difficulty));
					Attributes[GameAttribute.Hitpoints_Max] = _nativeHP * (1f + ((0.75f + (0.1f * player.World.Game.Difficulty)) * (count - 1)));
					Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];
					Attributes.BroadcastChangedIfRevealed();
					AdjustedPlayers = count;
				}
			}

			return true;

		}
		public Vector3D basePoint = null;

		public override void EnterWorld(Vector3D position)
		{
			base.EnterWorld(position);
			if (!Spawner)
				if (basePoint == null)
					basePoint = position;

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
			if (LoreSNOId != -1)
			{
				var players = GetPlayersInRange();
				if (players != null)
				{
					foreach (var player in players.Where(player => !player.HasLore(LoreSNOId)))
					{
						player.PlayLore(LoreSNOId, false);
					}
				}
			}
		}
	}
}
