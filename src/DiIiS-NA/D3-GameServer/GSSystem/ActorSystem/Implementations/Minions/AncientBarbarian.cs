using System.Linq;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.D3_GameServer.GSSystem.ActorSystem.Implementations.Minions
{
    abstract class AncientBarbarian : Minion
    {
        protected abstract int[] Powers { get; }
        public abstract AnimationSno IntroAnimation { get; }

        public AncientBarbarian(World world, ActorSno actorSno, PowerContext context) : base(world, actorSno, context.User, null)
        {
            Scale = 1.2f; //they look cooler bigger :)
                          //TODO: get a proper value for this.
            this.WalkSpeed *= 5;
            this.DamageCoefficient = context.ScriptFormula(11);
            var brain = new MinionBrain(this);
            foreach (var power in Powers)
            {
                brain.AddPresetPower(power);
            }
            SetBrain(brain);
            Attributes[GameAttributes.Summoned_By_SNO] = context.PowerSNO;
            Attributes[GameAttributes.Attacks_Per_Second] = 1.0f;

            Attributes[GameAttributes.Damage_Weapon_Min, 0] = context.ScriptFormula(11) * context.User.Attributes[GameAttributes.Damage_Weapon_Min_Total, 0];
            Attributes[GameAttributes.Damage_Weapon_Delta, 0] = context.ScriptFormula(11) * context.User.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0];

            Attributes[GameAttributes.Pet_Type] = 0x8;
            //Pet_Owner and Pet_Creator seems to be 0

            if (this.Master != null)
            {
                if (this.Master is Player)
                {
                    if ((this.Master as Player).Followers.Values.Count(a => a == SNO) > 1)
                        (this.Master as Player).DestroyFollower(SNO);
                }
            }

            LifeTime = TickTimer.WaitSeconds(world.Game, 30f);
        }
    }
}
