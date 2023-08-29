using System.Collections.Generic;
using DiIiS_NA.D3_GameServer;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.LoginServer;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations.General
{
    //30211   class DrinkHealthPotion
    [ImplementsPowerSNO(30211)]
    public class DrinkHealthPotion : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            if (User is not Player player) yield break;
            player.AddPercentageHP(GameModsConfig.Instance.Health.PotionRestorePercentage);
            AddBuff(player, player, new CooldownBuff(30211, TickTimer.WaitSeconds(player.World.Game, GameModsConfig.Instance.Health.PotionCooldown)));
        }
    }
}
