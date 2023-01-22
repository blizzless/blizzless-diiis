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
	[HandledSNO(ActorSno._tentaclelord)]
	public class CowKing : InteractiveNPC
	{
		private bool Available = false;

		public CowKing(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			CollFlags = 0;
			WalkSpeed = 0;
			Attributes[GameAttribute.Invulnerable] = true;
		}

		public override bool Reveal(Player player)
		{
			switch (World.Game.Difficulty)
			{
				case 0:
					Available = (player.Inventory.HasItem(-1077364974) || player.Inventory.HasItem(670723600) || player.Inventory.HasItem(-306066026) || player.Inventory.HasItem(1952916002));
					break;
				case 1:
					Available = (player.Inventory.HasItem(670723600) || player.Inventory.HasItem(-306066026) || player.Inventory.HasItem(1952916002));
					break;
				case 2:
					Available = (player.Inventory.HasItem(-306066026) || player.Inventory.HasItem(1952916002));
					break;
				case 3:
					Available = player.Inventory.HasItem(1952916002);
					break;
				default:
					Available = (player.Inventory.HasItem(-1077364974) || player.Inventory.HasItem(670723600) || player.Inventory.HasItem(-306066026) || player.Inventory.HasItem(1952916002));
					break;
			}

			if (Available)
				NotifyConversation(2);
			else
				return false;

			if (!base.Reveal(player))
				return false;

			return true;
		}


		public override void OnTargeted(Player player, TargetMessage message)
		{
			if (!Available) return;

			base.OnTargeted(player, message);
			foreach (var plr in World.Players)
			{
				plr.Value.Conversations.StartConversation(208400);
			}
		}
	}
}
