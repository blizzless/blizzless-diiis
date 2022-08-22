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
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class CryptPortals : QuestEvent
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public CryptPortals()
			: base(0)
		{
		}

		/*private static readonly List<Vector3D> PortalPositions = new List<Vector3D> { 
			new Vector3D{ X = 2233.954f, Y = 1793.8f, Z = 6.619959f},
			new Vector3D{ X = 2176.305f, Y = 1939.683f, Z = -3.581532f},
			new Vector3D{ X = 2055.373f, Y = 1951.219f, Z = -3.670345f},
			new Vector3D{ X = 2032.983f, Y = 1776.411f, Z = 1.434785f}
		};*/

		private static readonly List<ResolvedPortalDestination> PortalDests = new List<ResolvedPortalDestination> {
			new ResolvedPortalDestination{ WorldSNO = (int)WorldSno.trdun_crypt_falsepassage_01, DestLevelAreaSNO = 145182, StartingPointActorTag = 172 },
			new ResolvedPortalDestination{ WorldSNO = (int)WorldSno.trdun_crypt_falsepassage_02, DestLevelAreaSNO = 145182, StartingPointActorTag = 172 },
			new ResolvedPortalDestination{ WorldSNO = (int)WorldSno.trdun_crypt_skeletonkingcrown_00, DestLevelAreaSNO = 145182, StartingPointActorTag = 172 }
		};

		public override void Execute(MapSystem.World world)
		{
			//Logger.Debug("CryptPortals event started");
			var portals = world.GetPortalsByLevelArea(154588);
			if (portals.Count == 0) return;
			foreach (var dest in PortalDests)
			{
				var random_portal = portals[FastRandom.Instance.Next(0, portals.Count - 1)];
				random_portal.Destination = dest;
				//portal.EnterWorld(random_spot);
				portals.Remove(random_portal);
			}
		}
	}
}
