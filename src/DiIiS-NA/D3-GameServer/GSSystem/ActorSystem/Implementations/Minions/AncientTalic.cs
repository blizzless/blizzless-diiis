//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.D3_GameServer.GSSystem.ActorSystem.Implementations.Minions
{
    class AncientTalic : AncientBarbarian
    {
        private static readonly int[] powers = new int[]
        {
            30592,  //Weapon_Instant
            187092, //basic melee
            168825, //Leap //Only Active with Rune_E
            168830  //WhirlWind
        };
        public AncientTalic(World world, PowerContext context) : base(world, ActorSno._barbarian_calloftheancients_2, context)
        {
        }

        public override AnimationSno IntroAnimation => AnimationSno.barbarian_male_ancients_talic_intro;

        protected override int[] Powers => powers;
    }
}
