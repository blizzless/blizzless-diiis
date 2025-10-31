using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;
using DiIiS_NA.LoginServer.Toons;
using DiIiS_NA.Utilities;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("info", "Get current game information.", inGameOnly: true)]
public class InfoCommand : CommandGroup
{
    private readonly Logger _logger = LogManager.CreateLogger();

    [DefaultCommand(minUserLevel: Account.UserLevels.Tester, inGameOnly: true)]
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
                if (world == null) continue;
                
                var worldName = "Unknown";
                try
                {
                    worldName = world?.SNO.GetName() ?? "__NONE";
                }
                catch { }
                
                info.Add($"World: {worldName} - {(int)world.SNO}");
                info.Add($"World Id: {world.GlobalID:N}");
                info.Add($"Players: {world.Players?.Count ?? 0}");
                info.Add($"Monsters: {world.Monsters?.Count ?? 0}");
                info.Add($"{world.Monsters?.Count ?? 0} players in world: ");
                
                if (world.Players != null && world.Players.Any())
                {
                    foreach (var playerInWorld in world.Players.Where(s=>s.Value != null))
                    {
                        if (playerInWorld.Value == null) continue;
                        
                        info.Add($"> Player[{playerInWorld.Value.PlayerIndex}]");
                        info.Add($"> Id: {playerInWorld.Value.GlobalID:N}");
                        // info.Add($"Index: {playerInWorld.Value.PlayerIndex}");
                        info.Add($"> Name: {playerInWorld.Value.Name ?? "Unknown"}");
                        info.Add($"> Class: {playerInWorld.Value.Toon?.Class.GetToonClassName() ?? "Unknown"}");
                        info.Add($"> Level: {playerInWorld.Value.Toon?.Level ?? 0}");
                        
                        if (playerInWorld.Value.Attributes != null)
                        {
                            info.Add(
                                $"> Health: {playerInWorld.Value.Attributes[GameAttributes.Hitpoints_Cur]} / {playerInWorld.Value.Attributes[GameAttributes.Hitpoints_Max]}");
                            info.Add($"> Damage: {playerInWorld.Value.Attributes[GameAttributes.Damage_Min, 0]}");
                        }
                        else
                        {
                            info.Add("> Health: Unknown");
                            info.Add("> Damage: Unknown");
                        }
                    }

                    if (world?.Game?.GetActQuest(true) is { } actQuest)
                        info.Add(actQuest);
                }
                else
                {
                    return "No players in world.";
                }
            }

        return string.Join('\n', info);
    }
}