//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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
			world.GetActorBySNO(ActorSno._x1_pand_batteringram_background).PlayActionAnimation(AnimationSno.x1_pand_batteringram_background_move_in_and_out_hit_03);
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
