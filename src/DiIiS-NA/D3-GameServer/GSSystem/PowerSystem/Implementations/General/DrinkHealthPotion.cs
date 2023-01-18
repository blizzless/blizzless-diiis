//Blizzless Project 2022 
using System.Collections.Generic;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations.General
{
    //30211   class DrinkHealthPotion
    [ImplementsPowerSNO(30211)]
    public class DrinkHealthPotion : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            if (User is Player)
            {
                Player player = (Player)User;
                player.AddPercentageHP(60);
                AddBuff(player, player, new CooldownBuff(30211, TickTimer.WaitSeconds(player.World.Game, 30f)));
            }

            yield break;
        }
    }
}
