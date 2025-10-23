using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.CommandManager;

[CommandGroup("spawn", "Spawns a mob.\nUsage: spawn [actorSNO] [amount]", Account.UserLevels.GM)]
public class SpawnCommand : CommandGroup
{
    [DefaultCommand(inGameOnly: true)]
    public string Spawn(string[] @params, BattleClient invokerClient)
    {
        if (invokerClient == null)
            return "You cannot invoke this command from console.";

        if (invokerClient.InGameClient == null)
            return "You can only invoke this command while in-game.";

        var player = invokerClient.InGameClient.Player;
        var actorSNO = 6652; /* zombie */
        var amount = 1;

        /*
        if (@params != null)
        {
            if (!Int32.TryParse(@params[0], out amount))
                amount = 1;

            if (amount > 100) amount = 100;

            if (@params.Count() > 1)
                if (!Int32.TryParse(@params[1], out actorSNO))
                    actorSNO = 6652;
        }
        */
        if (@params != null)
        {
            if (!int.TryParse(@params[0], out actorSNO))
                actorSNO = 6652;


            if (@params.Length > 1)
                if (!int.TryParse(@params[1], out amount))
                    amount = 1;
            if (amount > 100) amount = 100;
        }

        for (var i = 0; i < amount; i++)
        {
            var position = new Vector3D(player.Position.X + (float)RandomHelper.NextDouble() * 20f,
                player.Position.Y + (float)RandomHelper.NextDouble() * 20f,
                player.Position.Z);

            var monster = player.World.SpawnMonster((ActorSno)actorSNO, position);
        }

        return $"Spawned {amount} mobs with ActorSNO: {(ActorSno)actorSNO}";
    }
}