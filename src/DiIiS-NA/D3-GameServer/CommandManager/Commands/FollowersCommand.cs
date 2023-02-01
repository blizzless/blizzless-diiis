using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("followers", "Manage your followers.", Account.UserLevels.Tester)]
public class FollowersCommand : CommandGroup
{
    [Command("list", "List all followers.")]
    public string List(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You must be in game to use this command.";

        List<string> followers = new();
        foreach (var follower in player.Followers.OrderBy(s => s.Value))
            followers.Add($"[{follower.Key}] {follower.Value.ToString()}");

        return string.Join('\n', followers);
    }

    [Command("dismiss", "Dismisses all followers.")]
    public string DismissAllCommand(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient?.InGameClient?.Player is not { } player)
            return "You are not in game.";

        var followers = player.Followers.ToArray();
        // destroy followers
        foreach (var follower in followers) player.DestroyFollower(follower.Value);

        return $"Dismissed {followers.Length} followers.";
    }
}