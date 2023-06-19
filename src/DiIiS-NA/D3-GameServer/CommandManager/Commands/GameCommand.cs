using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("game", "Game Commands", Account.UserLevels.Admin, inGameOnly: false)]
public class GameCommand : CommandGroup
{
    private static readonly Logger Logger = LogManager.CreateLogger();
    [Command("reload-mods", "Reload all game mods file.", Account.UserLevels.Admin, inGameOnly: false)]
    public void ReloadMods(string[] @params, BattleClient invokerClient)
    {
        GameModsConfig.ReloadSettings();
        invokerClient.SendServerWhisper("Game mods updated successfully!");
    }
}