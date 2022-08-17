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
//Blizzless Project 2022 
using System.Threading.Tasks;
//Blizzless Project 2022 
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
