using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations.General
{
    //196243   class DropBanner
    [ImplementsPowerSNO(185040)]
    public class DropBanner : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            foreach (var old in User.World.GetActorsBySNO(ActorSno._emotebanner_player_1))
                old.Destroy();
            Vector3D Range = new Vector3D();
            Range = TargetPosition - User.Position;
            var TrueTarget = new Vector3D(User.Position.X + Range.X / 3, User.Position.Y + Range.Y / 3, TargetPosition.Z);
            var EffectOfDrop = SpawnEffect(ActorSno._banner_arrival_proxyactor, TrueTarget, 0, WaitSeconds(0.7f));
            
            var b = User.World.SpawnMonster(ActorSno._emotebanner_player_1, TrueTarget);
            (User as PlayerSystem.Player).PlayerDirectBanner = b;

            yield break;
        }
    }
}
