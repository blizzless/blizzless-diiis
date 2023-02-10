using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("resourceful", "Makes your character with full resource. Useful for testing.",
    Account.UserLevels.Tester)]
public class ResourcefulCommand : CommandGroup
{
    [DefaultCommand]
    public string Resourceful(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You must be in game to use this command.";

        if (player.Attributes.FixedMap.Contains(FixedAttribute.Resourceful))
        {
            player.Attributes.FixedMap.Remove(FixedAttribute.Resourceful);
            player.Attributes.BroadcastChangedIfRevealed();
            return "You are no longer Resourceful.";
        }

        player.Attributes.FixedMap.Add(FixedAttribute.Resourceful, (attributes) =>
        {
            attributes[GameAttributes.Resource_Cur, 1] = 100;
        });

        player.Attributes.BroadcastChangedIfRevealed();
        return "You are now resourceful.";
    }
}