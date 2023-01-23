//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using System.Collections.Generic;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
    [ImplementsPowerSNO(190230)]  // Enchantress_Melee_Instant.pow
    public class LeahBoom : ActionTimedSkill
    {
        public override IEnumerable<TickTimer> Main()
        {
            TargetList targets = new TargetList();
            var Summoners = World.GetActorsBySNO(ActorSno._triunesummoner_a_cainevent);
            foreach (var Summoner in Summoners)
                targets.Actors.Add(Summoner);
            WeaponDamage(targets, 100.00f, DamageType.Physical);
            User.PlayAnimation(5, AnimationSno.leah_hulkout_spellcast);

            User.World.BroadcastInclusive(plr => new SetIdleAnimationMessage
            {
                ActorID = User.DynamicID(plr),
                AnimationSNO = 147942
            }, User);
            yield break;
        }

        public override float GetActionSpeed()
        {
            return base.GetActionSpeed() * 1.2f;
        }
    }
}
