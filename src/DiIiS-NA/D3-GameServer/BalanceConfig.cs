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
        public float SkeletonKingDamageCapPercentage
        {
            get => GetFloat(nameof(SkeletonKingDamageCapPercentage), 1f);
            set => Set(nameof(SkeletonKingDamageCapPercentage), value);
        }

        /// <summary>
        /// How much to divide Skeleton King's health by.
        /// </summary>
        public float SkeletonKingHealthDivider
        {
            get => GetFloat(nameof(SkeletonKingHealthDivider), 1f);
            set => Set(nameof(SkeletonKingHealthDivider), value);
        }


        public static BalanceConfig Instance { get; } = new();

		private BalanceConfig() : base("Balance")
		{
		}
	}
}
