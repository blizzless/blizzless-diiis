using DiIiS_NA.GameServer.GSSystem.PowerSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.D3_GameServer.GSSystem.ActorSystem.Implementations.Minions
{
    class AncientMawdawc : AncientBarbarian
    {
        private static readonly int[] powers = new int[]
        {
            30592,  //Weapon_Instant
            187092, //basic melee
            168827, //Seismic Slam //Only Active with Rune_C
            168828  //Weapon Throw
        };
        public AncientMawdawc(World world, PowerContext context) : base(world, ActorSno._barbarian_calloftheancients_3, context)
        {
        }

        public override AnimationSno IntroAnimation => AnimationSno.barbarian_male_ancients_mawdawc_intro;

        protected override int[] Powers => powers;
    }
}
