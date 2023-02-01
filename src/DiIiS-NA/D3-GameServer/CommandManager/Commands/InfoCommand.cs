using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("info", "Get current game information.")]
public class InfoCommand : CommandGroup
{
    [DefaultCommand]
    public string Info(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Game is not { } game || invokerClient.InGameClient.Player is not { } player ||
            invokerClient.Account is not { } account)
            return "You are not in game.";
        return GetInfo(account, player, game);
    }

    private string GetInfo(Account account, Player player, Game game)
    {
        List<string> info = new()
        {
            $"Game: {game.GameId}",
            $"Difficulty: {game.Difficulty}",
            $"Worlds: {game.Worlds.Count}",
            $"Players: {game.Players.Count}",
            $"Monsters: {game.Worlds.Sum(w => w.Monsters.Count)}"
        };

        if (account.UserLevel >= Account.UserLevels.GM)
            foreach (var world in game.Worlds)
            {
                info.Add($"World: {world.SNO.ToString()} - {(int)world.SNO}");
                info.Add($"Players: {world.Players.Count}");
                info.Add($"Monsters: {world.Monsters.Count}");
                info.Add($"{world.Monsters.Count} players in world: ");
                foreach (var playerInWorld in world.Players)
                {
                    info.Add($"> Player[{playerInWorld.Value.PlayerIndex}]");
                    info.Add($"> Id: {playerInWorld.Value.GlobalID}");
                    // info.Add($"Index: {playerInWorld.Value.PlayerIndex}");
                    info.Add($"> Name: {playerInWorld.Value.Name}");
                    info.Add($"> Class: {playerInWorld.Value.Toon.Class.ToString()}");
                    info.Add($"> Level: {playerInWorld.Value.Toon.Level}");
                    info.Add(
                        $"> Health: {playerInWorld.Value.Attributes[GameAttribute.Hitpoints_Cur]} / {playerInWorld.Value.Attributes[GameAttribute.Hitpoints_Max]}");
                    info.Add($"> Damage: {playerInWorld.Value.Attributes[GameAttribute.Damage_Min, 0]}");
                    info.Add("");
                }

                info.Add("");
            }

        return string.Join('\n', info);
    }
}