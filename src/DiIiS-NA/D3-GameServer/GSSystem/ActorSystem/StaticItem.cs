using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using System;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class StaticItem : Gizmo
	{
		public override ActorType ActorType { get { return ActorType.Item; } }

		public StaticItem(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			GBHandle.Type = (int)ActorType.Item;
			GBHandle.GBID = -1;//944034263;
			Attributes[GameAttributes.Operatable] = true;
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;
			return true;
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			Logger.Debug("(OnTargeted) StaticItem has been activated! Id: {0}, Type: {1}", SNO, ActorData.TagMap[ActorKeys.GizmoGroup]);
			//handling quest triggers
			if (World.Game.QuestProgress.QuestTriggers.ContainsKey((int)SNO))
			{
				var trigger = World.Game.QuestProgress.QuestTriggers[(int)SNO];
				if (trigger.TriggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.InteractWithActor)
				{
					World.Game.QuestProgress.UpdateCounter((int)SNO);
					if (trigger.Count == World.Game.QuestProgress.QuestTriggers[(int)SNO].Counter)
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
		}
	}
}
