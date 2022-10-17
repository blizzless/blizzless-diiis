//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(ActorSno._pvp_dueling_npc)]
	public class Brawler : InteractiveNPC
	{
		public Brawler(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.Invulnerable] = true;
		}

		public override bool Reveal(Player player)
		{
			return false; //disabled until fixed
			/*
			if (this.World.Game.CurrentAct != 3000 || this.World.WorldSNO.Id != 332336 || this.World.Game.IsHardcore || player.Level < 70 || this.World.Game.Players.Count > 1)
				return false;

			if (!base.Reveal(player))
				return false;

			this.NotifyConversation(2);

			return true;
			//*/
		}


		public override void OnTargeted(Player player, TargetMessage message)
		{
			base.OnTargeted(player, message);
			player.Conversations.StartConversation(275450);
		}
	}
}
