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
//Blizzless Project 2022 
using MonsterFF = DiIiS_NA.Core.MPQ.FileFormats.Monster;
//Blizzless Project 2022 
using ActorFF = DiIiS_NA.Core.MPQ.FileFormats.Actor;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Pet;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class Minion : Living, IUpdateable
	{
		public Actor Master; //The player who summoned the minion.

		public override ActorType ActorType { get { return ActorType.Monster; } }

		public int SummonLimit = 10;

		public TickTimer LifeTime = null;

		public float DamageCoefficient = 1f;

		public float CooldownReduction = 1f;

		public bool QuestFollower = false;
		// Resource generation timing
		private int _lastResourceUpdateTick;

		public override int Quality
		{
			get
			{
				return (int)DiIiS_NA.Core.MPQ.FileFormats.SpawnType.Normal;
			}
			set
			{
				// Not implemented
			}
		}

		public float PrimaryAttribute = 0;

		public Minion(MapSystem.World world, int snoId, Actor master, TagMap tags, bool QuestFollow = false, bool Revived = false)
			: base(world, snoId, tags)
		{
			// The following two seems to be shared with monsters. One wonders why there isn't a specific actortype for minions.
			this.Master = master;
			this.Field2 = 0x8;
			this.GBHandle.Type = (int)ActorType.Monster; this.GBHandle.GBID = 1;
			QuestFollower = QuestFollow;
			var monsterLevels = (GameBalance)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.GameBalance][19760].Data;
			if (Revived)
				LifeTime = TickTimer.WaitSeconds(world.Game, 15f);
			this.Attributes[GameAttribute.Level] = master.Attributes[GameAttribute.Level];
			if (!QuestFollower)
			{
				//this.Attributes[GameAttribute.Hitpoints_Max] = monsterLevels.MonsterLevel[this.Attributes[GameAttribute.Level]].F0;
				this.Attributes[GameAttribute.Hitpoints_Max] = 1000f + (this.Attributes[GameAttribute.Level] * 150f) + (this.Attributes[GameAttribute.Alt_Level] * 150f) + (master.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.35f);
				this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max];
				this.Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = 1;
				this.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] = 1f;
			}
			this.Attributes[GameAttribute.Weapon_Crit_Chance] = master.Attributes[GameAttribute.Weapon_Crit_Chance];
			this.Attributes[GameAttribute.Crit_Damage_Percent] = master.Attributes[GameAttribute.Crit_Damage_Percent];
			this.Attributes[GameAttribute.Crit_Percent_Bonus_Capped] = master.Attributes[GameAttribute.Crit_Percent_Bonus_Capped];
			//this.Attributes[GameAttribute.Attack_Percent] = master.Attributes[GameAttribute.Attack_Bonus_Percent];
			this.Attributes[GameAttribute.Damage_Weapon_Min, 0] = master.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			this.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = master.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			if (this.Master is Player)
			{
				this.PrimaryAttribute = (this.Master as Player).PrimaryAttribute;
			}

			this._lastResourceUpdateTick = 0;

			if (master != null)
			{
				//this.Attributes[GameAttribute.Summoned_By_ACDID] = (int)master.DynamicID;
				this.Attributes[GameAttribute.TeamID] = master.Attributes[GameAttribute.TeamID];
				if (master is Player)
				{
					if ((master as Player).Followers.Values.Count(a => a == snoId) >= this.SummonLimit)
						(master as Player).DestroyFollower(snoId);

					(master as Player).SetFollowerIndex(snoId);
					(master as Player).Followers.Add(this.GlobalID, snoId);
				}
			}
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
		}

		public void Update(int tickCounter)
		{
			if (this.Brain == null || this.World == null)
				return;

			this.Brain.Update(tickCounter);

			if (this.World.Game.TickCounter % 30 == 0 && !this.Dead)
			{
				float tickSeconds = 1f / 60f * (this.World.Game.TickCounter - _lastResourceUpdateTick);
				_lastResourceUpdateTick = this.World.Game.TickCounter;
				if (!QuestFollower)
				{
					float quantity = tickSeconds * this.Attributes[GameAttribute.Hitpoints_Regen_Per_Second];

					if (this.Attributes[GameAttribute.Hitpoints_Cur] < this.Attributes[GameAttribute.Hitpoints_Max_Total])
					{
						this.Attributes[GameAttribute.Hitpoints_Cur] = Math.Min(
							this.Attributes[GameAttribute.Hitpoints_Cur] + quantity,
							this.Attributes[GameAttribute.Hitpoints_Max_Total]);

						this.Attributes.BroadcastChangedIfRevealed();
					}
				}
			}

			if (this.LifeTime != null && this.LifeTime.TimedOut)
			{
				if (this.Master != null && this.Master is Player)
				{
					(this.Master as Player).Followers.Remove(this.GlobalID);
					(this.Master as Player).FreeFollowerIndex(this.ActorSNO.Id);
					(this.Master as Player).Revived.Remove(this);
				}
				(this.Master as Player).InGameClient.SendMessage(new PetDetachMessage()
				{
					PetId = this.DynamicID(this.Master as Player)
				});
				if (this is SkeletalMage)
				{
					if ((this as SkeletalMage).Rune_Flesh)
						this.World.SpawnMonster(454066, this.Position);
				}
				
				this.Destroy();
				//PetAwayMessage
				//this.Kill();
				return;
			}
		}

		public void SetBrain(AISystem.Brain brain)
		{
			this.Brain = brain;
		}

		public void AddPresetPower(int powerSNO)
		{
			(Brain as MinionBrain).AddPresetPower(powerSNO);
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;
			if (QuestFollower)
			{
				player.InGameClient.SendMessage(new PetMessage()
				{
					Owner = player.PlayerIndex,
					Index = 1,
					PetId = this.DynamicID(player),
					Type = 0x18,
				});
			}
			else
			{
				int TypeID = 7;
				int PlusIndex = 0;
				bool isGolem = false;
				if (this is BaseGolem ||
					this is IceGolem ||
					this is BoneGolem ||
					this is DecayGolem ||
					this is ConsumeFleshGolem ||
					this is BloodGolem)
				{
					TypeID = 27;
					isGolem = false;
				}
				if (this is SkeletalMage)
				{
					PlusIndex += 10;
					TypeID = 9;
				}
				if (this is NecromancerSkeleton_A)
					TypeID = 28;

				

				if (player.GlobalID == this.Master.GlobalID && !(this is CorpseSpider || this is CorpseSpiderQueen))
					player.InGameClient.SendMessage(new PetMessage()
					{
						Owner = player.PlayerIndex,
						Index = isGolem ? 9 : player.CountFollowers(this.ActorSNO.Id) + PlusIndex,
						PetId = this.DynamicID(player),
						Type = TypeID,
					});
			}
			return true;
		}
		public override bool Unreveal(Player player)
		{
			if (!base.Unreveal(player))
				return false;

			if (QuestFollower)
			{
				if(IsRevealedToPlayer(player))
				player.InGameClient.SendMessage(new PetDetachMessage()
				{
					PetId = this.DynamicID(player),
				});
			}
			else
			{
				int TypeID = 7;
				int PlusIndex = 0;
				bool isGolem = false;
				if (this is BaseGolem ||
					this is IceGolem ||
					this is BoneGolem ||
					this is DecayGolem ||
					this is ConsumeFleshGolem ||
					this is BloodGolem)
				{
					TypeID = 27;
					isGolem = false;
				}
				if (this is SkeletalMage)
				{
					PlusIndex += 10;
					TypeID = 9;
				}
				if (this is NecromancerSkeleton_A)
					TypeID = 28;
			}

			return true;
		}
	}
}
