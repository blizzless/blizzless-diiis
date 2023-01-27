using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.D3_GameServer.GSSystem.ActorSystem.Implementations.Minions
{
    class AncientKorlic : AncientBarbarian
    {
        private static readonly int[] powers = new int[]
        {
            30592,  //Weapon_Instant
            187092, //basic melee
            168823, //cleave
            168824  //furious charge //Only Active with Rune_A
        };
        public AncientKorlic(World world, PowerContext context) : base(world, ActorSno._barbarian_calloftheancients_1, context)
        {
        }

        public override AnimationSno IntroAnimation => AnimationSno.barbarian_male_ancients_korlic_intro;

        protected override int[] Powers => powers;
    }
}
