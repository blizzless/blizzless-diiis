//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Drawing;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Portal;
//Blizzless Project 2022 
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
			(World.Game.GetHearthPortal() as HearthPortal).ReturnWorld = World.SNO;
			(World.Game.GetHearthPortal() as HearthPortal).ReturnPosition = User.Position;

			Vector3D exCheckpoint = User.CheckPointPosition;

			(User as Player).InGameClient.SendMessage(new MessageSystem.Message.Definitions.Base.SimpleMessage(MessageSystem.Opcodes.LoadingWarping));
			if (world != User.World)
				User.ChangeWorld(world, World.Game.GetHearthPortal().Position);
			else
				User.Teleport(World.Game.GetHearthPortal().Position);

			User.CheckPointPosition = exCheckpoint;
			(World.Game.GetHearthPortal() as HearthPortal).Owner = (User as Player);
			World.Game.GetHearthPortal().SetVisible(true);

			(User as Player).InGameClient.SendMessage(new HearthPortalInfoMessage
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
