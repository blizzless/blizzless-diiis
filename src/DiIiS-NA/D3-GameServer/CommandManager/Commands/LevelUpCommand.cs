using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("levelup", "Levels your character.\nOptionally specify the number of levels: !levelup [count]",
    Account.UserLevels.GM)]
public class LevelUpCommand : CommandGroup
{
    [DefaultCommand]
    public string LevelUp(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient == null)
            return "You cannot invoke this command from console.";

        if (invokerClient.InGameClient == null)
            return "You can only invoke this command while in-game.";

        var player = invokerClient.InGameClient.Player;
        var amount = 1;

        if (@params != null)
            if (!int.TryParse(@params[0], out amount) || amount < 1)
                return "Invalid amount of levels.";

        for (var i = 0; i < amount; i++)
            if (player.Level >= 70)
            {
                player.UpdateExp((int)player.Attributes[GameAttributes.Alt_Experience_Next_Lo]);
                player.PlayEffect(Effect.ParagonLevelUp, null, false);
                player.World.PowerManager.RunPower(player, 252038);
            }
            else
            {
                player.UpdateExp((int)player.Attributes[GameAttributes.Experience_Next_Lo]);
                player.PlayEffect(Effect.LevelUp, null, false);
                player.World.PowerManager.RunPower(player, 85954);
            }

        player.Toon.GameAccount.NotifyUpdate();
        return player.Level >= 70 ? $"New paragon level: {player.ParagonLevel}" : $"New level: {player.Toon.Level}";
    }
}