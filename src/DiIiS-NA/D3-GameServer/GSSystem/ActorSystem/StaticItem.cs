//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
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
//Blizzless Project 2022 
using System;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class StaticItem : Gizmo
	{
		public override ActorType ActorType { get { return ActorType.Item; } }

		public StaticItem(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.GBHandle.Type = (int)ActorType.Item; this.GBHandle.GBID = -1;//944034263;
			this.Attributes[GameAttribute.Operatable] = true;
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;
			return true;
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			Logger.Debug("(OnTargeted) StaticItem has been activated! Id: {0}, Type: {1}", this.ActorSNO.Id, this.ActorData.TagMap[ActorKeys.GizmoGroup]);
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
		}
	}
}
