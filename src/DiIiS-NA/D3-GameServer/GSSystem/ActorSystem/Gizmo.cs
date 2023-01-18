//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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

		public Gizmo(World world, ActorSno sno, TagMap tags, bool is_marker = false)
			: base(world, sno, tags, is_marker)
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
			Logger.Trace("(OnTargeted) Gizmo has been activated! Id: {0}, Type: {1}", this.SNO, this.ActorData.TagMap[ActorKeys.GizmoGroup]);
			
			//handling quest triggers
			if (this.World.Game.QuestProgress.QuestTriggers.ContainsKey((int)this.SNO))
			{
				var trigger = this.World.Game.QuestProgress.QuestTriggers[(int)this.SNO];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor)
				{
					this.World.Game.QuestProgress.UpdateCounter((int)this.SNO);
					if (trigger.count == this.World.Game.QuestProgress.QuestTriggers[(int)this.SNO].counter)
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
			else if (this.World.Game.SideQuestProgress.QuestTriggers.ContainsKey((int)this.SNO))
			{
				var trigger = this.World.Game.SideQuestProgress.QuestTriggers[(int)this.SNO];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor)
				{
					this.World.Game.SideQuestProgress.UpdateSideCounter((int)this.SNO);
					if (trigger.count == this.World.Game.SideQuestProgress.QuestTriggers[(int)this.SNO].counter)
						trigger.questEvent.Execute(this.World); // launch a questEvent
				}
			}
			if (this.World.Game.SideQuestProgress.GlobalQuestTriggers.ContainsKey((int)this.SNO))
			{
				var trigger = this.World.Game.SideQuestProgress.GlobalQuestTriggers[(int)this.SNO];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor)
				{
					this.World.Game.SideQuestProgress.UpdateGlobalCounter((int)this.SNO);
					if (trigger.count == this.World.Game.SideQuestProgress.GlobalQuestTriggers[(int)this.SNO].counter)
						try
						{
							trigger.questEvent.Execute(this.World); // launch a questEvent
							this.World.Game.SideQuestProgress.GlobalQuestTriggers.Remove((int)this.SNO);
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
