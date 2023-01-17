//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class SpawnSouls : QuestEvent
	{

		private static readonly Logger Logger = LogManager.CreateLogger();

		public SpawnSouls()
			: base(151125)
		{
		}

		List<Vector3D> ActorsVector3D = new List<Vector3D> { }; //We fill this with the vectors of the actors that transform, so we spwan zombies in the same location.
		List<uint> monstersAlive = new List<uint> { }; //We use this for the killeventlistener.

		public override void Execute(MapSystem.World world)
		{
			//if (world.Game.Empty) return;
			//Logger.Debug("SpawnSouls event started");
			var spot1 = world.GetActorBySNO(ActorSno._trdun_skeleton_d_3);
			while (spot1 != null)
			{
				ActorsVector3D.Add(spot1.Position);
				spot1.Destroy();
				spot1 = world.GetActorBySNO(ActorSno._trdun_skeleton_d_3);
			}
			var spot2 = world.GetActorBySNO(ActorSno._trdun_skeleton_b_2);
			while (spot2 != null)
			{
				ActorsVector3D.Add(spot2.Position);
				spot2.Destroy();
				spot2 = world.GetActorBySNO(ActorSno._trdun_skeleton_b_2);
			}
			var spot3 = world.GetActorBySNO(ActorSno._trdun_skeleton_c_4);
			while (spot3 != null)
			{
				ActorsVector3D.Add(spot3.Position);
				spot3.Destroy();
				spot3 = world.GetActorBySNO(ActorSno._trdun_skeleton_c_4);
			}

			for (int i = 0; i < 6; i++)
			{
				var rand_pos = ActorsVector3D[FastRandom.Instance.Next(ActorsVector3D.Count())];
				world.SpawnMonster(ActorSno._ghost_jail_prisoner, rand_pos);
				ActorsVector3D.Remove(rand_pos);
			}
		}

	}
}
