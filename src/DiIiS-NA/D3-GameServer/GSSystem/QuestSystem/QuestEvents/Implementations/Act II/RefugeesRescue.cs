//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class RefugeesRescue : QuestEvent
	{

		public RefugeesRescue()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			if (world.Game.Empty) return;
			if (!(world.Game.QuestProgress is ActII)) return;
			var plr = world.Game.Players.Values.First();
			var Quest = DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.Quest][121792];
            System.Threading.Tasks.Task.Run(() => 
			{
				while ((world.Game.QuestProgress as ActII).refugees < 8)//plr.HaveFollower(201583))
					if (plr.HaveFollower(201583))
					{

						plr.DestroyFollower(201583);
						(world.Game.QuestProgress as ActII).refugees++;

						foreach (var player in world.Game.Players.Values)
							player.InGameClient.SendMessage(new QuestCounterMessage()
							{
								snoQuest = 121792,
								snoLevelArea = -1,
								StepID = 21,
								TaskIndex = 2,
								Counter = (world.Game.QuestProgress as ActII).refugees,
								Checked = ((world.Game.QuestProgress as ActII).refugees >= 8) ? 1 : 0,
							});
					}
				bool Active = false;

				while (!Active)
					foreach (var act in plr.GetActorsInRange(10f))
						if (act.ActorSNO.Id == 217709)
							Active = true;
				
				foreach (var fol in world.GetActorsBySNO(201583))
					fol.Destroy();
				world.Game.QuestManager.Advance();
			}
			);
			
		}

	}
}
