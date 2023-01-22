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

		public static BattleClient GetClientByCID(ulong cid)
		{
			return OnlinePlayers.FirstOrDefault(bc => bc.CID == cid);
		}

		public static void SendWhisper(string message)
		{
			Broadcast(client =>
			{
				client.SendServerWhisper(message);
			});
		}
		
		public static void Broadcast(Action<BattleClient> action, Func<BattleClient, bool> predicate)
		{
			foreach (var client in OnlinePlayers.Where(predicate))
				action(client);
		}
		
		public static void Broadcast(Action<BattleClient> action)
		{
			foreach (var client in OnlinePlayers)
				action(client);
		}

		public static void PlayerDisconnected(BattleClient client)
		{
			if (OnlinePlayers.Contains(client))
				OnlinePlayers.Remove(client);
		}
	}
}
