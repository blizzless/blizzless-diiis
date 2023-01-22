//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Encounter;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class AskBossEncounter : QuestEvent
	{
		int Encounter = -1;

		public AskBossEncounter(int encSNOid)
			: base(0)
		{
			Encounter = encSNOid;
		}

		public override void Execute(MapSystem.World world)
		{
			world.Game.TeleportToBossEncounter(Encounter);
		}
	}
}
