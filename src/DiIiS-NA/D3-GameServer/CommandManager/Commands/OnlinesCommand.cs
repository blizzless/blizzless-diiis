using System;
using System.Linq;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;
using DiIiS_NA.Utilities;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("onlines", "Get all online players' names\nUsage: onlines", Account.UserLevels.User)]
public class OnlinesCommand : CommandGroup
{
    private readonly Logger _logger = LogManager.CreateLogger<OnlinesCommand>();

    [DefaultCommand(Account.UserLevels.User, inGameOnly: true)]
    public string Charges(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient == null)
            return "You can only invoke this command while in-game.";

        try
        {
            var players = invokerClient.InGameClient.Game.Players.GetPlayersInGame();

            StrBuilder sb = new StrBuilder();
            sb.Append($"Players Online ({players.Count}):");

            foreach (var player in players)
            {
                sb.Append($" - {player.Value.Name}");
            }

            return sb.ToString(Separator.NewLine);
        }
        catch (Exception ex)
        {
            _logger.ErrorException(ex, "Onlines Command Error");
            return "Impossible to get online players.";
        }
    }
}