using System;
using System.Linq;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;
using DiIiS_NA.Utilities;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("recharge", "Resurrection Charges\nUsage: recharge 5", Account.UserLevels.GM)]
public class RechargeCommand : CommandGroup
{
    private const int MinResurrectionCharges = 1;
    private const int MaxResurrectionCharges = 99;

    [DefaultCommand(Account.UserLevels.GM, inGameOnly: true)]
    public string Charges(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player == null)
            return "You can only invoke this command while in-game.";

        var player = invokerClient.InGameClient.Player;

        if (@params.Length != 1 || !int.TryParse(@params[0], out var charges))
        {
            player.Attributes[GameAttributes.Corpse_Resurrection_Charges] = GameServerConfig.Instance.ResurrectionCharges;
            player.Attributes.BroadcastIfRevealed();
            return $"Resurrection charges reset to {GameServerConfig.Instance.ResurrectionCharges}.\nUsage: !recharge <positive amount>";
        }

        if (charges is < MinResurrectionCharges or > MaxResurrectionCharges)
            return $"Resurrection charges must be between {MinResurrectionCharges} and {MaxResurrectionCharges}, you specified {charges}.";

        player.Attributes[GameAttributes.Corpse_Resurrection_Charges] = charges;
        player.Attributes.BroadcastIfRevealed();
        return $"Successfully set your resurrection charges to {charges}.";
    }

    [Command("to", "Sets someone's resurrection charges: recharge to John 5", Account.UserLevels.GM, inGameOnly: true)]
    public string ChargesTo(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player == null)
            return "You can only invoke this command while in-game.";
        if (@params.Length != 2)
            return
                $"Usage: !recharge to <player name> <amount>.\nUse !onlines to get the online players.\nAmount must be ranged from {MinResurrectionCharges} to {MaxResurrectionCharges}";
        if (string.IsNullOrWhiteSpace(@params[0]))
            return "You must specify a player name.";
        if (!int.TryParse(@params[1], out var charges))
            return $"You must specify a valid amount of resurrection charges from {MinResurrectionCharges} to {MaxResurrectionCharges}.";
        if (charges is < MinResurrectionCharges or > MaxResurrectionCharges)
            return $"Resurrection charges must be between {MinResurrectionCharges} and {MaxResurrectionCharges}, and you set as {charges}.";

        // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
        // It can be null
        var player = invokerClient.InGameClient.Game.Players.GetPlayerByName(@params[0]);
        if (player == null)
        {
            StrBuilder sb = new StrBuilder();
            sb.Append("No players with this name specified.");
            sb.Append("Current players are:");
            foreach (var players in invokerClient.InGameClient.Game.Players.GetPlayersInGame())
            {
                sb.Append($" - {players.Value.Name}");
            }
            return sb.ToString(Separator.NewLine);
        }

        player.Attributes[GameAttributes.Corpse_Resurrection_Charges] = charges;
        player.Attributes.BroadcastIfRevealed();

        return $"Successfully set {player.Name}'s resurrection charges to {charges}.";
    }
}