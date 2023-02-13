using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Platinum;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("platinum",
    "Platinum for your character.\nOptionally specify the number of levels: !platinum [count]",
    Account.UserLevels.Tester, inGameOnly: true)]
public class PlatinumCommand : CommandGroup
{
    [DefaultCommand(inGameOnly: true)]
    public string Platinum(string[] @params, BattleClient invokerClient)
    {
        var player = invokerClient.InGameClient.Player;
        var amount = 1;

        if (@params != null)
            if (!int.TryParse(@params[0], out amount))
                amount = 1;


        player.InGameClient.SendMessage(new PlatinumAwardedMessage
        {
            CurrentPlatinum = player.InGameClient.BnetClient.Account.GameAccount.Platinum,
            PlatinumIncrement = amount
        });

        player.InGameClient.BnetClient.Account.GameAccount.Platinum += amount;
        player.Inventory.UpdateCurrencies();
        return "Platinum given.";
    }
}