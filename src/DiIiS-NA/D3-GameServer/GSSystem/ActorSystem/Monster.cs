
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
				if(this.SNO == ActorSno._x1_lr_boss_mistressofpain)
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
			this.Field2 = 0x8;
			this.GBHandle.Type = (int)ActorType.Monster; this.GBHandle.GBID = 1;
			this.Attributes[GameAttribute.TeamID] = 9;
			if (Monster.Id != -1)
				this.WalkSpeed = (Monster.Target as MonsterFF).AttributeModifiers[129];  
			//WalkSpeed /= 2f;
			
			this.Brain = new MonsterBrain(this);
			this.Attributes[GameAttribute.Attacks_Per_Second] = 1.2f;

			this.UpdateStats();
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{

		}

		public void UpdateStats()
		{
			var monsterLevels = (GameBalance)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.GameBalance][19760].Data;
			bool full_hp = (this.Attributes[GameAttribute.Hitpoints_Cur] == this.Attributes[GameAttribute.Hitpoints_Max_Total]);
			this.Attributes[GameAttribute.Level] = this.World.Game.MonsterLevel;
			//this.Attributes[GameAttribute.Hitpoints_Max] = (int)monsterLevels.MonsterLevel[this.World.Game.MonsterLevel - 1].HPMin * (int)this.HPMultiplier * (int)this.World.Game.HPModifier;
			int MonsterLevel = 1;
			if (this.World.Game.ConnectedPlayers.Count > 1)
				MonsterLevel = this.World.Game.ConnectedPlayers[0].Level;
			else
				MonsterLevel = this.World.Game.InitialMonsterLevel;


			this.Attributes[GameAttribute.Hitpoints_Max] = (int)((int)monsterLevels.MonsterLevel[MonsterLevel].HPMin + DiIiS_NA.Core.Helpers.Math.RandomHelper.Next(0, (int)monsterLevels.MonsterLevel[MonsterLevel].HPDelta) * this.HPMultiplier * this.World.Game.HPModifier);
			this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = ((int)this.World.Game.ConnectedPlayers.Count + 1) * 1.5f;
			this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] *= DiIiS_NA.GameServer.Config.Instance.RateMonsterHP;
			if (this.World.Game.ConnectedPlayers.Count > 1)
				this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative];// / 2f;
			var HPM = this.Attributes[GameAttribute.Hitpoints_Max];
			var HPMT = this.Attributes[GameAttribute.Hitpoints_Max_Total];
			float DamageMin = monsterLevels.MonsterLevel[this.World.Game.MonsterLevel].Dmg * this.DmgMultiplier;// * 0.5f;
			float DamageDelta = DamageMin;
			this.Attributes[GameAttribute.Damage_Weapon_Min, 0] = DamageMin * this.World.Game.DmgModifier * DiIiS_NA.GameServer.Config.Instance.RateMonsterDMG;
			this.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = DamageDelta;

			if (MonsterLevel > 30)
			{
				this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative];// * 0.5f;
				this.Attributes[GameAttribute.Damage_Weapon_Min, 0] = DamageMin * this.World.Game.DmgModifier * DiIiS_NA.GameServer.Config.Instance.RateMonsterDMG;// * 0.2f;
				this.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = DamageDelta;
			}
			if (MonsterLevel > 60)
			{
				this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative];// * 0.7f;
				this.Attributes[GameAttribute.Damage_Weapon_Min, 0] = DamageMin * this.World.Game.DmgModifier * DiIiS_NA.GameServer.Config.Instance.RateMonsterDMG;// * 0.15f;
				//this.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = DamageDelta * 0.5f;
			}

			this._nativeHP = this.Attributes[GameAttribute.Hitpoints_Max_Total];
			this._nativeDmg = this.Attributes[GameAttribute.Damage_Weapon_Min, 0];
			//if (full_hp)
			this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max_Total];

			this.Attributes.BroadcastChangedIfRevealed();
		}

		int _BleedFirstTick = 0;
		int _CaltropsFirstTick = 0;

		public void Update(int tickCounter)
		{
			if (this.DestroyTimer != null)
			{
				Logger.Trace("Killed monster destroy timer update");
				this.DestroyTimer.Update(tickCounter);
			}

			if (this.Brain == null)
				return;
			if (this.World == null)
				return;
			this.Brain.Update(tickCounter);
			
			if (this.World.SNO == WorldSno.a4dun_diablo_arena)
				if (this.SNO == ActorSno._diablo)
					if (this.Attributes[GameAttribute.Hitpoints_Cur] < (this.Attributes[GameAttribute.Hitpoints_Max_Total] / 2))
					{
						this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max_Total];
						this.World.Game.QuestManager.Advance();//advancing United Evil quest
						var nextWorld = this.World.Game.GetWorld(WorldSno.a4dun_diablo_shadowrealm_01);
						foreach (var plr in this.World.Players.Values)
							plr.ChangeWorld(nextWorld, nextWorld.GetStartingPointById(172).Position);
					}

			if (this is Boss)
			{
				if (!this.World.BuffManager.HasBuff<PowerSystem.Implementations.Caltrops.ActiveCalTrops>(this))
					_CaltropsFirstTick = tickCounter;

				if ((tickCounter - _CaltropsFirstTick) >= 2400)
				{
					var buff_owner = this.World.BuffManager.GetFirstBuff<PowerSystem.Implementations.Caltrops.ActiveCalTrops>(this).User;
					if (buff_owner is Player)
						(buff_owner as Player).GrantAchievement(74987243307067);
				}

			}
			if (!this.World.BuffManager.HasBuff<PowerSystem.Implementations.Rend.RendDebuff>(this))
				_BleedFirstTick = tickCounter;

			if ((tickCounter - _BleedFirstTick) >= 1200)
			{
				var buff_owner = this.World.BuffManager.GetFirstBuff<PowerSystem.Implementations.Rend.RendDebuff>(this).User;
				if (buff_owner is Player)
					(buff_owner as Player).GrantAchievement(74987243307052);
			}
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			

			lock (this.adjustLock)
			{
				int count = player.World.Game.Players.Count;
				if (count > 0 && this.AdjustedPlayers != count)
				{
					this.Attributes[GameAttribute.Damage_Weapon_Min, 0] = this._nativeDmg * (1f + (0.05f * (count - 1) * player.World.Game.Difficulty));
					this.Attributes[GameAttribute.Hitpoints_Max] = this._nativeHP * (1f + ((0.75f + (0.1f * player.World.Game.Difficulty)) * (count - 1)));
					this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max_Total];
					this.Attributes.BroadcastChangedIfRevealed();
					this.AdjustedPlayers = count;
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

			if (this.SNO == ActorSno._a3_battlefield_demonic_ballista) //ballistas hack
			{
				var ballistas = this.GetActorsInRange<Monster>(5f).Where(monster => monster.SNO == ActorSno._a3_battlefield_demonic_ballista);
				if (ballistas.Count() >= 2)
				{
					this.Destroy();
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
				var players = this.GetPlayersInRange();
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
