using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class Advance : QuestEvent
	{
		public Advance()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			world.Game.QuestManager.Advance();
		}
	}

    class AdvanceThenOpenPortal : QuestEvent
    {
        private readonly ActorSno[] _portalNames;

        public AdvanceThenOpenPortal(ActorSno[] portalNames)
            : base(0)
        {
            _portalNames = portalNames;
        }

        public override void Execute(MapSystem.World world)
        {
            foreach (var portalName in _portalNames)
            {
                foreach (var portal in world.GetPortalsBySNO(portalName))
                {
					portal.SetUsable(true);
                }
            }
            world.Game.QuestManager.Advance();
        }

    }

    class AdvanceWithNotify : QuestEvent
	{
		public AdvanceWithNotify()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			world.Game.QuestManager.NotifyQuest(1, true);
			world.Game.QuestManager.Advance();
		}
	}
}
