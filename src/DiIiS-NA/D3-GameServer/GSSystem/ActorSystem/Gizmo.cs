//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class Gizmo : Actor
	{
		public override ActorType ActorType { get { return ActorType.Gizmo; } }
		protected Logger Logger = new Logger("Gizmo");

		public Gizmo(World world, int snoId, TagMap tags, bool is_marker = false)
			: base(world, snoId, tags, is_marker)
		{
			this.Field2 = 0x9;//16;
			this.Field7 = 0x00000001;
			//this.CollFlags = 1; // this.CollFlags = 0; a hack for passing through blockers /fasbat
			if (this.Attributes[GameAttribute.TeamID] == 10) this.Attributes[GameAttribute.TeamID] = 1; //fix for bugged gizmos
			this.Attributes[GameAttribute.Hitpoints_Cur] = 1;
			//this.Attributes[GameAttribute.MinimapActive] = true;
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;
			return true;
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			if (this.Attributes[GameAttribute.Disabled] == true) return;
			Logger.Trace("(OnTargeted) Gizmo has been activated! Id: {0}, Type: {1}", this.ActorSNO.Id, this.ActorData.TagMap[ActorKeys.GizmoGroup]);
			
			//handling quest triggers
			if (this.World.Game.QuestProgress.QuestTriggers.ContainsKey(this.ActorSNO.Id))
			{
				var trigger = this.World.Game.QuestProgress.QuestTriggers[this.ActorSNO.Id];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor)
				{
					this.World.Game.QuestProgress.UpdateCounter(this.ActorSNO.Id);
					if (trigger.count == this.World.Game.QuestProgress.QuestTriggers[this.ActorSNO.Id].counter)
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
			else if (this.World.Game.SideQuestProgress.QuestTriggers.ContainsKey(this.ActorSNO.Id))
			{
				var trigger = this.World.Game.SideQuestProgress.QuestTriggers[this.ActorSNO.Id];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor)
				{
					this.World.Game.SideQuestProgress.UpdateSideCounter(this.ActorSNO.Id);
					if (trigger.count == this.World.Game.SideQuestProgress.QuestTriggers[this.ActorSNO.Id].counter)
						trigger.questEvent.Execute(this.World); // launch a questEvent
				}
			}
			if (this.World.Game.SideQuestProgress.GlobalQuestTriggers.ContainsKey(this.ActorSNO.Id))
			{
				var trigger = this.World.Game.SideQuestProgress.GlobalQuestTriggers[this.ActorSNO.Id];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor)
				{
					this.World.Game.SideQuestProgress.UpdateGlobalCounter(this.ActorSNO.Id);
					if (trigger.count == this.World.Game.SideQuestProgress.GlobalQuestTriggers[this.ActorSNO.Id].counter)
						try
						{
							trigger.questEvent.Execute(this.World); // launch a questEvent
							this.World.Game.SideQuestProgress.GlobalQuestTriggers.Remove(this.ActorSNO.Id);
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
				}
			}
		}

	}
}
