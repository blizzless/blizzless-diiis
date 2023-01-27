using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class StartSceneinHome : QuestEvent
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public StartSceneinHome()
			: base(0)
		{

		}

		public override void Execute(MapSystem.World world)
		{
			//{[Actor] [Type: ServerProp] SNOId:176900 GlobalId: 1013202487 Position: x:107.497185 y:138.07204 z:7.088848 Name: camera_cainsHouse_leahCloseUp}
			//{[Actor] [Type: ServerProp] SNOId:175759 GlobalId: 1013202485 Position: x:122.59496 y:131.74234 z:-0.6 Name: emitter_camera}
			var encWorld = world.Game.GetWorld(WorldSno.trout_tristram_cainshouse_event);

			var Maghda = encWorld.GetActorBySNO(ActorSno._maghda_nolaugh);
			Maghda.Attributes[GameAttribute.Hitpoints_Max] = 9000000f;
			Maghda.Attributes[GameAttribute.Hitpoints_Cur] = Maghda.Attributes[GameAttribute.Hitpoints_Max_Total];

			var Cultists = encWorld.GetActorsBySNO(ActorSno._triunesummoner_a_cainevent);
			foreach (var Cult in Cultists)
				(Cult as ActorSystem.Monster).Brain.DeActivate();

			foreach (var plr in world.Game.Players.Values)
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraCriptedSequenceStartMessage() { Activate = true });
			foreach (var plr in world.Game.Players.Values)
				plr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Camera.CameraFocusMessage() { ActorID = (int)encWorld.GetActorBySNO(ActorSno._emitter_camera).DynamicID(plr), Duration = 1f, Snap = false });
			StartConversation(world, 165125);
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

