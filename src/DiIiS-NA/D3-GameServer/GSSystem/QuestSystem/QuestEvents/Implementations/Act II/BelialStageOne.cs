//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class BelialStageOne : QuestEvent
	{

		public BelialStageOne()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			foreach (var actor in world.Actors.Values.Where(a => a is Monster || a is Player || a is Minion || a is Hireling))
			{
				actor.Attributes[GameAttribute.Stunned] = false;
				actor.Attributes.BroadcastChangedIfRevealed();
			}
			foreach (var plr in world.Players.Values)
			{
				plr.InGameClient.SendMessage(new BoolDataMessage(Opcodes.CameraTriggerFadeToBlackMessage) { Field0 = true });
				plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.CameraSriptedSequenceStopMessage) { });
			}

			world.GetActorBySNO(62975).Destroy();
			var Belial = world.SpawnMonster(62975, world.GetActorBySNO(59447).Position);
			
			Belial.Attributes[GameAttribute.Invulnerable] = true;
			Belial.Attributes.BroadcastChangedIfRevealed();
			(Belial as Monster).Brain.DeActivate();
			foreach (var Adr in world.GetActorsBySNO(3095))
				Adr.Destroy();
			foreach (var Adr in world.GetActorsBySNO(4580))
				Adr.Destroy();

			world.GetActorBySNO(59447).Destroy(); //hakan boy
			var guard = world.GetActorBySNO(81857, true);
			while (guard != null)
			{
				world.SpawnMonster(60816, guard.Position);
				guard.Destroy();
				guard = world.GetActorBySNO(81857, true);
			}
		}

	}
}
