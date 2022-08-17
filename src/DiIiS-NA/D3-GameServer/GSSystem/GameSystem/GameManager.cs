//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.ClientSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player;
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

namespace DiIiS_NA.GameServer.GSSystem.GameSystem
{
	public static class GameManager
	{
		private static readonly Logger Logger = LogManager.CreateLogger();
		public static readonly Dictionary<int, Game> Games = new Dictionary<int, Game>();

		public static Game CreateGame(int gameId, int initialLevel)
		{
			if (Games.ContainsKey(gameId))
				return Games[gameId];

			var game = new Game(gameId, initialLevel);
			Games.Add(gameId, game);
			return game;
		}

		public static Game GetGameById(int gameId)
		{
			return !Games.ContainsKey(gameId) ? null : Games[gameId];
		}

		public static int GetIdByGame(Game game)
		{
			return !Games.ContainsValue(game) ? -1 : Games.Keys.Where(g => Games[g] == game).First();
		}

		public static void RemovePlayerFromGame(GameClient gameClient)
		{
			try
			{
				if (gameClient == null)
				{
					Logger.Error("RemovePlayerFromGame() gameClient is null!");
					return;
				}

				foreach (var player in gameClient.Game.Players.Keys)
				{
					if (player == gameClient)
						player.SendMessage(new QuitGameMessage()
						{
							PlayerIndex = gameClient.Player.PlayerIndex,
						});
					else
						player.SendMessage(new PlayerIndexMessage(Opcodes.PlayerLeaveGameMessage) //PlayerLeaveGameMessage
						{
							PlayerIndex = gameClient.Player.PlayerIndex,
						});
				}

				if (gameClient.Game != null)
				{
					var gameId = gameClient.Game.GameId;
					if (!Games.ContainsKey(gameId)) return;

					var game = Games[gameId];
					if (!game.Players.ContainsKey(gameClient)) return;

					Player p = null;
					if (!game.Players.TryRemove(gameClient, out p))
					{
						Logger.Error("Can't remove player ({0}) from game with id: {1}", gameClient.Player.Toon.Name, gameId);
						return;
					}

					if (p != null)
					{

						//TODO: Move this inside player OnLeave event
						var toon = p.Toon;
						toon.TimePlayed += (int)(DateTimeExtensions.ToUnixTime(DateTime.UtcNow) - toon.LoginTime);
						toon.ExperienceNext = p.ExperienceNext;

						ClientSystem.GameServer.GSBackend.PlayerLeft(gameId);

						if (p.InGameClient != null)
						{
							var minions = p.Followers.Keys.ToList();
							foreach (var minion in minions)
								p.DestroyFollowerById(minion);
							p.World.Leave(p);
						}

						if (gameClient.BnetClient != null)
						{
							gameClient.BnetClient.Account.GameAccount.ScreenStatus = D3.PartyMessage.ScreenStatus.CreateBuilder().SetScreen(1).SetStatus(0).Build();
							gameClient.BnetClient.Account.GameAccount.NotifyUpdate();
							
						}
						else
						{
							try { ClientSystem.GameServer.GSBackend.UpdateClient(toon.GameAccount.PersistentID, toon.Level, 1); } catch { Logger.Warn("Exception on RemovePlayerFromGame()"); }
						}
					}
				}
				else
				{
					Logger.Error("RemovePlayerFromGame() gameClient.Game is null!");
				}
			}
			catch { }
		}
	}
}
