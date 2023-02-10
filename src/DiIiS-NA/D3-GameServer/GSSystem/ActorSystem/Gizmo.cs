using System;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
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
			Field2 = 0x9;//16;
			Field7 = 0x00000001;
			//this.CollFlags = 1; // this.CollFlags = 0; a hack for passing through blockers /fasbat
			if (Attributes[GameAttributes.TeamID] == 10) Attributes[GameAttributes.TeamID] = 1; //fix for bugged gizmos
			Attributes[GameAttributes.Hitpoints_Cur] = 1;
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
			if (Attributes[GameAttributes.Disabled] == true) return;
			Logger.Trace("(OnTargeted) Gizmo has been activated! Id: {0}, Type: {1}", SNO, ActorData.TagMap[ActorKeys.GizmoGroup]);
			
			//handling quest triggers
			if (World.Game.QuestProgress.QuestTriggers.ContainsKey((int)SNO))
			{
				var trigger = World.Game.QuestProgress.QuestTriggers[(int)SNO];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor)
				{
					World.Game.QuestProgress.UpdateCounter((int)SNO);
					if (trigger.count == World.Game.QuestProgress.QuestTriggers[(int)SNO].counter)
						try
						{
							trigger.questEvent.Execute(World); // launch a questEvent
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "questEvent()");
						}
				}
			}
			else if (World.Game.SideQuestProgress.QuestTriggers.ContainsKey((int)SNO))
			{
				var trigger = World.Game.SideQuestProgress.QuestTriggers[(int)SNO];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor)
				{
					World.Game.SideQuestProgress.UpdateSideCounter((int)SNO);
					if (trigger.count == World.Game.SideQuestProgress.QuestTriggers[(int)SNO].counter)
						trigger.questEvent.Execute(World); // launch a questEvent
				}
			}
			if (World.Game.SideQuestProgress.GlobalQuestTriggers.ContainsKey((int)SNO))
			{
				var trigger = World.Game.SideQuestProgress.GlobalQuestTriggers[(int)SNO];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor)
				{
					World.Game.SideQuestProgress.UpdateGlobalCounter((int)SNO);
					if (trigger.count == World.Game.SideQuestProgress.GlobalQuestTriggers[(int)SNO].counter)
						try
						{
							trigger.questEvent.Execute(World); // launch a questEvent
							World.Game.SideQuestProgress.GlobalQuestTriggers.Remove((int)SNO);
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
