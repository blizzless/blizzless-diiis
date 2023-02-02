using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterFF = DiIiS_NA.Core.MPQ.FileFormats.Monster;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Pet;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

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
				return (int)SpawnType.Normal;
			}
			set
			{
				// Not implemented
			}
		}

		public float PrimaryAttribute = 0;

		public Minion(MapSystem.World world, ActorSno sno, Actor master, TagMap tags, bool QuestFollow = false, bool Revived = false)
			: base(world, sno, tags)
		{
			// The following two seems to be shared with monsters. One wonders why there isn't a specific actortype for minions.
			Master = master;
			Field2 = 0x8;
			GBHandle.Type = (int)ActorType.Monster; GBHandle.GBID = 1;
			QuestFollower = QuestFollow;
			var monsterLevels = (GameBalance)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[SNOGroup.GameBalance][19760].Data;
			if (Revived)
				LifeTime = TickTimer.WaitSeconds(world.Game, 15f);
			Attributes[GameAttribute.Level] = master.Attributes[GameAttribute.Level];
			if (!QuestFollower)
			{
				//this.Attributes[GameAttribute.Hitpoints_Max] = monsterLevels.MonsterLevel[this.Attributes[GameAttribute.Level]].F0;
				Attributes[GameAttribute.Hitpoints_Max] = 1000f + (Attributes[GameAttribute.Level] * 150f) + (Attributes[GameAttribute.Alt_Level] * 150f) + (master.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.35f);
				Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max];
				Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = 1;
				Attributes[GameAttribute.Hitpoints_Regen_Per_Second] = 1f;
			}
			Attributes[GameAttribute.Weapon_Crit_Chance] = master.Attributes[GameAttribute.Weapon_Crit_Chance];
			Attributes[GameAttribute.Crit_Damage_Percent] = master.Attributes[GameAttribute.Crit_Damage_Percent];
			Attributes[GameAttribute.Crit_Percent_Bonus_Capped] = master.Attributes[GameAttribute.Crit_Percent_Bonus_Capped];
			//this.Attributes[GameAttribute.Attack_Percent] = master.Attributes[GameAttribute.Attack_Bonus_Percent];
			Attributes[GameAttribute.Damage_Weapon_Min, 0] = master.Attributes[GameAttribute.Damage_Weapon_Min_Total, 0];
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = master.Attributes[GameAttribute.Damage_Weapon_Delta_Total, 0];

			if (Master is Player)
			{
				PrimaryAttribute = (Master as Player).PrimaryAttribute;
			}

			_lastResourceUpdateTick = 0;

			if (master != null)
			{
				//this.Attributes[GameAttribute.Summoned_By_ACDID] = (int)master.DynamicID;
				Attributes[GameAttribute.TeamID] = master.Attributes[GameAttribute.TeamID];
				if (master is Player)
				{
					if ((master as Player).Followers.Values.Count(a => a == sno) >= SummonLimit)
						(master as Player).DestroyFollower(sno);

					(master as Player).SetFollowerIndex(sno);
					(master as Player).Followers.Add(GlobalID, sno);
				}
			}
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
		}

		public void Update(int tickCounter)
		{
			if (Brain == null || World == null)
				return;

			Brain.Update(tickCounter);

			if (World.Game.TickCounter % 30 == 0 && !Dead)
			{
				float tickSeconds = 1f / 60f * (World.Game.TickCounter - _lastResourceUpdateTick);
				_lastResourceUpdateTick = World.Game.TickCounter;
				if (!QuestFollower)
				{
					float quantity = tickSeconds * Attributes[GameAttribute.Hitpoints_Regen_Per_Second];

					if (Attributes[GameAttribute.Hitpoints_Cur] < Attributes[GameAttribute.Hitpoints_Max_Total])
					{
						Attributes[GameAttribute.Hitpoints_Cur] = Math.Min(
							Attributes[GameAttribute.Hitpoints_Cur] + quantity,
							Attributes[GameAttribute.Hitpoints_Max_Total]);

						Attributes.BroadcastChangedIfRevealed();
					}
				}
			}

			if (LifeTime != null && LifeTime.TimedOut)
			{
				if (Master != null && Master is Player)
				{
					(Master as Player).Followers.Remove(GlobalID);
					(Master as Player).FreeFollowerIndex(SNO);
					(Master as Player).Revived.Remove(this);
				}
				(Master as Player).InGameClient.SendMessage(new PetDetachMessage()
				{
					PetId = DynamicID(Master as Player)
				});
				if (this is SkeletalMage)
				{
					if ((this as SkeletalMage).Rune_Flesh)
						World.SpawnMonster(ActorSno._p6_necro_corpse_flesh, Position);
				}
				
				Destroy();
				//PetAwayMessage
				//this.Kill();
				return;
			}
		}

		public void SetBrain(AISystem.Brain brain)
		{
			Brain = brain;
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
					PetId = DynamicID(player),
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

				

				if (player.GlobalID == Master.GlobalID && !(this is CorpseSpider || this is CorpseSpiderQueen))
					player.InGameClient.SendMessage(new PetMessage()
					{
						Owner = player.PlayerIndex,
						Index = isGolem ? 9 : player.CountFollowers(SNO) + PlusIndex,
						PetId = DynamicID(player),
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
					PetId = DynamicID(player),
				});
			}

			return true;
		}
	}
}
