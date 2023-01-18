//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
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
