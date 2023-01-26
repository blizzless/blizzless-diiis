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
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using System.Threading.Tasks;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class SpawnSkeletons : QuestEvent
	{
		//ActorID: 0x7A3100DD  
		//ZombieSkinny_A_LeahInn.acr (2050031837)
		//ActorSNOId: 0x00031971:ZombieSkinny_A_LeahInn.acr

		private static readonly Logger Logger = LogManager.CreateLogger();

		public SpawnSkeletons()
			: base(151124)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			if (world.Game.Empty) return;
			//Logger.Debug("SpawnSkeletons event started");
			Task.Delay(1000).ContinueWith(delegate
			{
				foreach (var plr in world.Game.Players.Values)
					plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });
				var SkeletonKing_Bridge = world.GetActorBySNO(ActorSno._trdun_skeletonking_bridge_active);
				Task.Delay(1000).ContinueWith(delegate
				{
					foreach (var plr in world.Game.Players.Values)
						plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)SkeletonKing_Bridge.DynamicID(plr), Duration = 1f, Snap = false });
					
					StartConversation(world, 17923);

					SkeletonKing_Bridge.PlayAnimation(5, (AnimationSno)SkeletonKing_Bridge.AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening], 1f);

					world.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
					{
						ActorID = SkeletonKing_Bridge.DynamicID(plr),
						AnimationSNO = AnimationSetKeys.Open.ID,
					}, SkeletonKing_Bridge);

					Task.Delay(3000).ContinueWith(delegate
					{
						foreach (var plr in world.Game.Players.Values)
						{
							plr.InGameClient.SendMessage(new BoolDataMessage(Opcodes.CameraTriggerFadeToBlackMessage) { Field0 = true });
							plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.CameraSriptedSequenceStopMessage) { });
						}
					});
				});
			});
			
			

			var spawner = world.GetActorBySNO(ActorSno._trdun_rescuecainskelspawner);
			while (spawner != null)
			{
				var monster = FastRandom.Instance.Next(10) % 2 == 0 ? ActorSno._skeletonking_shield_skeleton : ActorSno._skeletonking_skeleton;
				world.SpawnMonster(monster, spawner.Position);
				spawner.Destroy();
				spawner = world.GetActorBySNO(ActorSno._trdun_rescuecainskelspawner);
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
