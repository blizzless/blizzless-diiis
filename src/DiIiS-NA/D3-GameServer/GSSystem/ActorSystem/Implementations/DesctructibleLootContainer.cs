//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	class DesctructibleLootContainer : Gizmo
	{
		private static readonly ActorSno[] tombs = new ActorSno[]
		{
			ActorSno._trout_oldtristramtombstonedestructiblea,
			ActorSno._trout_oldtristramtombstonedestructibled,
			ActorSno._trout_oldtristramtombstonedestructiblee,
			ActorSno._trout_oldtristramtombstonedestructibleb,
			ActorSno._tombstone_a_wilderness_trout_wilderness,
			ActorSno._tombstone_c_wilderness_trout_wilderness,
			ActorSno._tombstone_b_wilderness_trout_wilderness
		};

		private bool haveDrop;

		public DesctructibleLootContainer(World world, ActorSno sno, bool haveDrop, TagMap tags)
			: base(world, sno, tags)
		{
			this.haveDrop = haveDrop;
		}

		public void ReceiveDamage(Actor source, float damage /* critical, type */)
		{
			if (SNO == ActorSno._trout_highlands_goatman_totem_gharbad && World.Game.CurrentSideQuest != 225253) return;

			World.BroadcastIfRevealed(plr => new FloatingNumberMessage
			{
				Number = damage,
				ActorID = DynamicID(plr),
				Type = FloatingNumberMessage.FloatType.White
			}, this);

			Attributes[GameAttribute.Hitpoints_Cur] = Math.Max(Attributes[GameAttribute.Hitpoints_Cur] - damage, 0);
			//Attributes[GameAttribute.Last_Damage_ACD] = unchecked((int)source.DynamicID);

			Attributes.BroadcastChangedIfRevealed();

			if (Attributes[GameAttribute.Hitpoints_Cur] == 0 && !SNO.IsUndestroyable())
			{
				Die(source);
			}
		}

		public void Die(Actor source = null)
		{
			base.OnTargeted(null, null);
			if (haveDrop)
			{
				var dropRates = World.Game.IsHardcore ? LootManager.GetSeasonalDropRates((int)Quality, 70) : LootManager.GetDropRates((int)Quality, 70);
				foreach (var rate in dropRates)
					foreach (var plr in GetPlayersInRange(30))
					{
						float seed = (float)FastRandom.Instance.NextDouble();
						if (seed < 0.95f)
							World.SpawnGold(this, plr);
						if (seed < 0.06f)
							World.SpawnRandomCraftItem(this, plr);
						if (seed < 0.04f)
							World.SpawnRandomGem(this, plr);
						if (seed < 0.10f)
							World.SpawnRandomPotion(this, plr);
						if (seed < (rate * (1f + plr.Attributes[GameAttribute.Magic_Find])))
						{
							var lootQuality = World.Game.IsHardcore ? LootManager.GetSeasonalLootQuality((int)Quality, World.Game.Difficulty) : LootManager.GetLootQuality((int)Quality, World.Game.Difficulty);
							World.SpawnRandomEquip(plr, plr, lootQuality);
						}
						else
							break;
					}
			}

			Logger.Trace("Breaked barricade, id: {0}", SNO);

			if (source != null && source is Player && tombs.Contains(SNO))
			{
				(source as Player).AddAchievementCounter(74987243307171, 1);
			}

			if (this.AnimationSet.Animations.ContainsKey(AnimationSetKeys.DeathDefault.ID))
				World.BroadcastIfRevealed(plr => new PlayAnimationMessage
				{
					ActorID = DynamicID(plr),
					AnimReason = 11,
					UnitAniimStartTime = 0,
					tAnim = new PlayAnimationMessageSpec[]
					{
						new PlayAnimationMessageSpec()
						{
							Duration = 10,
							AnimationSNO = (int)AnimationSet.Animations[AnimationSetKeys.DeathDefault.ID],
							PermutationIndex = 0,
							AnimationTag = 0,
							Speed = 1
						}
					}

				}, this);

			Attributes[GameAttribute.Deleted_On_Server] = true;
			Attributes[GameAttribute.Could_Have_Ragdolled] = true;
			Attributes.BroadcastChangedIfRevealed();

			//handling quest triggers
			if (World.Game.QuestProgress.QuestTriggers.ContainsKey((int)SNO))
			{
				var trigger = World.Game.QuestProgress.QuestTriggers[(int)SNO];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillMonster)
				{
					World.Game.QuestProgress.UpdateCounter((int)SNO);
					if (trigger.count == World.Game.QuestProgress.QuestTriggers[(int)SNO].counter)
						trigger.questEvent.Execute(World); // launch a questEvent
				}
				else
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.MonsterFromGroup)
				{
					World.Game.QuestProgress.UpdateCounter((int)SNO);
				}
			}
			else if (World.Game.SideQuestProgress.QuestTriggers.ContainsKey((int)SNO))
			{
				var trigger = World.Game.SideQuestProgress.QuestTriggers[(int)SNO];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillMonster)
				{
					World.Game.SideQuestProgress.UpdateSideCounter((int)SNO);
					if (trigger.count == World.Game.SideQuestProgress.QuestTriggers[(int)SNO].counter)
						trigger.questEvent.Execute(World); // launch a questEvent
				}
			}

			Destroy();
		}


		public override void OnTargeted(Player player, TargetMessage message)
		{
			base.OnTargeted(player, message);
			ReceiveDamage(player, 100);
		}
	}
}
