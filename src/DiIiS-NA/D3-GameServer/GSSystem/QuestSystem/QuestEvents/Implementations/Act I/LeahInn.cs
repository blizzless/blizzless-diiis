using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class LeahInn : QuestEvent
	{
		//ActorID: 0x7A3100DD  
		//ZombieSkinny_A_LeahInn.acr (2050031837)
		//ActorSNOId: 0x00031971:ZombieSkinny_A_LeahInn.acr

		//private static readonly Logger Logger = LogManager.CreateLogger();

		public LeahInn()
			: base(151123) // 204113 // 151156
		{
		}

		List<Vector3D> ActorsVector3D = new List<Vector3D> { }; //We fill this with the vectors of the actors that transform, so we spwan zombies in the same location.
		List<uint> monstersAlive = new List<uint> { }; //We use this for the killeventlistener.

		public override void Execute(MapSystem.World world)
		{
			List<Actor> actorstotarget = new List<Actor> { };
			if (world.Game.Empty) return;
			StartConversation(world, 204113);

			var spawner = world.GetActorBySNO(ActorSno._omninpc_tristram_male_b_blacksmith);
			actorstotarget.Add(world.SpawnMonster(ActorSno._zombieskinny_a_leahinn, spawner.Position));
			spawner.Destroy();
			spawner = world.GetActorBySNO(ActorSno._omninpc_tristram_male_e_blacksmith);
			actorstotarget.Add(world.SpawnMonster(ActorSno._zombieskinny_a_leahinn, spawner.Position));
			spawner.Destroy();
			spawner = world.GetActorBySNO(ActorSno._omninpc_tristram_male_d_blacksmith);
			actorstotarget.Add(world.SpawnMonster(ActorSno._zombieskinny_a_leahinn, spawner.Position));
			spawner.Destroy();
			spawner = world.GetActorBySNO(ActorSno._omninpc_tristram_male_c_blacksmith);
			actorstotarget.Add(world.SpawnMonster(ActorSno._zombieskinny_a_leahinn, spawner.Position));
			spawner.Destroy();
			spawner = world.GetActorBySNO(ActorSno._omninpc_tristram_male_a_blacksmith);
			actorstotarget.Add(world.SpawnMonster(ActorSno._zombieskinny_a_leahinn, spawner.Position));
			spawner.Destroy();

			foreach (var actor in actorstotarget)
			{
				actor.PlayAnimation(9, AnimationSno.omninpc_male_hth_zombie_transition_intro_01, 1f);
				actor.Attributes[GameAttribute.Quest_Monster] = true;
				actor.Attributes.BroadcastChangedIfRevealed();
			}
		}

		private bool StartConversation(MapSystem.World world, Int32 conversationId)
		{
			foreach (var player in world.Players)
			{
				player.Value.Conversations.StartConversation(conversationId);
			}
			return true;
		}
	}
}
