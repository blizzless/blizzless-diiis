using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Platinum;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("achiplatinum",
    "Platinum for your character.\nOptionally specify the number of levels: !platinum [count]", Account.UserLevels.GM, inGameOnly: true)]
public class PlatinumAchiCommand : CommandGroup
{
    [DefaultCommand(inGameOnly: true)]
    public string Platinum(string[] @params, BattleClient invokerClient)
    {
        var player = invokerClient.InGameClient.Player;
        var amount = 1;
        var achiid = 74987243307074;

        if (@params != null)
            if (!int.TryParse(@params[0], out amount))
                amount = 1;
        //if (!Int32.TryParse(@params[1], out amount))
        //     achiid = 74987243307074;
        player.InGameClient.SendMessage(new PlatinumAchievementAwardedMessage
        {
            CurrentPlatinum = 0,
            idAchievement = (ulong)achiid,
            PlatinumIncrement = amount
        });


        return "Achievement test";
    }
}