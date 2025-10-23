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
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System.Threading.Tasks;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Hireling;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class JondarDeath : QuestEvent
	{
		//ActorID: 0x7A3100DD  
		//ZombieSkinny_A_LeahInn.acr (2050031837)
		//ActorSNOId: 0x00031971:ZombieSkinny_A_LeahInn.acr

		//private static readonly Logger Logger = LogManager.CreateLogger();

		public JondarDeath()
			: base(0)
		{
		}


		public override void Execute(MapSystem.World world)
		{
			world.Game.QuestManager.Advance();
			if (world.Game.Players.Count > 1)
				foreach (var plr in world.Game.Players.Values)
					plr.InGameClient.SendMessage(new HirelingNoSwapMessage()
					{
						NewClass = 1, //Призвать нельзя!
					});
			else
				foreach (var plr in world.Game.Players.Values)
					plr.InGameClient.SendMessage(new HirelingSwapMessage()
					{
						NewClass = 1, //Возможность призвать Храмовника
					});

		}

	}
}
