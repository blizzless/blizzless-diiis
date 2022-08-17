//Blizzless Project 2022
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

namespace DiIiS_NA.LoginServer.Battle
{
	public static class PlayerManager
	{
		public static readonly List<BattleClient> OnlinePlayers = new List<BattleClient>();

		public static void PlayerConnected(BattleClient client)
		{
			var already_logged = OnlinePlayers.Where(cli => cli.Account.Email == client.Account.Email);
			foreach (var logged in already_logged)
			{
				OnlinePlayers.Remove(client);
				logged.SocketConnection.DisconnectAsync();
			}

			OnlinePlayers.Add(client);
		}

		public static BattleClient GetClientbyCID(ulong cid)
		{
			foreach (var bc in OnlinePlayers)
				if (bc.CID == cid)
					return bc;
			return null;
		}

		public static void PlayerDisconnected(BattleClient client)
		{
			if (OnlinePlayers.Contains(client))
				OnlinePlayers.Remove(client);
		}
	}
}
