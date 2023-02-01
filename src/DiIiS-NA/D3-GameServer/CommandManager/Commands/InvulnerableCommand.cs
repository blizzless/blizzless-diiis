using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("invulnerable", "Makes you invulnerable", Account.UserLevels.GM)]
public class InvulnerableCommand : CommandGroup
{
    [DefaultCommand]
    public string Invulnerable(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You cannot invoke this command from console.";

        if (player.Attributes.FixedMap.Contains(FixedAttribute.Invulnerable))
        {
            player.Attributes.FixedMap.Remove(FixedAttribute.Invulnerable);
            player.Attributes[GameAttribute.Invulnerable] = false;
            player.Attributes.BroadcastChangedIfRevealed();
            return "You are no longer invulnerable.";
        }

        player.Attributes.FixedMap.Add(FixedAttribute.Invulnerable,
            attributes => { attributes[GameAttribute.Invulnerable] = true; });
        player.Attributes.BroadcastChangedIfRevealed();
        return "You are now invulnerable.";
    }
}