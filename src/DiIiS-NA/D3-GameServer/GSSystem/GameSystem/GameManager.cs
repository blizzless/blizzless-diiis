using DiIiS_NA.Core.Extensions;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.ClientSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Util;

namespace DiIiS_NA.GameServer.GSSystem.GameSystem
{
	public static class GameManager
	{
		private static readonly Logger Logger = LogManager.CreateLogger();
		public static readonly Dictionary<int, Game> Games = new Dictionary<int, Game>();

		public static Game CreateGame(int gameId, int initialLevel)
		{
			if (Games.TryGetValue(gameId, out var createdGame))
				return createdGame;

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
			return !Games.ContainsValue(game) ? -1 : Games.Keys.First(g => Games[g] == game);
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
                        player.SendMessage(
                            new PlayerIndexMessage(Opcodes.PlayerLeaveGameMessage) //PlayerLeaveGameMessage
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
                        Logger.Error("Can't remove player ({0}) from game with id: {1}", gameClient.Player.Toon.Name,
                            gameId);
                        return;
                    }

                    if (p != null)
                    {

                        //TODO: Move this inside player OnLeave event
                        var toon = p.Toon;
                        toon.TimePlayed += (int)(DateTime.UtcNow.ToUnixTime() - toon.LoginTime);
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
                            gameClient.BnetClient.Account.GameAccount.ScreenStatus = D3.PartyMessage.ScreenStatus
                                .CreateBuilder().SetScreen(1).SetStatus(0).Build();
                            gameClient.BnetClient.Account.GameAccount.NotifyUpdate();

                        }
                        else
                        {
                            try
                            {
                                ClientSystem.GameServer.GSBackend.UpdateClient(toon.GameAccount.PersistentID,
                                    toon.Level, 1);
                            }
                            catch
                            {
                                Logger.Warn("Exception on RemovePlayerFromGame()");
                            }
                        }
                    }
                }
                else
                {
                    Logger.Error("RemovePlayerFromGame() gameClient.Game is null!");
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException(ex, nameof(RemovePlayerFromGame));
            }
		}
	}
}
