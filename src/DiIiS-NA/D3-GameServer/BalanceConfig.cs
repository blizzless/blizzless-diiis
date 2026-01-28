using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer
{

    public sealed class BalanceConfig : DiIiS_NA.Core.Config.Config
	{
        public bool BalanceEnabled
        {
            get => GetBoolean(nameof(BalanceEnabled), true);
            set => Set(nameof(BalanceEnabled), value);
        }

        /// <summary>
        /// How much to divide Skeleton King's damage by.
        /// </summary>
        public float SkeletonKingDamageMultiplier
        {
            get => GetFloat(nameof(SkeletonKingDamageMultiplier), 1f);
            set => Set(nameof(SkeletonKingDamageMultiplier), value);
        }

        public float SkeletonKingHealthMultiplier
        {
            get => GetFloat(nameof(SkeletonKingHealthMultiplier), 1f);
            set => Set(nameof(SkeletonKingHealthMultiplier), value);
        }
        public float SkeletonKingWalkSpeed
        {
            get => GetFloat(nameof(SkeletonKingWalkSpeed), 1.0f);
            set => Set(nameof(SkeletonKingWalkSpeed), value);
        }

        public float MaghdaHealthMultiplier
        {
            get => GetFloat(nameof(MaghdaHealthMultiplier), 1.0f);
            set => Set(nameof(MaghdaHealthMultiplier), value);
        }

        public float MaghdaDamageMultiplier
        {
            get => GetFloat(nameof(MaghdaDamageMultiplier), 1.0f);
            set => Set(nameof(MaghdaDamageMultiplier), value);
        }
        public float MaghdaWalkSpeed
        {
            get => GetFloat(nameof(MaghdaWalkSpeed), 1.0f);
            set => Set(nameof(MaghdaWalkSpeed), value);
        }

        public float NormalBossHealthMultiplier
        {
            get => GetFloat(nameof(NormalBossHealthMultiplier), 1.0f);
            set => Set(nameof(NormalBossHealthMultiplier), value);
        }
        public float NormalBossDamageMultiplier
        {
            get => GetFloat(nameof(NormalBossDamageMultiplier), 1.0f);
            set => Set(nameof(NormalBossDamageMultiplier), value);
        }

        public float FixedCooldownSeconds
        {
            get => GetFloat(nameof(FixedCooldownSeconds), -1);
            set => Set(nameof(FixedCooldownSeconds), value);
        }

        public float NecroArmyOfTheDeadDamageMultiplier
        {
            get => GetFloat(nameof(NecroArmyOfTheDeadDamageMultiplier), 1);
            set => Set(nameof(NecroArmyOfTheDeadDamageMultiplier), value);
        }

        public float WaitTime(float time)
        {
            if (FixedCooldownSeconds >= 0)
                return FixedCooldownSeconds;
            return time;
        }
        public static BalanceConfig Instance { get; } = new();


        private BalanceConfig() : base("Balance")
		{
		}
	}
}
