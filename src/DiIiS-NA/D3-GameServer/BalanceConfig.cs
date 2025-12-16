using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer
{
	public sealed class BalanceConfig : DiIiS_NA.Core.Config.Config
	{
        public bool SkeletonKingBalanceEnabled
        {
            get => GetBoolean(nameof(SkeletonKingBalanceEnabled), true);
            set => Set(nameof(SkeletonKingBalanceEnabled), value);
        }

        /// <summary>
        /// How much to divide Skeleton King's damage by.
        /// </summary>
        public float SkeletonKingDamageMultiplier
        {
            get => GetFloat(nameof(SkeletonKingDamageMultiplier), 1f);
            set => Set(nameof(SkeletonKingDamageMultiplier), value);
        }

        /// <summary>
        /// How much to divide Skeleton King's health by.
        /// </summary>
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

        public static BalanceConfig Instance { get; } = new();


        private BalanceConfig() : base("Balance")
		{
		}
	}
}
