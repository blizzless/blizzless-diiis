//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
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
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(345935)]
	public class LootRunPortal : Portal
	{
		static readonly Logger Logger = LogManager.CreateLogger();
		private int MinimapIcon;

		public LootRunPortal(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Destination = new ResolvedPortalDestination
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

			this.Field2 = 0x9;//16;
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
			Logger.Debug("(OnTargeted) Portal has been activated, Id: {0}, LevelArea: {1}, World: {2}", this.ActorSNO.Id, this.Destination.DestLevelAreaSNO, this.Destination.WorldSNO);

			var world = this.World.Game.GetWorld((WorldSno)this.Destination.WorldSNO);

			if (world == null)
			{
				Logger.Warn("Portal's destination world does not exist (WorldSNO = {0})", this.Destination.WorldSNO);
				return;
			}

			var startingPoint = world.GetStartingPointById(this.Destination.StartingPointActorTag);

			if (startingPoint != null)
			{
				if (this.ActorSNO.Id == 230751) //a2 timed event
				{
					if (!this.World.Game.QuestManager.SideQuests[120396].Completed)
						player.ShowConfirmation(this.DynamicID(player), (() => {
							player.ChangeWorld(world, startingPoint);
						}));
				}
				else
				{
					if (world == this.World)
						player.Teleport(startingPoint.Position);
					else
						player.ChangeWorld(world, startingPoint);
				}

				if (this.World.Game.QuestProgress.QuestTriggers.ContainsKey(this.Destination.DestLevelAreaSNO)) //EnterLevelArea
				{
					var trigger = this.World.Game.QuestProgress.QuestTriggers[this.Destination.DestLevelAreaSNO];
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
					{
						try
						{
							trigger.questEvent.Execute(this.World); // launch a questEvent
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}
				if (this.World.Game.SideQuestProgress.QuestTriggers.ContainsKey(this.Destination.DestLevelAreaSNO)) //EnterLevelArea
				{
					var trigger = this.World.Game.SideQuestProgress.QuestTriggers[this.Destination.DestLevelAreaSNO];
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
					{
						try
						{
							trigger.questEvent.Execute(this.World); // launch a questEvent
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}
				if (this.World.Game.SideQuestProgress.GlobalQuestTriggers.ContainsKey(this.Destination.DestLevelAreaSNO)) //EnterLevelArea
				{
					var trigger = this.World.Game.SideQuestProgress.GlobalQuestTriggers[this.Destination.DestLevelAreaSNO];
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
					{
						try
						{
							trigger.questEvent.Execute(this.World); // launch a questEvent
							this.World.Game.SideQuestProgress.GlobalQuestTriggers.Remove(this.Destination.DestLevelAreaSNO);
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
					}
				}
				foreach (var bounty in this.World.Game.QuestManager.Bounties)
					bounty.CheckLevelArea(this.Destination.DestLevelAreaSNO);
			}
			else
				Logger.Warn("Portal's tagged starting point does not exist (Tag = {0})", this.Destination.StartingPointActorTag);
		}
	}
}
