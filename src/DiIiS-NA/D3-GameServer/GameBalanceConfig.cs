using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer
{
    public struct GameBalanceConfig
    {
        public float HitpointMultiplier { get; set; } = 1.0f;
        public float DamageMultiplier { get; set; } = 1.0f;
        public float WalkSpeed { get; set; } = 1.36f;
        public bool IsValid { get; private set; }
        public GameBalanceConfig(ActorSno actor)
        {
            float hitpointMultiplier = BalanceConfig.Instance.NormalBossHealthMultiplier;
            float damageMultiplier = BalanceConfig.Instance.NormalBossDamageMultiplier;
            float walkSpeed = 1.0f;
            switch (actor)
            {
                case ActorSno._skeletonking:
                    hitpointMultiplier = BalanceConfig.Instance.SkeletonKingHealthMultiplier;
                    damageMultiplier = BalanceConfig.Instance.SkeletonKingDamageMultiplier;
                    walkSpeed = BalanceConfig.Instance.SkeletonKingWalkSpeed;
                    break;
                case ActorSno._maghda:
                    hitpointMultiplier = BalanceConfig.Instance.MaghdaHealthMultiplier;
                    damageMultiplier = BalanceConfig.Instance.MaghdaDamageMultiplier;
                    walkSpeed = BalanceConfig.Instance.MaghdaWalkSpeed;
                    break;
                default:
                    hitpointMultiplier = BalanceConfig.Instance.NormalBossHealthMultiplier;
                    damageMultiplier = BalanceConfig.Instance.NormalBossDamageMultiplier;
                    walkSpeed = 1.3f;
                    break;
            }
            HitpointMultiplier = hitpointMultiplier;
            DamageMultiplier = damageMultiplier;
            WalkSpeed = walkSpeed;
            IsValid = true;
        }
    }
}
