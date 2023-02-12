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
			int levelArea = scene.Specification.SNOLevelAreas[0];
			((HearthPortal)World.Game.GetHearthPortal()).ReturnWorld = World.SNO;
			((HearthPortal)World.Game.GetHearthPortal()).ReturnPosition = User.Position;

			Vector3D exCheckpoint = User.CheckPointPosition;

			((Player)User).InGameClient.SendMessage(new MessageSystem.Message.Definitions.Base.SimpleMessage(MessageSystem.Opcodes.LoadingWarping));
			if (world != User.World)
				User.ChangeWorld(world, World.Game.GetHearthPortal().Position);
			else
				User.Teleport(World.Game.GetHearthPortal().Position);

			User.CheckPointPosition = exCheckpoint;
			((HearthPortal)World.Game.GetHearthPortal()).Owner = (User as Player);
			World.Game.GetHearthPortal().SetVisible(true);

			((Player)User).InGameClient.SendMessage(new HearthPortalInfoMessage
			{
				snoLevelArea = levelArea,
				snoUnknown = -1,
				Field1 = -1,
			});

			var town_proximity = new RectangleF(User.Position.X - 1f, User.Position.Y - 1f, 2f, 2f);
			var town_scenes = User.World.QuadTree.Query<Scene>(town_proximity);
			var town_scene = town_scenes[0]; // Parent scene /fasbat

			if (town_scenes.Count == 2) // What if it's a subscene?
			{
				if (town_scenes[1].ParentChunkID != 0xFFFFFFFF)
					town_scene = town_scenes[1];
			}

			var town_levelArea = town_scene.Specification.SNOLevelAreas[0];
			if (User.World.Game.QuestProgress.QuestTriggers.ContainsKey(town_levelArea)) //EnterLevelArea
			{
				var trigger = User.World.Game.QuestProgress.QuestTriggers[town_levelArea];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.EnterLevelArea)
				{
					try
					{
						trigger.questEvent.Execute(User.World); // launch a questEvent
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
