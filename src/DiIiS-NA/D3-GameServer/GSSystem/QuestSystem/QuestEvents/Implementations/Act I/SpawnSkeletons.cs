using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System.Linq;
using System;
using System.Collections.Generic;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
using System.Threading.Tasks;
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


			foreach (var spawner in world.GetActorsBySNO(ActorSno._trdun_rescuecainskelspawner))
			{
				//var monster = FastRandom.Instance.Next(10) % 2 == 0 ? ActorSno._skeletonking_shield_skeleton : ActorSno._skeletonking_skeleton;
				var monster = ActorSno._skeletonking_shield_skeleton; // quest wants 4 killed _skeletonking_shield_skeleton
                world.SpawnMonster(monster, spawner.Position);
                spawner.Destroy();
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
