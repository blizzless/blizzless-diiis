//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents
{
	class Babah : QuestEvent
	{
		public Babah()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			world.GetActorBySNO(295438).PlayActionAnimation(334748);
			TickTimer Timeout = new SecondsTickTimer(world.Game, 5.5f);
			var Boom = System.Threading.Tasks.Task<bool>.Factory.StartNew(() => WaitToSpawn(Timeout));
			Boom.ContinueWith(delegate
			{
				world.Game.QuestManager.NotifyQuest(1, true);
				world.Game.QuestManager.Advance();
			});
			
		}

		private bool WaitToSpawn(TickTimer timer)
		{
			while (timer.TimedOut != true)
			{

			}
			return true;
		}
	}
}
