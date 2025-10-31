using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("difficulty", "Difficulty of the game", Account.UserLevels.GM, inGameOnly: true)]
public class DifficultyCommand : CommandGroup
{
    private const int MinDifficulty = 0;
    private const int MaxDifficulty = 19;

    [Command("max", "Sets difficulty to max", Account.UserLevels.GM, inGameOnly: true)]
    public string Max(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient is null)
            return "You must execute this command in-game.";
        if (invokerClient.InGameClient.Player.World.Game.Difficulty == MaxDifficulty)
            return "You can't increase difficulty any more.";
        invokerClient.InGameClient.Player.World.Game.SetDifficulty(MaxDifficulty);
        return $"Difficulty set to max - {invokerClient.InGameClient.Player.World.Game.Difficulty}";
    }
    
    [Command("min", "Sets difficulty to min", Account.UserLevels.GM, inGameOnly: true)]
    public string Min(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient is null)
            return "You must execute this command in-game.";
        if (invokerClient.InGameClient.Player.World.Game.Difficulty == MinDifficulty)
            return "You can't decrease difficulty any more.";
        invokerClient.InGameClient.Player.World.Game.SetDifficulty(MinDifficulty);
        return $"Difficulty set to min - {invokerClient.InGameClient.Player.World.Game.Difficulty}";
    }
    
    [Command("up", "Increases difficulty of the game", Account.UserLevels.GM, inGameOnly: true)]
    public string Up(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient is null)
            return "You must execute this command in-game.";
        if (invokerClient.InGameClient.Player.World.Game.Difficulty == MaxDifficulty)  
            return "You can't increase difficulty any more.";
        invokerClient.InGameClient.Player.World.Game.RaiseDifficulty(invokerClient.InGameClient, null);
        return $"Difficulty increased - set to {invokerClient.InGameClient.Player.World.Game.Difficulty}";
    }

    [Command("down", "Decreases difficulty of the game", Account.UserLevels.GM, inGameOnly: true)]
    public string Down(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient is null)
            return "You must execute this command in-game.";
        if (invokerClient.InGameClient.Player.World.Game.Difficulty == MinDifficulty)
            return "Difficulty is already at minimum";
        invokerClient.InGameClient.Player.World.Game.LowDifficulty(invokerClient.InGameClient, null);
        return $"Difficulty decreased - set to {invokerClient.InGameClient.Player.World.Game.Difficulty}";
    }

    [Command("set", "Sets the difficulty of the game", Account.UserLevels.GM, inGameOnly: true)]
    public string Set(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient is null)
            return "You must execute this command in-game.";
        if (!int.TryParse(@params[0], out var difficulty) || difficulty is < MinDifficulty or > MaxDifficulty)
            return "Invalid difficulty. Must be between 0 and 19.";
        invokerClient.InGameClient.Player.World.Game.SetDifficulty(difficulty);
        return $"Difficulty set to {invokerClient.InGameClient.Player.World.Game.Difficulty}";
    }

    [Command("get", "Gets the difficulty of the game", Account.UserLevels.User, inGameOnly: true)]
    public string Get(string[] @params, BattleClient invokerClient)
    {
        return invokerClient?.InGameClient is null ? 
            "You must execute this command in-game." : 
            $"Difficulty is set to {invokerClient.InGameClient.Player.World.Game.Difficulty}";
    }

    [DefaultCommand(inGameOnly: true)]
    public string Default(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient is null)
            return "You must execute this command in-game.";
        return $"Commands:\n" +
               $"Difficulties range from 0-19.\n\n" +
               $"Use !difficulty get - to get in-game difficulty.\n" +
               $"Use !difficulty set <value> - to set difficulty to a specific value.\n" +
               $"Use !difficulty up - to increase difficulty by 1.\n" +
               $"Use !difficulty down - to decrease difficulty by 1.\n" +
               $"Use !difficulty max - to set difficulty to max ({MaxDifficulty}).\n" +
               $"Use !difficulty min - to set difficulty to min ({MinDifficulty}).";
    }
}