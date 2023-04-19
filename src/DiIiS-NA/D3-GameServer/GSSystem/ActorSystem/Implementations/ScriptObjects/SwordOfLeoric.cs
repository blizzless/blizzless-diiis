using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(ActorSno._trdun_crypt_deathoftheking_sword_clickable)]
	public class SwordOfLeoric : Gizmo
	{
		public SwordOfLeoric(World world, ActorSno sno, TagMap tags) :
			base(world, sno, tags)
		{
			//163449 - Sword Leoric
			//220219 - Point to Spawn Ghost Leoric 
			//220218 - Point to Spawn Ghost Knight
			//4182 - Ghost Knight
			//4183 - Lachdanan's Ghost
			//5365 - King Leoric's Ghost
			//139823  Event_DoK_Kill.cnv
			//139825  Event_DoK_Death.cnv


		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			var GhostLeoricPoint = player.World.GetActorBySNO(ActorSno._spawner_leoric_dokevent).Position;
			var GhostKingtsSpawners = player.World.GetActorsBySNO(ActorSno._spawner_ghostknight_dokevent);
			//Спауним Дух Леорика
			List<Actor> GhostKnights = new List<Actor>() { };
			//Спауним Духов Рыцарей
			for (int i = 0; i < 4; i++)
				GhostKnights.Add(player.World.SpawnMonster(ActorSno._ghostknight2, GhostKingtsSpawners[i].Position));

			var LeoricGhost = player.World.SpawnMonster(ActorSno._skeletonking_leoricghost, GhostLeoricPoint);
			var LachdananGhost = player.World.SpawnMonster(ActorSno._ghostknight3, GhostKingtsSpawners[4].Position);

            LeoricGhost.Attributes[GameAttributes.TeamID] = 2;
            LeoricGhost.Attributes[GameAttributes.Untargetable] = true;
            LachdananGhost.Move(Position, MovementHelpers.GetFacingAngle(LeoricGhost, LachdananGhost));
			LachdananGhost.Move(Position, MovementHelpers.GetFacingAngle(LachdananGhost, Position));
			LachdananGhost.Attributes[GameAttributes.TeamID] = 2;
			LachdananGhost.Attributes[GameAttributes.Untargetable] = true;
			LachdananGhost.Attributes.BroadcastChangedIfRevealed();
			(LachdananGhost as Monster).Brain.DeActivate();
			foreach (var GKnight in GhostKnights)
			{
				GKnight.Attributes[GameAttributes.TeamID] = 2;
                GKnight.Attributes[GameAttributes.Untargetable] = true;
                GKnight.Attributes.BroadcastChangedIfRevealed();
				(GKnight as Monster).Brain.DeActivate();
				GKnight.Move(Position, MovementHelpers.GetFacingAngle(GKnight, Position));
			}
			//Запуск сцены
			StartConversation(player.World, 139823);

			World.BroadcastIfRevealed(plr => new PlayAnimationMessage
			{
				ActorID = DynamicID(plr),
				AnimReason = 5,
				UnitAniimStartTime = 0,
				tAnim = new PlayAnimationMessageSpec[]
				{
					new PlayAnimationMessageSpec()
					{
						Duration = 50,
						AnimationSNO = AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening],
						PermutationIndex = 0,
						AnimationTag = 0,
						Speed = 1
					}
				}

			}, this);
			World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
			{
				ActorID = DynamicID(plr),
				AnimationSNO = AnimationSetKeys.Open.ID
			}, this);

			bool status = false;
			Attributes[GameAttributes.Team_Override] = (status ? -1 : 2);
			Attributes[GameAttributes.Untargetable] = !status;
			Attributes[GameAttributes.NPC_Is_Operatable] = status;
			Attributes[GameAttributes.Operatable] = status;
			Attributes[GameAttributes.Operatable_Story_Gizmo] = status;
			Attributes[GameAttributes.Disabled] = !status;
			Attributes[GameAttributes.Immunity] = !status;
			Attributes.BroadcastChangedIfRevealed();
		}
		private bool StartConversation(World world, Int32 conversationId)
		{
			foreach (var player in world.Players)
				player.Value.Conversations.StartConversation(conversationId);
			return true;
		}
	}
}
