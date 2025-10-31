using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Portal;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
	[ImplementsPowerSNO(191590)]
	public class TownTeleport : ActionTimedSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			User.PlayEffectGroup(202824);
			yield return WaitSeconds(0.2f);
			var world = World.Game.StartingWorld;
			var proximity = new RectangleF(User.Position.X - 1f, User.Position.Y - 1f, 2f, 2f);
			var scenes = World.QuadTree.Query<Scene>(proximity);
			var scene = scenes[0]; // Parent scene /fasbat
			var levelArea = scene.Specification.SNOLevelAreas[0];
            if (World.Game.GetHearthPortal() is HearthPortal heartPortal)
            {
                heartPortal.ReturnWorld = World.SNO;
                heartPortal.ReturnPosition = User.Position;
            }

            Vector3D exCheckpoint = User.CheckPointPosition;

			if (User is Player plr1)
			    plr1.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Base.SimpleMessage(MessageSystem.Opcodes.LoadingWarping));
			if (world != User.World)
				User.ChangeWorld(world, World.Game.GetHearthPortal().Position);
			else
				User.Teleport(World.Game.GetHearthPortal().Position);

			User.CheckPointPosition = exCheckpoint;

            if (World.Game.GetHearthPortal() is HearthPortal heartPortal2)
            {
				if (User is Player plr2)
                    heartPortal2.Owner = plr2;
            }

            World.Game.GetHearthPortal().SetVisible(true);

            if (User is Player plr3)
            {
                plr3.InGameClient.SendMessage(new HearthPortalInfoMessage
                {
                    snoLevelArea = levelArea,
                    snoUnknown = -1,
                    Field1 = -1,
                });
            }

            var townProximity = new RectangleF(User.Position.X - 1f, User.Position.Y - 1f, 2f, 2f);
            var townScenes = User.World.QuadTree.Query<Scene>(townProximity);
            var townScene = townScenes[0]; // Parent scene /fasbat

            // If there are multiple scenes, find the most appropriate one
            if (townScenes.Count > 1)
            {
                // Look for the deepest subscene (one with a parent that is also in our query results)
                for (int i = 1; i < townScenes.Count; i++)
                {
                    if (townScenes[i].ParentChunkID != 0xFFFFFFFF)
                    {
                        // This is a subscene, use it
                        townScene = townScenes[i];
                        break;
                    }
                }
            }

            var townLevelArea = townScene.Specification.SNOLevelAreas[0];
			if (User.World.Game.QuestProgress.QuestTriggers.TryGetValue(townLevelArea, out var questTriggerLevelArea)) //EnterLevelArea
			{
				var trigger = User.World.Game.QuestProgress.QuestTriggers[townLevelArea];
				if (trigger.TriggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
				{
					try
					{
						trigger.QuestEvent.Execute(User.World); // launch a questEvent
					}
					catch (Exception e)
					{
						Logger.WarnException(e, "questEvent()");
					}
				}
			}

			yield break;
		}

		public override float GetActionSpeed()
		{
			// for some reason the formula for _Instant.pow does not multiply by 1.1 even though it should
			// manually scale melee speed
			return base.GetActionSpeed() * 10f;
		}
	}
}
