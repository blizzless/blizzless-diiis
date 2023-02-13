using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("invulnerable", "Makes you invulnerable", Account.UserLevels.GM, inGameOnly: true)]
public class InvulnerableCommand : CommandGroup
{
    [DefaultCommand]
    public string Invulnerable(string[] @params, BattleClient invokerClient)
    {
        var player = invokerClient.InGameClient.Player;

        if (player.Attributes.FixedMap.Contains(FixedAttribute.Invulnerable))
        {
            player.Attributes.FixedMap.Remove(FixedAttribute.Invulnerable);
            player.Attributes[GameAttributes.Invulnerable] = false;
            player.Attributes.BroadcastChangedIfRevealed();
            return "You are no longer invulnerable.";
        }

        player.Attributes.FixedMap.Add(FixedAttribute.Invulnerable,
            attributes => { attributes[GameAttributes.Invulnerable] = true; });
        player.Attributes.BroadcastChangedIfRevealed();
        return "You are now invulnerable.";
    }
}