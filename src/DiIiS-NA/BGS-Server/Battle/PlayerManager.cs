using DiIiS_NA.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.LoginServer.Battle
{
	public static class PlayerManager
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public static readonly List<BattleClient> OnlinePlayers = new();

		public static void PlayerConnected(BattleClient client)
		{
			Logger.Info("Player connected: {0} (cid {1})", client.Account.Email, client.Cid);
			var alreadyLoggedIn = OnlinePlayers.Where(cli => cli.Account.Email == client.Account.Email).ToArray();
			foreach (var logged in alreadyLoggedIn)
			{
				Logger.Warn("Duplicate login for {0} — kicking previous session (cid {1})", client.Account.Email, logged.Cid);
				OnlinePlayers.Remove(client);
				logged.SocketConnection.DisconnectAsync();
			}

			OnlinePlayers.Add(client);
		}

		public static BattleClient GetClientByEmail(string email) => OnlinePlayers.FirstOrDefault(cli => cli.Account.Email == email);
		public static BattleClient GetClientByBattleTag(string battleTag) => OnlinePlayers.FirstOrDefault(cli => cli.Account.BattleTag == battleTag);

		public static BattleClient GetClientByCid(ulong cid)
		{
			return OnlinePlayers.FirstOrDefault(bc => bc.Cid == cid);
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
			{
				OnlinePlayers.Remove(client);
				Logger.Info("Player disconnected: {0}, online count {1}", client.Account?.Email, OnlinePlayers.Count);
			}
		}
	}
}
