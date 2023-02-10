using System;
using System.Linq;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
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

			Attributes[GameAttributes.Hitpoints_Cur] = Math.Max(Attributes[GameAttributes.Hitpoints_Cur] - damage, 0);
			//Attributes[GameAttribute.Last_Damage_ACD] = unchecked((int)source.DynamicID);

			Attributes.BroadcastChangedIfRevealed();

			if (Attributes[GameAttributes.Hitpoints_Cur] == 0 && !SNO.IsUndestroyable())
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
						if (seed < (rate * (1f + plr.Attributes[GameAttributes.Magic_Find])))
						{
							var lootQuality = World.Game.IsHardcore ? LootManager.GetSeasonalLootQuality((int)Quality, World.Game.Difficulty) : LootManager.GetLootQuality((int)Quality, World.Game.Difficulty);
							World.SpawnRandomEquip(plr, plr, lootQuality);
						}
						else
							break;
					}
			}

			Logger.Trace("Breaked barricade, id: {0}", SNO);

			if (source is Player player && tombs.Contains(SNO))
			{
				player.AddAchievementCounter(74987243307171, 1);
			}

			if (AnimationSet.TagMapAnimDefault.ContainsKey(AnimationSetKeys.DeathDefault))
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
							AnimationSNO = AnimationSet.TagMapAnimDefault[AnimationSetKeys.DeathDefault],
							PermutationIndex = 0,
							AnimationTag = 0,
							Speed = 1
						}
					}

				}, this);

			Attributes[GameAttributes.Deleted_On_Server] = true;
			Attributes[GameAttributes.Could_Have_Ragdolled] = true;
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
