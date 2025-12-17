using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.D3_GameServer.GSSystem.ActorSystem.Implementations
{

    //[HandledSNO(ActorSno._skeletonking)]
    public class SkeletonKing : Monster
    {
        private readonly static Logger Logger = LogManager.CreateLogger(nameof(SkeletonKing));
        public SkeletonKing(World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            // Skeleton king adjustments based on server config for balance
            if (BalanceConfig.Instance.BalanceEnabled)
            {
                Logger.Trace("Applying $[red bold]$Skeleton King (Leoric)$[/]$ balance adjustments $[blue]$(Balance > SkeletonKingBalanceEnabled)$[/]$.");
                Attributes[GameAttributes.Hitpoints_Cur] /= BalanceConfig.Instance.SkeletonKingHealthMultiplier;
                Attributes[GameAttributes.Hitpoints_Max] /= BalanceConfig.Instance.SkeletonKingHealthMultiplier;
                Attributes[GameAttributes.DamageCap_Percent] /= BalanceConfig.Instance.SkeletonKingDamageMultiplier;
                Attributes[GameAttributes.Crit_Damage_Cap] /= BalanceConfig.Instance.SkeletonKingDamageMultiplier;
            }
        }


        public override bool Reveal(Player player)
        {
            if (!base.Reveal(player))
                return false;

            return true;
        }

        public override bool Unreveal(Player player)
        {
            if (!base.Unreveal(player))
                return false;

            return true;
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            base.OnTargeted(player, message);
        }
    }
}
