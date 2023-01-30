using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System.Linq;
using System;
using System.Collections.Generic;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
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
				var randomPortal = portals.PickRandom();
				randomPortal.Destination = dest;
				//portal.EnterWorld(random_spot);
				portals.Remove(randomPortal);
			}
		}
	}
}
