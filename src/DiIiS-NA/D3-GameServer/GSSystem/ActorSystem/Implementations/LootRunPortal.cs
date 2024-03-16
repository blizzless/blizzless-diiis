using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(ActorSno._x1_openworld_lootrunportal)]
	public class LootRunPortal : Portal
	{
		static readonly Logger Logger = LogManager.CreateLogger();
		private int MinimapIcon;

		public LootRunPortal(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Destination = new ResolvedPortalDestination
			{
				WorldSNO = (int)WorldSno.x1_westm_graveyard_deathorb,
				DestLevelAreaSNO = 338946,
				StartingPointActorTag = 171
			};

			// Override minimap icon in markerset tags
			if (tags.ContainsKey(MarkerKeys.MinimapTexture))
			{
				MinimapIcon = tags[MarkerKeys.MinimapTexture].Id;
			}
			else
			{
				MinimapIcon = ActorData.TagMap[ActorKeys.MinimapMarker].Id;
			}

			Field2 = 0x9;//16;
		}

		public override bool Reveal(Player player)
		{
			return false;

			/*if (!base.Reveal(player))
				return false;
			
			player.InGameClient.SendMessage(new PortalSpecifierMessage()
			{
				ActorID = this.DynamicID(player),
				Destination = this.Destination
			});

			return true;*/
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			Logger.Debug("(OnTargeted) Portal has been activated, Id: {0}, LevelArea: {1}, World: {2}", (int)SNO, Destination.DestLevelAreaSNO, Destination.WorldSNO);

			var world = World.Game.GetWorld((WorldSno)Destination.WorldSNO);

			if (world == null)
			{
				Logger.Warn("Portal's destination world does not exist (WorldSNO = {0})", Destination.WorldSNO);
				return;
			}

			var startingPoint = world.GetStartingPointById(Destination.StartingPointActorTag);

			if (startingPoint != null)
			{
				if (SNO == ActorSno._a2dun_zolt_portal_timedevent) //a2 timed event
				{
					if (!World.Game.QuestManager.SideQuests[120396].Completed)
						player.ShowConfirmation(DynamicID(player), (() => {
							player.ChangeWorld(world, startingPoint);
						}));
				}
				else
				{
					if (world == World)
						player.Teleport(startingPoint.Position);
					else
						player.ChangeWorld(world, startingPoint);
				}

				if (World.Game.QuestProgress.QuestTriggers.ContainsKey(Destination.DestLevelAreaSNO)) //EnterLevelArea
				{
					var trigger = World.Game.QuestProgress.QuestTriggers[Destination.DestLevelAreaSNO];
					if (trigger.TriggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
					{
						try
						{
							trigger.QuestEvent.Execute(World); // launch a questEvent
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}
				if (World.Game.SideQuestProgress.QuestTriggers.ContainsKey(Destination.DestLevelAreaSNO)) //EnterLevelArea
				{
					var trigger = World.Game.SideQuestProgress.QuestTriggers[Destination.DestLevelAreaSNO];
					if (trigger.TriggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
					{
						try
						{
							trigger.QuestEvent.Execute(World); // launch a questEvent
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}
				if (World.Game.SideQuestProgress.GlobalQuestTriggers.ContainsKey(Destination.DestLevelAreaSNO)) //EnterLevelArea
				{
					var trigger = World.Game.SideQuestProgress.GlobalQuestTriggers[Destination.DestLevelAreaSNO];
					if (trigger.TriggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
					{
						try
						{
							trigger.QuestEvent.Execute(World); // launch a questEvent
							World.Game.SideQuestProgress.GlobalQuestTriggers.Remove(Destination.DestLevelAreaSNO);
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}
				foreach (var bounty in World.Game.QuestManager.Bounties)
					bounty.CheckLevelArea(Destination.DestLevelAreaSNO);
			}
			else
				Logger.Warn("Portal's tagged starting point does not exist (Tag = {0})", Destination.StartingPointActorTag);
		}
	}
}
