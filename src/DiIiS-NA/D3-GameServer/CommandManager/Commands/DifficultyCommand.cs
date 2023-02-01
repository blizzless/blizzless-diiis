using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("difficulty", "Changes difficulty of the game", Account.UserLevels.GM)]
public class DifficultyCommand : CommandGroup
{
    [Command("up", "Increases difficulty of the game", Account.UserLevels.GM)]
    public string Up(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient is null)
            return "You must execute this command in-game.";
        if (invokerClient.InGameClient.Player.World.Game.Difficulty == 19)
            return "You can't increase difficulty any more.";
        invokerClient.InGameClient.Player.World.Game.RaiseDifficulty(invokerClient.InGameClient, null);
        return $"Difficulty increased - set to {invokerClient.InGameClient.Player.World.Game.Difficulty}";
    }

    [Command("down", "Decreases difficulty of the game", Account.UserLevels.GM)]
    public string Down(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient is null)
            return "You must execute this command in-game.";
        if (invokerClient.InGameClient.Player.World.Game.Difficulty == 0)
            return "Difficulty is already at minimum";
        invokerClient.InGameClient.Player.World.Game.LowDifficulty(invokerClient.InGameClient, null);
        return $"Difficulty decreased - set to {invokerClient.InGameClient.Player.World.Game.Difficulty}";
    }

    [Command("set", "Sets difficulty of the game", Account.UserLevels.GM)]
    public string Set(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient is null)
            return "You must execute this command in-game.";
        if (!int.TryParse(@params[0], out var difficulty) || difficulty is < 0 or > 19)
            return "Invalid difficulty. Must be between 0 and 19.";
        invokerClient.InGameClient.Player.World.Game.SetDifficulty(difficulty);
        return $"Difficulty set to {invokerClient.InGameClient.Player.World.Game.Difficulty}";
    }

    [DefaultCommand]
    public string Get(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient is null)
            return "You must execute this command in-game.";
        return $"Current difficulty is {invokerClient.InGameClient.Player.World.Game.Difficulty}";
    }
}