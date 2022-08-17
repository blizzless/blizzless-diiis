//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
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

		private bool haveDrop;

		public DesctructibleLootContainer(World world, int snoId, bool haveDrop, TagMap tags)
			: base(world, snoId, tags)
		{
			this.haveDrop = haveDrop;
		}

		private List<int> tombs = new List<int>() { 6155, 6158, 6159, 6156, 74909, 75023, 75132 };

		private int[] Unbreakables = new int[] { 81699, 5744, 89503 };

		public void ReceiveDamage(Actor source, float damage /* critical, type */)
		{
			if (this.ActorSNO.Id == 225252 && this.World.Game.CurrentSideQuest != 225253) return;

			World.BroadcastIfRevealed(plr => new FloatingNumberMessage
			{
				Number = damage,
				ActorID = this.DynamicID(plr),
				Type = FloatingNumberMessage.FloatType.White
			}, this);

			Attributes[GameAttribute.Hitpoints_Cur] = Math.Max(Attributes[GameAttribute.Hitpoints_Cur] - damage, 0);
			//Attributes[GameAttribute.Last_Damage_ACD] = unchecked((int)source.DynamicID);

			Attributes.BroadcastChangedIfRevealed();

			if (Attributes[GameAttribute.Hitpoints_Cur] == 0 && !this.Unbreakables.Contains(this.ActorSNO.Id))
			{
				Die(source);
			}
		}

		public void Die(Actor source = null)
		{
			base.OnTargeted(null, null);
			if (haveDrop)
			{
				var dropRates = this.World.Game.IsHardcore ? LootManager.GetSeasonalDropRates((int)this.Quality, 70) : LootManager.GetDropRates((int)this.Quality, 70);
				foreach (var rate in dropRates)
					foreach (var plr in this.GetPlayersInRange(30))
					{
						float seed = (float)FastRandom.Instance.NextDouble();
						if (seed < 0.95f)
							this.World.SpawnGold(this, plr);
						if (seed < 0.06f)
							this.World.SpawnRandomCraftItem(this, plr);
						if (seed < 0.04f)
							this.World.SpawnRandomGem(this, plr);
						if (seed < 0.10f)
							this.World.SpawnRandomPotion(this, plr);
						if (seed < (rate * (1f + plr.Attributes[GameAttribute.Magic_Find])))
						{
							var lootQuality = this.World.Game.IsHardcore ? LootManager.GetSeasonalLootQuality((int)this.Quality, this.World.Game.Difficulty) : LootManager.GetLootQuality((int)this.Quality, this.World.Game.Difficulty);
							this.World.SpawnRandomEquip(plr, plr, lootQuality);
						}
						else
							break;
					}
			}

			Logger.Trace("Breaked barricade, id: {0}", this.ActorSNO.Id);

			if (source != null && source is Player && this.tombs.Contains(this.ActorSNO.Id))
			{
				(source as Player).AddAchievementCounter(74987243307171, 1);
			}

			if (this.AnimationSet.TagMapAnimDefault.ContainsKey(AnimationSetKeys.DeathDefault))
				World.BroadcastIfRevealed(plr => new PlayAnimationMessage
				{
					ActorID = this.DynamicID(plr),
					AnimReason = 11,
					UnitAniimStartTime = 0,
					tAnim = new PlayAnimationMessageSpec[]
					{
						new PlayAnimationMessageSpec()
						{
							Duration = 10,
							AnimationSNO = AnimationSet.TagMapAnimDefault[AnimationSetKeys.DeathDefault],
							PermutationIndex = 0,
							AnimationTag = 0,
							Speed = 1
						}
					}

				}, this);

			this.Attributes[GameAttribute.Deleted_On_Server] = true;
			this.Attributes[GameAttribute.Could_Have_Ragdolled] = true;
			Attributes.BroadcastChangedIfRevealed();

			//handling quest triggers
			if (this.World.Game.QuestProgress.QuestTriggers.ContainsKey(this.ActorSNO.Id))
			{
				var trigger = this.World.Game.QuestProgress.QuestTriggers[this.ActorSNO.Id];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillMonster)
				{
					this.World.Game.QuestProgress.UpdateCounter(this.ActorSNO.Id);
					if (trigger.count == this.World.Game.QuestProgress.QuestTriggers[this.ActorSNO.Id].counter)
						trigger.questEvent.Execute(this.World); // launch a questEvent
				}
				else
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.MonsterFromGroup)
				{
					this.World.Game.QuestProgress.UpdateCounter(this.ActorSNO.Id);
				}
			}
			else if (this.World.Game.SideQuestProgress.QuestTriggers.ContainsKey(this.ActorSNO.Id))
			{
				var trigger = this.World.Game.SideQuestProgress.QuestTriggers[this.ActorSNO.Id];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillMonster)
				{
					this.World.Game.SideQuestProgress.UpdateSideCounter(this.ActorSNO.Id);
					if (trigger.count == this.World.Game.SideQuestProgress.QuestTriggers[this.ActorSNO.Id].counter)
						trigger.questEvent.Execute(this.World); // launch a questEvent
				}
			}

			this.Destroy();
		}


		public override void OnTargeted(Player player, TargetMessage message)
		{
			base.OnTargeted(player, message);
			ReceiveDamage(player, 100);
		}
	}
}
