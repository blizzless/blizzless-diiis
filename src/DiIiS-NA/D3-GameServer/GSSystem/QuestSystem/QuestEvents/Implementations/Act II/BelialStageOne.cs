using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
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
				actor.Attributes[GameAttributes.Stunned] = false;
				actor.Attributes.BroadcastChangedIfRevealed();
			}
			foreach (var plr in world.Players.Values)
			{
				plr.InGameClient.SendMessage(new BoolDataMessage(Opcodes.CameraTriggerFadeToBlackMessage) { Field0 = true });
				plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.CameraSriptedSequenceStopMessage) { });
			}

			world.GetActorBySNO(ActorSno._belial_trueform).Destroy();
			var Belial = world.SpawnMonster(ActorSno._belial_trueform, world.GetActorBySNO(ActorSno._belialboyemperor).Position);
			
			Belial.Attributes[GameAttributes.Invulnerable] = true;
			Belial.Attributes.BroadcastChangedIfRevealed();
			(Belial as Monster).Brain.DeActivate();
			foreach (var Adr in world.GetActorsBySNO(ActorSno._adria))
				Adr.Destroy();
			foreach (var Adr in world.GetActorsBySNO(ActorSno._leah))
				Adr.Destroy();

			world.GetActorBySNO(ActorSno._belialboyemperor).Destroy(); //hakan boy
			var guard = world.GetActorBySNO(ActorSno._caldeumguard_spear_imperial, true);
			while (guard != null)
			{
				world.SpawnMonster(ActorSno._khamsin_snakeman_melee, guard.Position);
				guard.Destroy();
				guard = world.GetActorBySNO(ActorSno._caldeumguard_spear_imperial, true);
			}
		}

	}
}
